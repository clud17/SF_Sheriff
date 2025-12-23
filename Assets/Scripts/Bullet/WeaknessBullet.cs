using UnityEngine;

public class WeaknessBullet : BulletBase
{
    public override void Hitscan(RaycastHit2D hitinfo) // 히트스캔 메소드
    {
        base.Hitscan(hitinfo);
        if (base.getIsHit())
        {
            hitinfo.collider.GetComponent<Health>().SetWeakness(true);
            base.setIsHit(false); // 초기화
        }

    }
    public override void Projectile() // 투사체 메소드
    {
        base.Projectile();
        if (rb != null && H_System != null)
        {
            rb.linearVelocity = moveDirection * 110f;  // velocity를 사용하여 총알 이동
        }
    }
}
