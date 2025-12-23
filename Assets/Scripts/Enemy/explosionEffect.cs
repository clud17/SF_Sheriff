using UnityEngine;

public class explosionEffect : MonoBehaviour
{
    //폭발 이펙트 프리팹에 들어갈 코드입니다.
    public GameObject ExplosionEffect;
    protected Transform player;
    RevolverHealthSystem revolverHealthSystem;
    private float knockbackRange;
    private int damage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        revolverHealthSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<RevolverHealthSystem>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어의 Transform을 찾음
    }
    void Start() // 폭발 이펙트는 아무리 오래 살아도 0.5초 살아있습니다.
    {
        Invoke("DestroySelf", 0.5f);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMove playerMove = other.gameObject.GetComponent<PlayerMove>();
            Vector2 direction = (player.position - transform.position).normalized;   // 벡터 계산
            direction.y = 1.0f;                                                      // 벡터 계산
            Vector2 knockback = direction * knockbackRange;                          // 계산된 벡터에 넉백 거리 곱함  
            playerMove.ApplyKnockback(knockback);

            Debug.Log("플레이어에게 데미지 줌, explosionEffects");
            if (revolverHealthSystem != null)   // 데미지 적용
            {
                // 플레이어의 리볼버 체력 시스템에서 데미지를 적용
                revolverHealthSystem.TakeDamage(damage); // 나중에 float으로 전환해야돼 RevolverHealthSystem에서 데미지 적용할때
            }
            DestroySelf();
        }
    }
    void DestroySelf()
    {
        Destroy(gameObject); // 현재 오브젝트 삭제
    }
    public void GetValue(float knRange, int dmg)    // GetValue를 재사용했습니다. 코드 가독성이 망했습니다. 하지만 돌아가죠?
    {                                               // 상속은 아니지만 이 오브젝트를 생성한 오브젝트로부터 받아와야 할 값을 받아오는 코드입니다.
        knockbackRange = knRange;                   // EnemyBomb.cs(터지기 전 폭탄을 다루는 코드)의 onDestroy() 를 참고해주십시오.
        damage = dmg;                               
    }
}
