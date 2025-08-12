using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    protected Transform player;
    RevolverHealthSystem revolverHealthSystem;
    private Rigidbody2D EBrb;  // EnemyBullet의 Rigidbody2D 컴포넌트

    private float knockbackRange;
    private void Awake()
    {
        revolverHealthSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<RevolverHealthSystem>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어의 Transform을 찾음
        EBrb = GetComponent<Rigidbody2D>();         //현재 총알의 rigidbody
        knockbackRange = 0.0f;

        Destroy(gameObject, 3.0f);          // 3초뒤면 적군 총알 사라짐
    }
    public void EnemyShoot(Vector2 EnmeyBulletDirection)
    {
        if (EBrb == null) return;
        EBrb.linearVelocity = EnmeyBulletDirection * 10f; // 총알 이동 속도 설정
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject == null) return; // 총알이 없으면 무시
        else
        {
            if (other.CompareTag("Player") && revolverHealthSystem != null)
            {
                PlayerMove playerMove = other.GetComponent<PlayerMove>();

                Vector2 direction = (player.position - transform.position).normalized;   // 벡터 계산
                direction.y = 1.0f;                                                      // 벡터 계산
                Vector2 knockback = direction * knockbackRange;                          // 계산된 벡터에 넉백 거리 곱함  
                playerMove.ApplyKnockback(knockback);

                Debug.Log("플레이어에게 데미지 줌");
                Destroy(gameObject); // 총알 오브젝트 삭제
            }
            else if (other.CompareTag("Ground"))
            {
                Destroy(gameObject); // 총알이 땅에 닿으면 삭제
            }
        }
    }
    public void GetKnockBackRange(float knRange)    //WHAT THE FUCK? 이렇게 할 수 밖에 없었습니다.... By.계
    {                                               // 이것이 뭐하는 코드냐면 LongEnemy나 DroneEnemy에서 넉백 범위를 받아오기 위해
        knockbackRange = knRange;                   // 이렇게 구현했습니다.
    }
}
