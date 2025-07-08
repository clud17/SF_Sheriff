using Mono.Cecil;
using UnityEngine;

public class ChargeBullet : BulletBase
{
    void Awake()
    {
        bulletName = "우클릭탄";
        speed      = 110f;
        lifetime   = 2f;
    }
    public override void Hitscan() // 히트스캔 메소드
    {
        base.Hitscan();


    }
    public override void Projectile() // 투사체 메소드
    {
        base.Projectile();
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * speed;  // velocity를 사용하여 총알 이동
        }
    }
}
