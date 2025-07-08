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
    public override void Fire() // 부모 Fire를 상속받은 Fire 메소드
    {
        base.Fire();
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * speed;  // velocity를 사용하여 총알 이동
        }
    }
}
