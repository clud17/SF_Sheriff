using System.Runtime.CompilerServices;
using UnityEngine;

public class DroneEnemyAI : EnemyAI
{
    private PlayerMove playerMove;
    RevolverHealthSystem revolverHealthSystem;
    protected override void Init()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어의 Transform을 찾음
        playerMove = player.GetComponent<PlayerMove>(); // 플레이어 이동 스크립트 가져오기(넉백을 위해 필요)
        revolverHealthSystem = player.GetComponent<RevolverHealthSystem>(); // 플레이어의 리볼버 체력 시스템 가져오기
        
        HP = GetComponent<Health>();
        HP.maxHealth = 10f; //최대 체력 설정

        detectionRange = 11f;
        attackRange = 7.0f;
        moveSpeed = 4.0f;
        isPlayerDetected = false;

        damage = 1.0f; // 공격력 설정
        attackCooldown = 2.0f; // 공격 쿨타임 설정
        knockbackRange = 2.0f; // 넉백 거리 설정
    }
    protected override void MoveTowardsPlayer()   // 날아다니는 몹은 이동하는 방식이 다르므로 오버라이드
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime; // 이동
    }
    protected override void EnemyAttack()
    {
        base.EnemyAttack();

        // ======투사체 생성해서 player랑 충돌하면 넉백===
        if (revolverHealthSystem != null)   // 데미지 적용
        {
            // 플레이어의 리볼버 체력 시스템에서 데미지를 적용
            revolverHealthSystem.TakeDamage((int)damage * 10); // 나중에 float으로 전환해야돼 RevolverHealthSystem에서 데미지 적용할때
        }
        
        //벡터 계산해서 넉백 적용 근데 공격 애니메이션에 맞춰서 넉백 적용해야함 ==> 조건 달아야할듯
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 knockback = direction * knockbackRange;

        playerMove.ApplyKnockback(knockback);
    }
}