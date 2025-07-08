using Mono.Cecil;
using UnityEngine;

public class PassBullet : BulletBase
{
    /*
    특별한 기능이 있는 총알 만드는 법
    1. BulletBase 클래스의 하위 클래스가 될 수 있게, 특이 총알의 클래스를 생성한다.
    아무 코드도 적지 않을 시 기본 탄의 효과를 그대로 받아온다.
    2. 특이 총알의 프리팹을 만든 후, 클래스 파일을 inspector에 넣는다.
    총알 사용하는 법은 GunController의 주석에서 설명.
    */
    void Awake()
    {
        bulletName = "관통탄";
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
