using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
public class DefaultGun : baseGun
{
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
        // 회전 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        //Debug.Log(currentAmmo + "이므로 " + myBullets[currentAmmo].bulletName + "이 발사됩니다.");
        Debug.Log(gundata.currentAmmo);
        BulletBase now = WC.myBullets[gundata.currentAmmo--];
        Debug.Log(now);
        now.gameObject.SetActive(true);
        now.transform.position = tip.position;            // 발사 위치
        now.SetDirection(direction);                            // 방향 설정
        now.transform.rotation = rotation;                      // 회전 설정
        now.Projectile();
    }
    public override IEnumerator DelayedShoot(GameObject player, Transform tip)
    {
        if (gundata.isCharging || gundata.isReloading) yield break; // 이미 실행 중이면 무시
        gundata.isCharging = true;
        yield return new WaitForSeconds(0.3f); // 0.3초 대기  //차지샷 딜레이시간
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - tip.position).normalized;

        // 회전 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        //총알 발사
        BulletBase now = WC.myBullets[gundata.currentAmmo];
        Debug.Log(now);
        now.gameObject.SetActive(true);                         // 오브젝트 다시 활성화
        now.transform.position = tip.position;            // 발사 위치
        now.SetDirection(direction);                            // 방향 설정
        now.transform.rotation = rotation;                      // 회전 설정
        now.Projectile();

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
