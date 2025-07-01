using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float lifetime;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    // public Bullet()
    // {

    // }

    public void SetDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;
    }

    void Start()
    {
        speed = 110f; // 총알 속도 설정
        lifetime = 2f; // 총알 유지 시간 설정

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * speed;  // velocity를 사용하여 총알 이동
        }
        Destroy(gameObject, lifetime); // 총알이 2초 후 자동 파괴
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")  // 총알과 적의 충돌 감지
        {
            Debug.Log("적에게 데미지를 줌");
            Destroy(this.gameObject);
        }
    }
}
/**/