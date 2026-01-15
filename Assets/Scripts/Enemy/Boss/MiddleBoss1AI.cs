using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.U2D.Animation;
using UnityEngine;

// 공격 패턴 타입 열거형
public enum AttackType
{
    BasicAttack,
    JumpAttack,
    DashAttack,
    TripleJumpAttack,
    ChainHookAttack
}

public class MiddleBoss1AI : BossAI
{
    private PlayerMove playerMove;
    RevolverHealthSystem revolverHealthSystem;
    private BossHealth bossHealth;
    private Dictionary<AttackType, bool> patternEnabled; // 공격 패턴 활성화 상태 저장용 딕셔너리
    private SpriteRenderer spriteRenderer;

    // 손 모양 변경용 필드 - SpriteResolver - 2페이즈 변환 시 검에서 훅으로 변경
    [SerializeField] private SpriteResolver handResolver;
    private string handCategory = "hand_R";
    private string swordLabel = "hand_sword_R";
    private string hookLabel = "hand_hook_R";

    // 패턴 공격 관련 변수
    private bool actionLock = false; // 패턴 공격 할 때, 잠금 플래그
    private float approachTimer = 0f; // 접근 시간 측정용 타이머
    private Coroutine bossLoopCoroutine;
    // 기본 공격 히트박스
    [SerializeField] private BossHitbox basicAttackHitbox; 
    // 점프 공격 히트박스 및 플래그
    [SerializeField] private BossHitbox jumpBodyHitbox;
    [SerializeField] private BossHitbox landAoeHitbox;
    // 점프 히트 플래그
    private bool JumpHitPlayer = false;
    private bool isTripleJumping = false;
    // 대쉬 공격 히트박스 및 필드 추가
    [SerializeField] private BossHitbox dashAttackHitbox;
    private float dashAccelTime;
    private float dashDecelTime;
    private float dashMaxSpeed;
    private bool dashStopRequested = false;
    // 체인 공격 필드 추가
    private bool forceHookOnPhase2Enter = true; // 2페이즈로 변환 시 강제로 사슬 공격 하도록 하는 플래그
    private float lastChainHookHealth; // 사슬 공격 전 -35만큼의 조건을 알기 위한 필드
    private bool chainHookFirstUse = true; // 첫 사용 조건 면제용
    [SerializeField] private HookProjectile HookPrefab;
    [SerializeField] private Transform hookSpawnPoint; // 공격 시작 지점
    private bool hookResolved = false;
    private bool hookHit = false;
    private Vector3 hookMissPos;

    protected override void Init()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // 플레이어의 Transform을 찾음
        playerMove = player.GetComponent<PlayerMove>(); // 플레이어 이동 스크립트 가져오기(넉백을 위해 필요)
        revolverHealthSystem = player.GetComponent<RevolverHealthSystem>(); // 플레이어의 리볼버 체력 시스템 가져오기
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        bossHealth = GetComponent<BossHealth>();
        bossHealth.maxHealth = 500.0f; //최대 체력 설정
        bossHealth.currentEnemyHealth = bossHealth.maxHealth; // 현재 체력 초기화
        // 기본 스탯 설정
        detectionRange = 25.0f;
        attackRange = 3.0f;
        moveSpeed = 1.0f;
        isPlayerDetected = false;

        damage = 1; // 공격력 설정
        attackCycle = 1.0f; // 공격 쿨타임 설정
        isAttacking = false; // 공격 중인지 여부 초기화
        knockbackRange = 4.0f; // 넉백 거리 설정

        useBaseUpdateAI = false;

        // 대쉬 공격 필드 초기화
        dashAccelTime = 1.0f;   // 대쉬 가속 시간
        dashDecelTime = 1.0f;   // 대쉬 감속 시간
        dashMaxSpeed  = 10.0f;  // 대쉬 최대 속도

