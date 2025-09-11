using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using static UnityEngine.Rendering.DebugUI.Table;

public class HitScanGun : BaseGun
{
    //float maxDistance = 10f; // 히트스캔 최대 사거리 설정

    //public BulletBase
    public override void InitSetting()
    {
        base.InitSetting(); // 부모 클래스의 InitSetting 호출
        gundata.fireRate = 0.5f; // 발사 간격 설정
        gundata.nextFireTime = 0f; // 초기화
        gundata.AmmoreloadTime = 1.5f; // 재장전 시간 설정
    }

    public override void Fire(GameObject player, Transform tip)
    {
        // 총알이 0개이거나 인덱스가 유효하지 않으면 함수를 즉시 종료
        if (gundata.currentAmmo <= 0)
        {
            Debug.LogWarning("총알이 없어 발사할 수 없습니다.");
            return;
        }

        // 인덱스를 '남은 총알 수 - 1'로 계산하여 배열 범위를 벗어나지 않도록 함
        int bulletIndex = gundata.currentAmmo - 1;
        if (bulletIndex < 0 || bulletIndex >= WC.myBulletObj.Length)
        {
            Debug.LogError($"총알 인덱스 오류! 현재 인덱스: {bulletIndex}, 배열 크기: {WC.myBulletObj.Length}");
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - tip.position).normalized;
        RaycastHit2D[] hit = Physics2D.RaycastAll(tip.position, direction, 30f);        // RaycastAll을 사용하여 여러 충돌체를 감지

        BulletBase now = WC.myBulletObj[gundata.currentAmmo].GetComponent<BulletBase>();
        int i=0;
        // 이제 안전하게 배열에 접근 가능
        for (i = 0; i < hit.Length; i++)
        {
            RaycastHit2D hitinfo = hit[i];
            // 여기에 폭탄에 총을 쐈을 때, 레이저가 막히는 문제 발생 해결해야함~
            if (hitinfo.collider.gameObject.CompareTag("EnemyAttack")) continue;
            // if (hitinfo.c ~~ 해ㅑ서 거를 거 더 추가하면 됩니다.(확장성 망함)
            // 관통탄을 쏜다고 가정하면 위에 있는 if문들을 싸그리 무시하고 now.hitscan으로 가게 하면 됩니다.

            now.Hitscan(hit[i]); // 충돌 처리 메소드 호출
            
            break; // 관통탄이면 break가 아니라 continue 되게 해야 함.
        }
        
        DrawTracer(now, tip.position, direction, hit.Length==i?30f:hit[i].distance);

        Debug.DrawRay(tip.position, direction * 100f, Color.red, 1f);
    }

    public override IEnumerator DelayedShoot(GameObject player, Transform tip)
    {
        if (gundata.currentAmmo <= 0)
        {
            Debug.LogWarning("총알이 없어 발사할 수 없습니다.");
            yield break;
        }
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - tip.position).normalized;
        RaycastHit2D[] hit = Physics2D.RaycastAll(tip.position, direction, 30f);        // RaycastAll을 사용하여 여러 충돌체를 감지

        BulletBase now = WC.myBulletObj[0].GetComponent<BulletBase>();
        int i=0;
        // 이제 안전하게 배열에 접근 가능
        for (i = 0; i < hit.Length; i++)
        {
            RaycastHit2D hitinfo = hit[i];
            now.Hitscan(hit[i]); // 충돌 처리 메소드 호출
            break; // 관통탄이면 break가 아니라 continue 되게 해야 함.
        }
        DrawTracer(now, tip.position, direction, hit.Length==0?30f:hit[i].distance);

        Debug.DrawRay(tip.position, direction * 100f, Color.blue, 1f);
    }

    public override IEnumerator ReloadAmmo()
    {
        return base.ReloadAmmo();
    }

    public void DrawTracer(BulletBase bullet, Vector2 start, Vector2 direction, float distance)
    {
        Vector2 end = start + direction.normalized * distance;
        LineRenderer tracer = Instantiate(bullet.bulletData.tracerPrefab).GetComponent<LineRenderer>();
        tracer.startColor = bullet.bulletData.tracerColor;
        tracer.endColor = bullet.bulletData.tracerColor;
        tracer.startWidth = bullet.bulletData.tracerWidth;
        tracer.endWidth = bullet.bulletData.tracerWidth;
        tracer.positionCount = 2;
        tracer.SetPosition(0, start);
        tracer.SetPosition(1, end);
        StartCoroutine(FadeOutLine(tracer, 0.2f));
    }

    private IEnumerator FadeOutLine(LineRenderer line, float duration)
    {
        float elapsed = 0f;
        Color startColor = line.startColor;
        Color endColor = line.endColor;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            line.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            line.endColor = new Color(endColor.r, endColor.g, endColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(line.gameObject);
    }
}