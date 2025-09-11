using UnityEngine;

public class EnemyBomb : MonoBehaviour
{
    protected Transform player;
    RevolverHealthSystem revolverHealthSystem;
    private Rigidbody2D EBrb;  // EnemyBomb의 Rigidbody2D 컴포넌트

    private float knockbackRange;
    private int damage;

    private bool istakedamage;
    private void Awake()
    {
        revolverHealthSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<RevolverHealthSystem>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어의 Transform을 찾음
        EBrb = GetComponent<Rigidbody2D>();         //현재 폭탄의 rigidbody
        knockbackRange = 0.0f;
        damage = 0;

        istakedamage = false;

        Destroy(gameObject, 3.0f);          // 3초뒤면 적군 폭탄 사라짐
    }
    public void EnemyShoot(Vector2 EnemyBulletDirection)
    {
        if (EBrb == null) return;
        EnemyBulletDirection.y = 0.5f;                  // 약간 위로 발사??? 나중에 조정해봐야됨
        EBrb.linearVelocity = EnemyBulletDirection * 10f; // 폭탄 이동 속도 설정
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (gameObject == null || istakedamage) return; // 폭탄이 없으면 무시
        else
        {
            if (other.gameObject.CompareTag("Player") && revolverHealthSystem != null)
            {
                istakedamage = true; // 플레이어에게 데미지를 주었음을 표시

                PlayerMove playerMove = other.gameObject.GetComponent<PlayerMove>();
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
            }
            else if (other.gameObject.CompareTag("Ground"))
            {
            }
        }
    }
    public void GetValue(float knRange, int dmg)    //WHAT THE FUCK? 이렇게 할 수 밖에 없었습니다.... By.계
    {                                               // 이것이 뭐하는 코드냐면 LongEnemy나 DroneEnemy에서 넉백 범위를 받아오기 위해
        knockbackRange = knRange;                   // 이렇게 구현했습니다.
        damage = dmg;
    }
}
