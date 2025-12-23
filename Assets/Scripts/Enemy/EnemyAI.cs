using System.Collections;
using System.ComponentModel.Design;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
/// <summary>
/// 적 AI의 기본 클래스. 모든 적은 이 클래스를 상속받음
/// </summary>
public class EnemyAI : MonoBehaviour
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
    protected Health HP;
    private SpriteRenderer Enemysprend;
    protected virtual void Init() // 자식이 호출하는 메소드
    {}

    void Start()
    {
        Init(); // 초기화 메소드 호출
        EnemyAnimator = GetComponent<Animator>();
        Enemysprend = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        //플레이어 탐지 // ray 사용해야함 if문 바꿔야함
        Vector2 dir = (player.position - transform.position).normalized;

        LayerMask mask = LayerMask.GetMask("Player", "Ground", "Gate");           // 플레이어와 장애물 레이어만 감지
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectionRange, mask);  // 등록된 레이어만 감지하도록

        Debug.DrawRay(transform.position, dir * detectionRange, Color.green);   // 디버그용 레이 시각화(없애도 됨)

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Player")) // null이 아니거나 플레이어 태그이면 감지
        {
            isPlayerDetected = true;
        }
        else
        {
            isPlayerDetected = false;
            StopMovement();
        }

        //플레이어를 탐지했으면 행동 시작
        if (isPlayerDetected)
        {
            Enemysprend.transform.localScale = new Vector3(player.transform.position.x < transform.position.x ? 1f : -1f, 1f, 1f);

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
