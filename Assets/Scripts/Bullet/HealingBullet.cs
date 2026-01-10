using UnityEngine;

public class HealingBullet : BulletBase
{
    public override void Hitscan(RaycastHit2D hitinfo) // 히트스캔 메소드
    {
        base.Hitscan(hitinfo);
        if (base.getIsHit() && H_System != null)
        {
            //임시로 체력회복 구현
            H_System.Heal(1);  // 일단 한칸 증가하게

            //base.bulletData.healsOnHit 만큼 실드를 회복하면.
            base.setIsHit(false); // 초기화
        }
        // 여기에 궤적 추가하면 될듯???

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
