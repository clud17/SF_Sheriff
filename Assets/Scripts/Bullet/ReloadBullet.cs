using UnityEngine;

public class ReloadBullet : BulletBase
{
    public override void Hitscan(RaycastHit2D hitinfo) // 히트스캔 메소드
    {
        base.Hitscan(hitinfo);
        if (base.getIsHit() && B_Gun != null)
        {
            B_Gun.gundata.isReloadBullet = true; // BaseGun의 플래그를 true로 설정
            base.setIsHit(false); // 초기화
        }

    }
    public override void Projectile() // 투사체 메소드
    {
        base.Projectile();
        if (rb != null && B_Gun != null)
        {
            rb.linearVelocity = moveDirection * 110f;  // velocity를 사용하여 총알 이동
        }
    }
}
