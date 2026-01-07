using System.Collections;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    protected Transform player;
    protected float detectionRange;
    public float attackRange;
    protected float moveSpeed;

    public int damage;  // 공격력
    protected float attackCycle; // 공격 쿨타임
    protected bool isAttacking; // 공격 중인지 여부
    protected float knockbackRange; // 넉백 거리

    protected bool isPlayerDetected;
    
    protected Animator EnemyAnimator;
    protected BossHealth HP;
    private SpriteRenderer Enemysprend;

    [SerializeField] protected bool useBaseUpdateAI = true;

    protected virtual void Init() // 자식이 호출하는 메소드
    {}

    void Start()
    {
        EnemyAnimator = GetComponent<Animator>();
        Enemysprend = GetComponent<SpriteRenderer>();
        Init(); // 초기화 메소드 호출
        
    }

    protected virtual void Update()
    {
        if (!useBaseUpdateAI) return;
        if (player == null) return;

        float distanceToPlayer = GetDistanceToPlayer();

        // 탐지 (간단 거리 기반. Raycast 탐지면 기존 로직 유지해도 됨)
        isPlayerDetected = distanceToPlayer <= detectionRange;

        if (!isPlayerDetected)
        {
            StopMovement();
            return;
        }

        FacePlayer();

        if (distanceToPlayer > attackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StopMovement();
        }
    }


    protected float GetDistanceToPlayer()
    {
        if (player == null) return float.PositiveInfinity;
        return Vector2.Distance(transform.position, player.position);
    }

    protected void FacePlayer()
    {
        if (player == null || Enemysprend == null) return;

        float dir = player.position.x - transform.position.x;
        Enemysprend.transform.localScale = new Vector3(dir < 0 ? 1f : -1f, 1f, 1f);
    }

    protected virtual void MoveTowardsPlayer()  // why virtual? => 날아다니는 몹은 이동 방식이 다르므로 
    {                                           // 오버라이드를 사용함
        if (player == null) return;

        EnemyAnimator.SetBool("IsWalking", true);

        Vector2 direction = (player.position - transform.position).normalized;
        direction.y = 0f;
        direction = direction.normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    protected void StopMovement()
    {
        EnemyAnimator.SetBool("IsWalking", false);
    }
    protected virtual IEnumerator EnemyAttack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackCycle);
        isAttacking = false;
    }

    void OnDrawGizmosSelected()   // 탐지범위, 공격범위 확인용 gizmos (삭제해도 됨)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);  // 탐지 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);     // 공격 범위
    }
}