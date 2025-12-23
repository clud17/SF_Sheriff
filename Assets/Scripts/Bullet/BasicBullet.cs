using Mono.Cecil;
using UnityEngine;

public class BasicBullet : BulletBase
{

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Hitscan(RaycastHit2D hitinfo) // 히트스캔 메소드
    {
        base.Hitscan(hitinfo);
        if (base.getIsHit())
        {
            /* 여기에 특수 기능 추가
            
            */
            base.setIsHit(false); // 초기화
        }
        // 여기에 궤적 추가
        // 궤적 종료지점 설정 : 맞은 대상이 있으면 맞은 대상의 좌표, 맞은 대상이 없으면 
        //Vector2 endPoint = hitinfo.collider != null ? hitinfo.point : 
    }
    public override void Projectile() // 투사체 메소드
    {
        base.Projectile();

        if (rb != null)
        {
            rb.linearVelocity = moveDirection * 110f;  // velocity를 사용하여 총알 이동
        }
        else
        {
            Debug.Log("BasicBullet의 rb가 null");
        }
        Debug.Log(bulletName + "이 " + lifetime + "초 후에 삭제");
        Destroy(gameObject, lifetime);
    }
}
