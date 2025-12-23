using UnityEngine;
/// <summary>
/// 원거리 적군이 발사하는 총알
/// </summary>
public class EnemyBullet : MonoBehaviour
{
    protected Transform player;
    RevolverHealthSystem revolverHealthSystem;
    private Rigidbody2D EBrb;  // EnemyBullet의 Rigidbody2D 컴포넌트

    private float knockbackRange;
    private int damage;

    private bool istakedamage;
    private void Awake()
    {
        revolverHealthSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<RevolverHealthSystem>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어의 Transform을 찾음
        EBrb = GetComponent<Rigidbody2D>();         //현재 총알의 rigidbody
        knockbackRange = 0.0f;
        damage = 0;

        istakedamage = false;

        Destroy(gameObject, 3.0f);          // 3초뒤면 적군 총알 사라짐
    }
    public void EnemyShoot(Vector2 EnemyBulletDirection)
    {
        // 만약 ray를 구현한다면 여기에
        if (EBrb == null) return;
        EBrb.linearVelocity = EnemyBulletDirection * 50.0f; // 총알 이동 속도 설정
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject == null || istakedamage) return; // 총알이 없으면 무시
        else
        {
            if (other.CompareTag("Player") && revolverHealthSystem != null)
            {
                istakedamage = true; // 플레이어에게 데미지를 주었음을 표시

                PlayerMove playerMove = other.GetComponent<PlayerMove>();
                Vector2 direction = (player.position - transform.position).normalized;   // 벡터 계산
                direction.y = 1.0f;                                                      // 벡터 계산
                Vector2 knockback = direction * knockbackRange;                          // 계산된 벡터에 넉백 거리 곱함  
                playerMove.ApplyKnockback(knockback);

                Debug.Log("플레이어에게 데미지 줌");
                if (revolverHealthSystem != null)   // 데미지 적용
                {
                    // 플레이어의 리볼버 체력 시스템에서 데미지를 적용
                    revolverHealthSystem.TakeDamage(damage); // 나중에 float으로 전환해야돼 RevolverHealthSystem에서 데미지 적용할때
                }
                Destroy(gameObject); // 총알 오브젝트 삭제
            }
            else if (other.CompareTag("Ground"))
            {
                Destroy(gameObject); // 총알이 땅에 닿으면 삭제
            }
        }
    }
    public void GetValue(float knRange, int dmg)    //WHAT THE FUCK? 이렇게 할 수 밖에 없었습니다.... By.계
    {                                               // 이것이 뭐하는 코드냐면 LongEnemy나 DroneEnemy에서 넉백 범위를 받아오기 위해
        knockbackRange = knRange;                   // 이렇게 구현했습니다.
        damage = dmg;
    }


}
