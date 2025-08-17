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
        /// 이 메소드에서 하는 역할
        /// 1. 실제 Raycast를 발사해 결과를 hit에 저장한다.
        /// 2. 궤적을 그린다.

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - tip.position).normalized;
        RaycastHit2D[] hit = Physics2D.RaycastAll(tip.position, direction, 30f);        // RaycastAll을 사용하여 여러 충돌체를 감지
        // 히트스캔을 위한 레이캐스트 총구(tip), 방향, 최대거리(30f), layerMask(enemy) 설정

        BulletBase now = WC.myBulletObj[gundata.currentAmmo].GetComponent<BulletBase>();
        int i=0;
        for (i = 0; i < hit.Length; i++)
        {
            RaycastHit2D hitinfo = hit[i];
            now.Hitscan(hit[i]); // 충돌 처리 메소드 호출
            break; // 관통탄이면 break가 아니라 continue 되게 해야 함.
        }
        DrawTracer(now, tip.position, direction, hit.Length==0?30f:hit[i].distance);
        Debug.DrawRay(tip.position, direction * 100f, Color.red, 1f);
        

        gundata.currentAmmo--;
    }
    public override IEnumerator DelayedShoot(GameObject player, Transform tip)
    {
        if (gundata.isCharging || gundata.isReloading) yield break; // 이미 실행 중이면 무시
        gundata.isCharging = true;
        yield return new WaitForSeconds(0.3f); // 0.3초 대기  //차지샷 딜레이시간
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - tip.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(tip.position, direction, 30f);    // 히트스캔을 위한 레이캐스트 총구(tip), 방향, 최대거리(10f), layerMask(enemy) 설정


        BulletBase now = WC.myBulletObj[gundata.currentAmmo].GetComponent<BulletBase>();
        //총알 발사
        if (hit.collider != null)
        {
            DrawTracer(now, tip.position, direction, hit.distance);
        }
        else
        {
            DrawTracer(now, tip.position, direction, 30f); // 맞은 대상이 없으면 최대 거리로 궤적 그리기
        }

        now.Hitscan(hit);

        gundata.currentAmmo = 0; // 차지샷을 사용했으므로 현재 탄약을 0으로 설정

        //플레이어 넉백
        Vector2 knockbackDir = new Vector2(-direction.x, -direction.y).normalized;     // 넉백 방향 설정 (x축은 왼쪽/오른쪽, y축은 위쪽으로 설정)

        // 현재 캐릭터의 rigidbody2D를 가져옴
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 knockbackForce = knockbackDir * 8f;                         // 넉백 벡터 설정(방향+힘)

            player.GetComponent<PlayerMove>().ApplyKnockback(knockbackForce);   // 플레이어 이동 스크립트에 넉백 적용 함수 호출
        }
        gundata.isCharging = false; // 다시 발사 가능해짐
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

        // Destroy(tracer.gameObject, 0.15f);
        StartCoroutine(FadeOutLine(tracer, 0.2f));  // 0.2초간 페이드아웃
    }
    private IEnumerator FadeOutLine(LineRenderer line, float duration)
    {
        float elapsed = 0f;  // 지금까지 경과한 시간

        Color startColor = line.startColor;
        Color endColor = line.endColor;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);  // 알파값을 1에서 0으로 선형 보간(점점 줄임)
            line.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            line.endColor   = new Color(endColor.r, endColor.g, endColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(line.gameObject);  // 완전히 사라진 후 제거
    }

}