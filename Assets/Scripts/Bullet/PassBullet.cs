using Mono.Cecil;
using UnityEngine;

public class PassBullet : BulletBase
{
    public override void Hitscan(RaycastHit2D hitinfo) // 히트스캔 메소드
    {
        base.Hitscan(hitinfo);

        // 여기에 궤적 추가하면 될듯???

    }
    public override void Projectile() // 투사체 메소드
    {
        base.Projectile();
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * 110f;  // velocity를 사용하여 총알 이동
        }
    }
}
