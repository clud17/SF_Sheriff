using System.Collections;
using System.ComponentModel.Design;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    protected Transform player;
    protected float detectionRange;
    protected float attackRange;
    protected float moveSpeed;

    protected int damage;  // 공격력
    protected float attackCycle; // 공격 쿨타임
    protected bool isAttacking; // 공격 중인지 여부
    protected float knockbackRange; // 넉백 거리

    protected bool isPlayerDetected;
    
    private Animator EnemyAnimator;
    protected Health HP;
    
    protected virtual void Init() // 자식이 호출하는 메소드
    {}

    void Start()
    {
        Init(); // 초기화 메소드 호출
        EnemyAnimator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        //플레이어 탐지
        if (distanceToPlayer <= detectionRange)
        {
            isPlayerDetected = true;
        }
        else
        {
            isPlayerDetected = false;
            StopMovement();
            Invoke("PatrolMovement", 1.0f);
            // isPlayerDetected = false;
            // if (!isPatrolling)
            // {
            //     StopMovement();
            //     Invoke("PatrolMovement", 1.0f);
            // }
        }

        //플레이어를 탐지했으면 행동 시작
        if (isPlayerDetected)
        {
            //공격 범위보다 멀면 이동
            if (distanceToPlayer > attackRange && isAttacking == false)
            {
                MoveTowardsPlayer();
            }
            else //stop and attack
            {
                //공격 범위 안이면 멈춤 (공격 준비)
                StopMovement();
                if (isAttacking == false) // 공격 중이 아니면 공격 시작
                {
                    StartCoroutine(EnemyAttack());
                }
                
            }
        }

        // if (!isPlayerDetected && isPatrolling)
        // {
        //     transform.position += (Vector3)patrolDirection * moveSpeed * Time.deltaTime;
        //     patrolTimer -= Time.deltaTime;

        //     if (patrolTimer <= 0f)
        //     {
        //         isPatrolling = false;
        //         Invoke("PatrolMovement", 0.5f); // 다음 패트롤 방향 기다림
        //     }
        // }
    }

    protected virtual void MoveTowardsPlayer()  // why virtual? => 날아다니는 몹은 이동 방식이 다르므로 
    {                                           // 오버라이드를 사용함
        Vector2 direction = (player.position - transform.position).normalized;
        direction.y = 0f;
        direction = direction.normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    void StopMovement()
    {
        //정지 상태 유지(아무 코드 없는게 정상)
    }
    // protected Vector2 patrolDirection;
    // protected float patrolMoveDuration = 1f;
    // protected float patrolTimer = 0f;
    // protected bool isPatrolling = false;
    void PatrolMovement()
    {
        // //패트롤 이동 로직
        // patrolDirection = new Vector2(Random.Range(-1f, 1f), 0f).normalized;

        // // 타이머 초기화
        // patrolTimer = patrolMoveDuration;
        // isPatrolling = true;
    }
    protected virtual IEnumerator EnemyAttack()
    {
        if (isAttacking) yield break;
        isAttacking = true; // 공격 시작
        EnemyAnimator.SetBool("isAttacking", true);
        
        Debug.Log($"{attackCycle}초 후 공격");
        yield return new WaitForSeconds(attackCycle); // 공격 쿨타임 대기
        EnemyAnimator.SetBool("isAttacking", false);
        isAttacking = false; // 공격 종료
    }

    void OnDrawGizmosSelected()   // 탐지범위, 공격범위 확인용 gizmos (삭제해도 됨)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);  // 탐지 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);     // 공격 범위
    }
}
