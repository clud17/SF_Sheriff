using System.ComponentModel.Design;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    protected Transform player;
    protected float detectionRange;
    protected float attackRange;
    protected float moveSpeed;

    protected bool isPlayerDetected;
    
    protected Health HP;
    
    protected virtual void Init() // 자식이 호출하는 메소드
    {}

    void Start()
    {
        Init(); // 초기화 메소드 호출
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
            StopMovement(); // 플레이어가 탐지 범위를 벗어나면 멈춤
            return; // 탐지 범위를 벗어나면 이후 로직 실행하지 않음
        }

        //플레이어를 탐지했으면 행동 시작
        if (isPlayerDetected)
        {
            //공격 범위보다 멀면 이동
            if (distanceToPlayer > attackRange)
            {
                MoveTowardsPlayer();
            }
            else //stop and attack
            {
                //공격 범위 안이면 멈춤 (공격 준비)
                StopMovement();

            }
        }
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
        //정지 상태 유지

    }
    void OnDrawGizmosSelected()   // 탐지범위, 공격범위 확인용 gizmos (삭제해도 됨)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);  // 탐지 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);     // 공격 범위
    }
}