        // 사슬 공격 필드 초기화
        lastChainHookHealth = bossHealth.currentEnemyHealth; // 이전 체력에서 감소된 체력을 계산하기 위해서 사용

        // 히트박스에 데미지/넉백 값 전달
        basicAttackHitbox?.GetValue(knockbackRange, damage);
        jumpBodyHitbox?.GetValue(knockbackRange, damage);
        landAoeHitbox?.GetValue(knockbackRange, damage);
        dashAttackHitbox?.GetValue(knockbackRange, damage);

        // 공격 패턴 초기화
        patternEnabled = new Dictionary<AttackType, bool>()
        {
            { AttackType.BasicAttack, false },
            { AttackType.JumpAttack, false },
            { AttackType.DashAttack, false },
            { AttackType.TripleJumpAttack, false },
            { AttackType.ChainHookAttack, false }
        }; 

        ApplyPhaseSettings(bossHealth.Phase); // 시작 페이즈 적용

        if(bossLoopCoroutine == null)
        {
            bossLoopCoroutine = StartCoroutine(BossBehaviorLoop());
        }
    }

    // 이벤트 구독 설정 // 이벤트 구독은 생명주기와 같이 가야함
    private void OnEnable()
    {
        if(bossHealth == null) bossHealth = GetComponent<BossHealth>();
        bossHealth.OnPhase2Request += StartPhase2Transition; // 페이즈 2 진입 요청 이벤트 구독
        bossHealth.OnPhaseChanged += HandlePhaseChange; // 이벤트 구독

        // 대쉬 공격 히트박스에 플레이어 넉백 요청 액션 구독
        if(dashAttackHitbox != null) dashAttackHitbox.OnHitPlayer += OnDashHitPlayer;
        // 점프 공격 히트박스에 플레이어 넉백 요청 액션 구독
        if (jumpBodyHitbox != null) jumpBodyHitbox.OnHitPlayer += OnJumpHitPlayer;
        if (landAoeHitbox != null) landAoeHitbox.OnHitPlayer += OnJumpHitPlayer;

    }
    // 이벤트 구독 취소
    private void OnDisable()
    {
        if (bossHealth != null) {
            bossHealth.OnPhase2Request -= StartPhase2Transition; // 페이즈 2 진입 요청 이벤트 구독 해제
            bossHealth.OnPhaseChanged -= HandlePhaseChange; // 이벤트 구독 해제
        }
        // 대쉬 공격 히트박스 액션 구독 해제
        if(dashAttackHitbox != null) dashAttackHitbox.OnHitPlayer -= OnDashHitPlayer;
        // 점프 공격 히트박스 액션 구독 해제
        if (jumpBodyHitbox != null) jumpBodyHitbox.OnHitPlayer -= OnJumpHitPlayer;
        if (landAoeHitbox != null) landAoeHitbox.OnHitPlayer -= OnJumpHitPlayer;

        if(bossLoopCoroutine != null)
        {
            StopCoroutine(bossLoopCoroutine);
            bossLoopCoroutine = null;
        }
    }
    // 애니메이션 이벤트 전용 핸들러 메서드 - 손을 훅으로 변경 (연출용 메서드)
    public void AnimEvent_SetHookHand()
    {
        if (handResolver == null) return;

        handResolver.SetCategoryAndLabel(handCategory, hookLabel);
        handResolver.ResolveSpriteToSpriteRenderer();
    }

    // 이벤트 전용 핸들러 메서드 // BossHealth의 페이즈 변경 이벤트가 발생하면 호출됨
    private void HandlePhaseChange(int newPhase)
    {
        ApplyPhaseSettings(newPhase);
    }

    // 페이즈에 따른 공격 패턴 설정 메서드
    private void ApplyPhaseSettings(int phase)
    {
        DisableAllPatterns();
        // 패턴에 따른 공격 패턴 변경
        if(phase == 1) 
        {
            // 1페이즈에서 사용하는 공격 패턴 ON
            patternEnabled[AttackType.BasicAttack] = true;
            patternEnabled[AttackType.JumpAttack] = true;
            patternEnabled[AttackType.DashAttack] = true;
            
        }
        else if(phase == 2)
        {
            // 2페이즈에서 사용하는 공격 패턴 ON
            patternEnabled[AttackType.BasicAttack] = true;
            patternEnabled[AttackType.TripleJumpAttack] = true;
            patternEnabled[AttackType.ChainHookAttack] = true;
        }
    }

    // 페이즈 2 전환 코루틴 시작 메서드
    private void StartPhase2Transition()
    {
        StartCoroutine(Phase2Transition());
    }
    // 페이즈 2 전환 코루틴 - 애니메이션 재생 후 페이즈 변경
    private IEnumerator Phase2Transition()
    {

        actionLock = true; // 행동 잠금
        // 페이즈 전환 애니메이션 재생
        EnemyAnimator.SetTrigger("PhaseChange");
        yield return new WaitForSeconds(3.0f); // 애니메이션 시간에 맞춰 조정

        bossHealth.SetPhase(2); // 페이즈 변경
        bossHealth.SetInvicible(false); // 무적 상태 해제

        // 2페 진입 즉시 사슬공격 1회 강제 
        if (forceHookOnPhase2Enter)
        {
            yield return StartCoroutine(pattern_ChainHookAttack());

            forceHookOnPhase2Enter = false;

            // 패턴 후 1.0초 정지
            yield return new WaitForSeconds(1.0f);
        }


        actionLock = false; // 행동 잠금 해제
    }

    // 대쉬 공격이 플레이어를 맞췄을 때 호출되는 메서드
    private void OnDashHitPlayer()
    {
        dashStopRequested = true;
    }
    // 점프 공격이 플레이어를 맞췄을 때 호출되는 메서드
    private void OnJumpHitPlayer()
    {
        if (!isTripleJumping) return;
        JumpHitPlayer = true;
    }

    // 페이즈가 변경될 때 모든 패턴 비활성화
    private void DisableAllPatterns()
    {
        var keys = new System.Collections.Generic.List<AttackType>(patternEnabled.Keys);
        foreach (var k in keys)
            patternEnabled[k] = false;
    }

    private IEnumerator BossBehaviorLoop()
    {
        while (true)
        {
            // 초기화/태그 누락 방어
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player")?.transform;
                yield return null;
                continue;
            }

            approachTimer += Time.deltaTime;

            while (actionLock)
            {
                StopMovement();
                yield return null;
                continue;
            }

            float dist = GetDistanceToPlayer();

            // 탐지 범위 밖이면 멈추기
            if (dist > detectionRange)
            {
                StopMovement();
                yield return null;
                continue;
            }

            FacePlayer();

            // 패턴 선택
            AttackType? next = SelectNextPattern();

            // 선택된 패턴이 없으면 기본 추적
            if (next == null)
            {
                if (dist > attackRange) MoveTowardsPlayer();
                else StopMovement();
                yield return null;
                continue;
            }

            // 패턴 실행
            actionLock = true;

            if (IsApproachPattern(next.Value))
                approachTimer = 0f;

            yield return ExecutePattern(next.Value);

            StopMovement();
            yield return new WaitForSeconds(1.0f); // 공통 후딜

            actionLock = false;
            yield return null;
        }
    }

    //[접근]패턴인지 확인 하는 메서드
    private bool IsApproachPattern(AttackType t)
    {
        return t == AttackType.JumpAttack
            || t == AttackType.DashAttack
            || t == AttackType.TripleJumpAttack;
    }
    // 현재 페이즈에서 활성화(true) 된 패턴 중, 실행 조건을 만족하는 패턴을 선택
    private AttackType? SelectNextPattern()
    {
        List<AttackType> candidates = new();
        foreach(var pattern in patternEnabled)
        {
            if(!pattern.Value) continue; // 비활성화된 패턴은 건너뜀
            if(IsPatternConditionMet(pattern.Key))
            {
                candidates.Add(pattern.Key);
            }
        }

        if(candidates.Count == 0)
        {
            return null; // 실행 가능한 패턴이 없음
        }

        return candidates[Random.Range(0, candidates.Count)];
    }
    // 패턴별 실행 조건 확인 메서드
    private bool IsPatternConditionMet(AttackType pattern)
    {
        float dist = Vector3.Distance(transform.position, player.position);

        switch(pattern)
        {
            case AttackType.BasicAttack:
                return dist <= 3.0f && IsPlayerInFront();
            case AttackType.JumpAttack:
            case AttackType.DashAttack:
            case AttackType.TripleJumpAttack:
                return approachTimer >= 3.0f && dist > 1.0f && dist <= 15.0f;
            case AttackType.ChainHookAttack:
                {
                    bool hpOK = chainHookFirstUse || (lastChainHookHealth - bossHealth.currentEnemyHealth) >= 35.0; 
                    return dist > 15.0f && hpOK;
                }
                
            default:
                return false;
        }
    }
    // 패턴별 코루틴 호출(실행) 메서드
    private IEnumerator ExecutePattern(AttackType pattern)
    {
        switch(pattern)
        {
            case AttackType.BasicAttack:
                yield return StartCoroutine(Pattern_BasicAttack());
                break;
            case AttackType.JumpAttack:
                yield return StartCoroutine(pattern_JumpAttack());
                break;
            case AttackType.DashAttack:
                yield return StartCoroutine(pattern_DashAttack());
                break;
            case AttackType.TripleJumpAttack:
                yield return StartCoroutine(pattern_TripleJumpAttack());
                break;
            case AttackType.ChainHookAttack:
                yield return StartCoroutine(pattern_ChainHookAttack());
                break;
        }
    }
    // 히트박스 활성화/비활성화 메서드
    // 기본 공격
    public void EnableBasicHitbox() => basicAttackHitbox.EnableHitbox();
    public void DisableBasicHitbox() => basicAttackHitbox.DisableHitbox();
    // 점프 공격
    public void EnableJumpBodyHitbox() => jumpBodyHitbox.EnableHitbox();
    public void DisableJumpBodyHitbox() => jumpBodyHitbox.DisableHitbox();
    public void LandAoEAttack()
    {
        if (landAoeHitbox == null) return;
        StartCoroutine(LandAoeTickCo());
    }
    private IEnumerator LandAoeTickCo()
    {
        landAoeHitbox.EnableHitbox();
        yield return null;
        landAoeHitbox.DisableHitbox();
    }
    // 돌진 공격
    public void EnableDashAttackHitbox() => dashAttackHitbox.EnableHitbox();
    public void DisableDashAttackHitbox() => dashAttackHitbox.DisableHitbox();

    private IEnumerator Pattern_BasicAttack() // [메모] 공통 후딜은 ExecutePattern에서 처리
    {
        // 기본 공격 패턴 구현
        EnemyAnimator.SetTrigger("BasicAttack");
        yield return new WaitForSeconds(0.45f); // 공격 모션 재생 시간
    }
    private IEnumerator pattern_JumpAttack()
    {
        // 점프 공격 패턴 구현
        EnemyAnimator.SetTrigger("JumpAttack");
        yield return StartCoroutine(JumpToPlayer(0.5f, 0.5f, 5.0f));
        EnemyAnimator.SetTrigger("JumpAttackStop");
    }
    private IEnumerator pattern_DashAttack()
    {
        // 대시 공격 패턴 구현
        dashStopRequested = false;

        EnemyAnimator.SetTrigger("DashAttack");
        yield return new WaitForSeconds(0.2f); // 선딜 - 달릴 준비

        float dir = Mathf.Sign(player.position.x - transform.position.x);
        if(dir == 0) dir = 1f;

        // 가속 구간
        float t = 0f;
        while (t < dashAccelTime)
        {
            t += Time.deltaTime;
            float speed = Mathf.Lerp(0f, dashMaxSpeed, t / dashAccelTime);
            transform.position += new Vector3(dir * speed * Time.deltaTime, 0f, 0f);
            
            // 피격되면 즉시 감속
            if(dashStopRequested) break;

            // x좌표 도달 시 감속
            if((dir > 0f && transform.position.x >= player.position.x) ||
               (dir < 0f && transform.position.x <= player.position.x))
            { break; }

            yield return null;
        }

        // 플레이어 가 히트 전까지 최대 속도 유지
        while (!dashStopRequested)
        {
            // 대시 유지 구간 (플레이어 위치에 도달할 때까지)
            transform.position += new Vector3(dir * dashMaxSpeed * Time.deltaTime, 0f, 0f);

            if((dir > 0f && transform.position.x >= player.position.x) ||
               (dir < 0f && transform.position.x <= player.position.x))
            { break; }

            yield return null;
        }

        // 감속 구간
        EnemyAnimator.SetTrigger("DashStop");
        t = 0f;
        while (t < dashDecelTime)
        {
            t += Time.deltaTime;
            float speed = Mathf.Lerp(dashMaxSpeed, 0f, t / dashDecelTime);
            transform.position += new Vector3(dir * speed * Time.deltaTime, 0f, 0f);
            yield return null;
        }
    }
    private IEnumerator pattern_TripleJumpAttack()
    {
        // 트리플 점프 공격 패턴 구현
        isTripleJumping = true;

        // 첫 도약 (선딜 0.5초)
        JumpHitPlayer = false;
        EnemyAnimator.SetTrigger("JumpAttack");
        yield return StartCoroutine(JumpToPlayer(0.5f, 0.5f, 5.0f));
        if (JumpHitPlayer) // 플레이어를 맞췄으면 다음 점프로 넘어가지 않음
        {
            isTripleJumping = false;
            EnemyAnimator.SetTrigger("JumpAttackStop");
            yield break; 
        }
        // 나머지 2회 도약 (선딜 0.3초)
        for(int i = 0; i < 2; i++)
        {
            JumpHitPlayer = false;
            EnemyAnimator.SetTrigger("JumpAttackFast");
            yield return StartCoroutine(JumpToPlayer(0.4f, 0.5f, 5.0f));
            if (JumpHitPlayer) // 플레이어를 맞췄으면 다음 점프로 넘어가지 않음
            {
                isTripleJumping = false;
                yield break; 
            }
        }
        
        EnemyAnimator.SetTrigger("JumpAttackStop");
        isTripleJumping = false;
    }
    private IEnumerator pattern_ChainHookAttack()
    {
        // 체인 훅 공격 패턴 구현
        MarkChainHookUsed();

        hookResolved = false;
        hookHit = false;
        hookMissPos = Vector3.zero;

        // 1) 0.3초 팔 뻗기
        EnemyAnimator.SetTrigger("HookAttack");
        yield return new WaitForSeconds(0.3f);

        // 2) 갈고리 발사
        Vector2 spawn = hookSpawnPoint.position;
        Vector2 target = player.position;

        Vector2 fireDir = (target - spawn).normalized;

        Quaternion lookRotat = Quaternion.FromToRotation(Vector2.right, fireDir);
        Quaternion rotat = lookRotat * Quaternion.Euler(0f, 0f, -90f);

        var hook = Instantiate(HookPrefab, hookSpawnPoint.position, rotat);

        hook.Init(player, fireDir, OnChainHookHit, OnChainHookMiss);
        
        // 훅 결과(히트/미스) 기다림
        while (!hookResolved)
            yield return null;

        if (hookHit)
        {
            yield return StartCoroutine(Co_ChainHitSequence());
        }
        else
        {
            // 미스: 0.7초 정지 후 훅 위치로 이동(초당 20타일)
            yield return new WaitForSeconds(0.7f);
            yield return StartCoroutine(Co_MoveToX(hookMissPos.x, 20f));
        }

    }

    // 플레이어가 보스 앞에 있는지 확인하는 메서드
    private bool IsPlayerInFront()
    {
        float dir = spriteRenderer.flipX ? -1f : 1f; // 보스가 보는 방향
        float dx = player.position.x - transform.position.x;

        return dx * dir > 0f;
    }
    // 플레이어 위치로 점프하는 코루틴
    private IEnumerator JumpToPlayer(float windup, float duration, float arcHeight)
    {
        // 점프 공격 패턴 구현
        yield return new WaitForSeconds(windup); // 선딜 - 도약 준비

        // 3) 이 시점 플레이어 위치로 목표 확정
        Vector3 start = transform.position;
        Vector3 target = player.position;
        target.y = start.y;

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / duration);

            float yOffset = arcHeight * 4f * n * (1f - n);

            Vector3 pos = Vector3.Lerp(start, target, n);
            pos.y += yOffset;
            transform.position = pos;

            yield return null;
        }

        transform.position = target;
        LandAoEAttack();
    }

    // 사슬 공격 시작 시 체력이 -35가 됐을 때, 체력을 갱신 시켜주는 메소드
    private void MarkChainHookUsed()
    {
        chainHookFirstUse = false;
        lastChainHookHealth = bossHealth.currentEnemyHealth;
    }
    private void OnChainHookHit()
    {
        if (hookResolved) return;
        hookResolved = true;
        hookHit = true;
    }
    private void OnChainHookMiss(Vector3 missPos)
    {
        if (hookResolved) return;
        hookResolved = true;
        hookHit = false;
        hookMissPos = missPos;
    }
     // Chain HIT 시퀀스
    private IEnumerator Co_ChainHitSequence()
    {
        // 1차 데미지
        revolverHealthSystem.TakeDamage(1);

        // 기절(이동 제한) - PlayerMove에 FreezeMovement가 있음
        float pullSpeed = 12f;

        playerMove.SetExternalControl(true); // player 움직임 제한 on
        // 끌려오기
        yield return StartCoroutine(Co_PullPlayerToBoss(pullSpeed)); // 12의 속도로 끌려옴
        playerMove.SetExternalControl(false); // player 움직임 제한 off
    }
    // 플레이어 끌기
    private IEnumerator Co_PullPlayerToBoss(float speed)
    {
        bool attacked = false;
        float t = 0f;

        while (Mathf.Abs(transform.position.x - player.position.x) > 1.0f)
        {
            if (!attacked && Mathf.Abs(transform.position.x - player.position.x) < 2.0f)
            {
                attacked = true;
                EnemyAnimator.SetTrigger("BasicAttack");
                yield return new WaitForSeconds(0.5f);
            }

            t += Time.deltaTime;

            Vector3 target = transform.position;
            target.y = player.position.y; // 수평 끌기

            Vector3 dir = (target - player.position).normalized;
            player.position += dir * speed * Time.deltaTime;

            yield return null;
        }
    }
    // 미스 시 훅 위치로 이동
    private IEnumerator Co_MoveToX(float targetX, float speed)
    {
        while (Mathf.Abs(transform.position.x - targetX) > 0.1f)
        {
            float dir = Mathf.Sign(targetX - transform.position.x);
            transform.position += new Vector3(dir * speed * Time.deltaTime, 0f, 0f);
            yield return null;
        }
    }
    protected override IEnumerator EnemyAttack()
    {
        if (isAttacking) return null; // 이미 공격 중이면 중복 공격 방지       

        if (revolverHealthSystem != null)   // 데미지 적용
        {
            // knifeObject.GetComponent<knife>().GetValue(knockbackRange, damage);

        }
        return base.EnemyAttack();
    }

}