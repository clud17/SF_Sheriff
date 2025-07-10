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

    public int gunmode; // 총 (0:히트스캔, 1:투사체)
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

    public abstract void Fire(GameObject player, Transform tip);
    public abstract IEnumerator DelayedShoot(GameObject player, Transform tip);
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