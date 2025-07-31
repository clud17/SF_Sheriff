using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public BaseGun currentGun; // 현재 장착된 총
    public GameObject player; // 플레이어 오브젝트
    public Transform tip; // 총알 발사 위치

    public GameObject[] myBulletObj = new GameObject[7];

    public BulletData[] myBulletData = new BulletData[7];
    //250717 추가된 코드

    public GameObject[] bulletPrefabs;
    public BulletData[] bulletDatas;

    // 사운드
    public AudioClip ShootingSound;  // 총기 발사 사운드
    public AudioClip ChargeSound;  // 충전 사운드
    public AudioClip ReloadSound;
    AudioSource aud;

    void Start()
    {
        /// Start() 메소드
        /// my___에 지금 탄환이 어떤 것이 있는지 저장하는 용도.
        /// 탄환 순서나 종류 바뀌면 이 코드 다시 실행되어야 함.
        /// 따라서 총알 바꾸는 기능 도입되면 여기 말고 baseGun.총알변경(배열[7]) 쓰는 게 좋아보임

        // 기본 탄환의 종류를 지정하는 배열. 0: 기본, 1: charge(바꿔야함 우클릭에만 적용되게), 2: 관통 등등. 나중에 휴식을 할 때, 바꿀수 있어야함
        int[] startBullet = { 1, 3, 1, 3, 3, 1 }; // ABCDEF.      //(수정 필 : 나중에 업데이트 할 수 있게 gundata에 넣는다??)
        for (int i = 6; i >= 1; i--) // 기본 탄환 6개.
        {
            myBulletObj[7 - i] = bulletPrefabs[startBullet[i - 1]];
            myBulletObj[7 - i].GetComponent<BulletBase>().InitFromData();
            myBulletData[7 - i] = bulletDatas[startBullet[i - 1]];
        }
        // 위 코드를 실행한 이후 myBullets 배열은 { ,F,E,D,C,B,A}가 된다
        myBulletObj[0] = bulletPrefabs[0]; // bulletPrefabs[0] 
        myBulletData[0] = bulletDatas[startBullet[0]];
        currentGun.InitSetting(); // 총 초기화 설정
        
        this.aud = GetComponent<AudioSource>();
    }


    void Update()
    {
        if (currentGun.gundata.isReloading) return; // 재장전 중이면 아무것도 하지 않음

        if (Input.GetMouseButtonDown(0) && Time.time >= currentGun.gundata.nextFireTime)
        { //좌클릭 코드 구현
            if (currentGun.gundata.currentAmmo == 0) Debug.Log("need to reload"); //현재탄환 큐가 비어있으면
            else // 탄약이 있으면
            {
                currentGun.Fire(player, tip); // 발사
                this.aud.PlayOneShot(ShootingSound); // 총기 발사 사운드 재생
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
                this.aud.PlayOneShot(ChargeSound); // 총기 발사 사운드 재생
                Debug.Log($"{currentGun.gundata.currentAmmo}");
                this.aud.PlayOneShot(ReloadSound); // 재장전 사운드 재생
                StartCoroutine(currentGun.ReloadAmmo());  // 우클릭시 남은 총알 관계없이 재장전
            }
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            // 짧게 눌렀을 경우 → 총알 재장전
            if (!currentGun.gundata.isReloading)
            {
                StartCoroutine(currentGun.ReloadAmmo());
                this.aud.PlayOneShot(ReloadSound); // 재장전 사운드 재생
            }
        }
    }
}
