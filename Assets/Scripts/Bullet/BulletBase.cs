using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
    // 총알 베이스 추상 클래스
    // 이 파일은 모든 총알이 기본으로 가지고 있어야 하는 기능들에 대해 정의하는 추상 클래스이다.
    // 모든 총알은 이 BulletBase를 상속하며, 탄환만의 특수한 기능은 개별 Bullet 코드에서 관리한다.
    public BulletData bulletData;

    //총알이라면 무조건 가지고 있어야 하는 필드들
    public Vector2 moveDirection;//방향
    protected Rigidbody2D rb;

    protected string bulletName;
    float damage;
    bool healsOnHit;
    protected float lifetime;
    Color tracerColor;
    float tracerWidth;
    GameObject tracerPrefab;
    Sprite icon;

    public void InitFromData()
    {
        bulletName = bulletData.bulletName;
        damage = bulletData.damage;
        healsOnHit = bulletData.healsOnHit;
        lifetime = bulletData.lifetime;
        tracerColor = bulletData.tracerColor;
        tracerWidth = bulletData.tracerWidth;
        tracerPrefab = bulletData.tracerPrefab;
        icon = bulletData.icon;
    }

    public int gunmode; // 총 모드 (0: hitscan, 1: projectile)

    private Coroutine returnRoutine;
    public void SetDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;
    }
    public virtual void Hitscan(RaycastHit2D hitinfo)
    {
        InitFromData();
        //Debug.Log($"{bulletName}이 발사됨");
        gunmode = 0;
        if (hitinfo.collider != null && hitinfo.collider.CompareTag("Enemy"))  // ray가 맞았을 때, 적 tag가 enemy면 데미지 줌
        {
            Debug.Log("적에게 데미지를 줌(히트스캔)");
        }

    }
    public virtual void Projectile()
    {
        InitFromData();
        // 총알 발사시 공통적으로 우선실행되어야 하는 코드들. 자식 클래스에서 base.Fire()로 쓸 수 있음
        gunmode = 1;
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)  // 투사체 모드일 때 충돌 감지
    {
        //OnTriggerEnter2D는 일반적인 오버라이딩 규칙을 따르지 않는다.
        //자식 클래스에서 OnTriggerEnter2D를 구현하면 그 코드만 따르고, 아예 구현하지 않으면 밑 코드를 따른다.
        if(gunmode == 1) // 투사체 모드일 때만 충돌 감지
        {
            if (collision.gameObject.tag == "Enemy")  // 총알과 적의 충돌 감지
            {
                Debug.Log("적에게 데미지를 줌(투사체)");
                if (bulletName != "관통탄")
                {
                    Destroy(gameObject);
                }
            }
        }
        
    }
}