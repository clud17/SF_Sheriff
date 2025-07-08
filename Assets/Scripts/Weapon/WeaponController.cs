using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public baseGun currentGun; // 현재 장착된 총
    public GameObject player; // 플레이어 오브젝트
    public Transform tip; // 총알 발사 위치

    public BulletBase[] myBullets = new BulletBase[7];
    public GameObject[] bulletPrefabs;
    public GameObject ChargeBulletObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 기본 탄환의 종류를 지정하는 배열. 0: 기본, 1: charge(바꿔야함 우클릭에만 적용되게), 2: 관통 등등. 나중에 휴식을 할 때, 바꿀수 있어야함
        int[] startBullet = { 1, 3, 1, 3, 3, 1 }; // ABCDEF.
        for (int i = 6; i >= 1; i--) // 기본 탄환 6개.
        {
            //총알 프리팹을 미리 만들어 두고, 클래스만 인큐해 넣는다.
            GameObject bulletObj = Instantiate(bulletPrefabs[startBullet[i - 1]]);
            BulletBase bullet = bulletObj.GetComponent<BulletBase>();
            myBullets[7 - i] = bullet;
            //myBullets[i] = bullet;
        }
        // 위 코드를 실행한 이후 myBullets 배열은 { ,F,E,D,C,B,A}가 된다
        ChargeBulletObj = Instantiate(bulletPrefabs[0]); // bulletPrefabs[0] 

        currentGun.InitSetting(); // 총 초기화 설정
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGun.gundata.isReloading) return; // 재장전 중이면 아무것도 하지 않음

        if (Input.GetMouseButtonDown(0) && Time.time >= currentGun.gundata.nextFireTime)
        { //좌클릭 코드 구현
            if (currentGun.gundata.currentAmmo == 0) Debug.Log("need to reload"); //현재탄환 큐가 비어있으면
            else // 탄약이 있으면
            {
                currentGun.Fire(player, tip); // 발사
                Debug.Log($"{currentGun.gundata.currentAmmo}");
                currentGun.gundata.nextFireTime = Time.time + currentGun.gundata.fireRate;  // 발사 주기를 관리하기 위해

            }
        }
        if (Input.GetMouseButtonDown(1))
        { // 우클릭 코드 구현
            if (currentGun.gundata.currentAmmo == 0) Debug.Log("need to reload");
            else
            {
                StartCoroutine(currentGun.DelayedShoot(player, tip));
                Debug.Log($"{currentGun.gundata.currentAmmo}");

                StartCoroutine(currentGun.ReloadAmmo());  // 우클릭시 남은 총알 관계없이 재장전
            }
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            // 짧게 눌렀을 경우 → 총알 재장전
            if (!currentGun.gundata.isReloading)
            {
                StartCoroutine(currentGun.ReloadAmmo());
            }       
        }
    }
}
