using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
public class HitScanGun : baseGun
{
    //float maxDistance = 10f; // 히트스캔 최대 사거리 설정

    //public BulletBase
    public override void InitSetting()
    {
        base.InitSetting(); // 부모 클래스의 InitSetting 호출
        gundata.fireRate = 0.25f; // 발사 간격 설정
        gundata.nextFireTime = 0f; // 초기화
        gundata.AmmoreloadTime = 1.5f; // 재장전 시간 설정

    }
    public override void Fire(GameObject player, Transform tip)
    {
        // 총알 발사 메소드
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - tip.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(tip.position, direction, 10f, LayerMask.GetMask("Enemy"));    // 히트스캔을 위한 레이캐스트 총구(tip), 방향, 최대거리(10f), layerMask(enemy) 설정

        BulletBase now = WC.myBullets[gundata.currentAmmo--];
        now.Hitscan(hit);
    }
    public override IEnumerator DelayedShoot(GameObject player, Transform tip)
    {
        if (gundata.isCharging || gundata.isReloading) yield break; // 이미 실행 중이면 무시
        gundata.isCharging = true;
        yield return new WaitForSeconds(0.3f); // 0.3초 대기  //차지샷 딜레이시간
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - tip.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(tip.position, direction, 10f, LayerMask.GetMask("Enemy"));    // 히트스캔을 위한 레이캐스트 총구(tip), 방향, 최대거리(10f), layerMask(enemy) 설정

        //총알 발사
        BulletBase now = WC.myBullets[0];
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
}