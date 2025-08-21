using System.Collections;
using UnityEngine;

public class ShortEnemyAI : EnemyAI
{
    private PlayerMove playerMove;
    RevolverHealthSystem revolverHealthSystem;
    
    protected override void Init()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어의 Transform을 찾음
        playerMove = player.GetComponent<PlayerMove>(); // 플레이어 이동 스크립트 가져오기(넉백을 위해 필요)
        revolverHealthSystem = player.GetComponent<RevolverHealthSystem>(); // 플레이어의 리볼버 체력 시스템 가져오기

        HP = GetComponent<Health>();
        HP.maxHealth = 25f; //최대 체력 설정

        detectionRange = 11f;
        attackRange = 3.0f;
        moveSpeed = 4f;
        isPlayerDetected = false;

        damage = 1; // 공격력 설정
        attackCycle = 1.0f; // 공격 쿨타임 설정
        isAttacking = false; // 공격 중인지 여부 초기화
        knockbackRange = 4.0f; // 넉백 거리 설정
    }
    protected override IEnumerator EnemyAttack()
    {
        if (isAttacking) return null; // 이미 공격 중이면 중복 공격 방지       
        if (revolverHealthSystem != null)   // 데미지 적용
        {
            // 여기에 약간의 아이디어를 첨부하면?? ==> 사각형 콜라이더를 적군이 바라보는 방향에 추가해서
            // 사각형 콜라이더가 플레이어와 충돌하면 데미지를 받는 방식도 나쁘지 않은듯

            // 플레이어의 리볼버 체력 시스템에서 데미지를 적용
            revolverHealthSystem.TakeDamage(damage); // 나중에 float으로 전환해야돼 RevolverHealthSystem에서 데미지 적용할때
        }

        //벡터 계산해서 넉백 적용 근데 공격 애니메이션에 맞춰서 넉백 적용해야함 ==> 조건 달아야할듯
        Vector2 direction = (player.position - transform.position).normalized;   // 벡터 계산
        direction.y = 1.0f;                                                      // 벡터 계산
        Vector2 knockback = direction * knockbackRange;                          // 계산된 벡터에 넉백 거리 곱함  
        playerMove.ApplyKnockback(knockback);
        
        return base.EnemyAttack();
    }
    
}