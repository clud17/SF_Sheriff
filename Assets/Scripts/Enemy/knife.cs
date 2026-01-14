using UnityEngine;

public class knife : MonoBehaviour
{
    private float knockbackRange;
    private int damage;
    void Awake()
    {
        knockbackRange = 5;
        damage = 1;
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name +"@@@");
        if (collision.tag == "Player")
        {
            Debug.Log("Player");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            RevolverHealthSystem revolverHealthSystem = player.GetComponent<RevolverHealthSystem>();

            revolverHealthSystem.TakeDamage(damage); // damage

            Vector2 direction = (player.transform.position - transform.position).normalized;   // 벡터 계산
            direction.y = 1.0f;                                                      // 벡터 계산
            Vector2 knockback = direction * knockbackRange;                          // knockback
            player.GetComponent<PlayerMove>().ApplyKnockback(knockback);
        }
    }

    public void GetValue(float knRange, int dmg)    
    {                                               
        knockbackRange = knRange;                   
        damage = dmg;
    }
}
