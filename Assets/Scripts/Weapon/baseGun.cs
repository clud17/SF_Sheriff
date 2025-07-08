using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Data
{
    public float fireRate;      // 발사 간격
    public float nextFireTime; // 총알 발사 딜레이
    public bool isCharging;
    public bool isReloading; // 재장전 중인지 아닌지
    public float AmmoreloadTime; // 재장전시간
    //HP 관리 필드
    public int maxHP;// 최대 체력
    public int currentHP;    // 현재 체력 (피격 시 감소)
    public int currentAmmo;  // 현재 남은 탄약의 갯수

}


public abstract class baseGun : MonoBehaviour
{
    public Data gundata; // 총의 데이터 구조체
    public WeaponController WC;
    public virtual void InitSetting()
    {
        gundata.isCharging = false;
        gundata.isReloading = false; // 재장전 중인지 아닌지
        gundata.maxHP = 6; // 최대 체력
        gundata.currentHP = gundata.maxHP; // 현재 체력을 최대 체력으로 초기화
        gundata.currentAmmo = gundata.currentHP; // 현재 탄약을 현재 체력으로 초기화

    }

    public virtual void Fire(GameObject player, Transform tip)
    { // 총알 발사 메소드
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - tip.position).normalized;
        // 회전 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        //Debug.Log(currentAmmo + "이므로 " + myBullets[currentAmmo].bulletName + "이 발사됩니다.");
        Debug.Log(gundata.currentAmmo);
        BulletBase now = WC.myBullets[gundata.currentAmmo--];
        Debug.Log(now);
        now.gameObject.SetActive(true);                         // 오브젝트 다시 활성화
        now.transform.position = tip.position;            // 발사 위치
        now.SetDirection(direction);                            // 방향 설정
        now.transform.rotation = rotation;                      // 회전 설정
        now.Fire();
    }
    public virtual IEnumerator DelayedShoot(GameObject player, Transform tip)     
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
        now.Fire();

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
    public virtual IEnumerator ReloadAmmo()
    {
        gundata.isReloading = true;
        Debug.Log("장전중..");
        yield return new WaitForSeconds(gundata.AmmoreloadTime);
        Debug.Log("장전 완료");

        gundata.currentAmmo = gundata.currentHP; // 현재 체력을 현재 탄창으로

        gundata.isReloading = false;
    }
}
