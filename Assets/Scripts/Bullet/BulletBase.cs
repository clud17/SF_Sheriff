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
    protected RevolverHealthSystem H_System; // 체력 시스템 참조

    protected string bulletName;
    float damage;
    bool healsOnHit;
    protected float lifetime;
    Color tracerColor;
    float tracerWidth;
    GameObject tracerPrefab;
    Sprite icon;

    // 총알이 적중했는지 여부 체크 // Basic, Pass, heal Bullet..등에 사용
    // 왜 사용했냐? => 총알 당 특수 기능을 구현하기 위해선 bullet에서 적중 여부를 알아야 하기 때문
    private bool ishit = false; 
    protected bool getIsHit()
    {
        return ishit;
    }
    protected void setIsHit(bool value)
    {
        ishit = value;
    }

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
        H_System = FindObjectOfType<RevolverHealthSystem>(); // 체력 시스템 참조
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
        gunmode = 0;
        if (hitinfo.collider == null) return;

        switch (hitinfo.collider.tag)
        {
            case "Enemy":
                setIsHit(true);
                hitinfo.collider.GetComponent<Health>().ApplyDamage(damage); // 적 체력에 데미지 적용
                Debug.Log("적에게 데미지를 줌(히트스캔)");
                
                break;
            case "Switch":
                Debug.Log("오브젝트 가동됨");
                hitinfo.collider.GetComponent<Switch>().TurnSwitch();
                break;
            default:
                Debug.Log("BulletBase에서의 태그 미지정");
                break;
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