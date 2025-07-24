using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange;
    public float attackRange;
    public float moveSpeed;

    private bool isPlayerDetected;

    private Health HP;
    void Start()
    {
        HP = GetComponent<Health>();
        HP.maxHealth = 25f; //최대 체력 설정

        detectionRange = 11f;
        attackRange = 3.0f;
        moveSpeed = 4f;

        isPlayerDetected = false;
    }
    void Update()
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

    void MoveTowardsPlayer()
    {
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