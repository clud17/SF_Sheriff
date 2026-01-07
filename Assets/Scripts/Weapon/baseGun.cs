using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Data
{
    public float fireRate;       // 발사 간격
    public float nextFireTime; // 총알 발사 딜레이
    public bool isCharging;
    public bool isReloading; // 재장전 중인지 아닌지
    public float AmmoreloadTime; // 재장전시간
    //HP 관리 필드
    public int maxHP;// 최대 체력
    public int currentHP;    // 현재 체력 (피격 시 감소)
    public int currentAmmo;  // 현재 남은 탄약의 갯수

    public int gunmode; // 총 (0:히트스캔, 1:투사체)

    // 장전탄을 위한 플래그 추가
    public bool isReloadBullet;
}


public abstract class BaseGun : MonoBehaviour
{
    public Data gundata; // 총의 데이터 구조체
    public WeaponController WC; // WeaponController 참조 (이 변수를 사용하지 않을 경우 제거할 수 있습니다)

    public virtual void InitSetting()
    {
        gundata.isCharging = false;
        gundata.isReloading = false; // 재장전 중인지 아닌지
        gundata.maxHP = 6; // 최대 체력
        gundata.currentHP = gundata.maxHP; // 현재 체력을 최대 체력으로 초기화
        gundata.isReloadBullet = false; // 장전탄 적중 플래그 초기화
        // gundata.currentAmmo는 WeaponController에서 RevolverHealthSystem을 통해 초기화/관리됩니다.
        // g250731: BaseGun 내부에서 currentAmmo를 초기화하는 대신, WeaponController가 관리하도록 합니다.
        // gundata.currentAmmo = gundata.currentHP; 
    }

    public abstract void Fire(GameObject player, Transform tip);
    public abstract IEnumerator DelayedShoot(GameObject player, Transform tip);
    public virtual IEnumerator ReloadAmmo()
    {
        gundata.isReloading = true;
        Debug.Log("장전중..");

        if (gundata.isReloadBullet) // ReloadBullet이 적군에게 맞았으면
        {
            yield return new WaitForSeconds(gundata.AmmoreloadTime * 0.25f);
            gundata.isReloadBullet = false; // ReloadBullet.cs에서 플래그 변경한 것을 초기화
            Debug.Log("특수 장전 완료");
        }
        else // 일반 재장전
        {
            yield return new WaitForSeconds(gundata.AmmoreloadTime);
            Debug.Log("장전 완료");
        }


        // g250731: 현재 탄약 수(currentAmmo)는 RevolverHealthSystem에서 가져온 값으로
        // WeaponController에서 업데이트하도록 합니다. BaseGun은 재장전 '동작'만 수행합니다.
        // gundata.currentAmmo = gundata.currentHP; // 현재 체력을 현재 탄창으로 

        gundata.isReloading = false;
    }

}