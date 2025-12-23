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
    public bool[] isBulletUnlocked;

    //250731 체력♡총알
    public RevolverHealthSystem revolverHealthSystem; // 체력 스크립트와 연결


    void Start()
    {
        isBulletUnlocked = new bool[bulletDatas.Length];
        isBulletUnlocked[1] = isBulletUnlocked[3] = true;
        
        /// Start() 메소드
        /// my___에 지금 탄환이 어떤 것이 있는지 저장하는 용도.
        /// 탄환 순서나 종류 바뀌면 이 코드 다시 실행되어야 함.

        setBullet(new int[] {1,2,1,4,5,1});

        // aud = GetComponent<AudioSource>(); // AudioManager.Instance를 사용하므로 필요 없습니다.

        //250731 RevolverHealthSystem 연결
        // RevolverHealthSystem이 할당되었는지 확인하고, 없으면 씬에서 찾습니다.
        if (revolverHealthSystem == null)
        {
            revolverHealthSystem = FindObjectOfType<RevolverHealthSystem>();
            if (revolverHealthSystem == null)
            {
                Debug.LogError("RevolverHealthSystem이 WeaponController에 할당되지 않았거나 씬에서 찾을 수 없습니다. 수동으로 할당해주세요.");
                enabled = false; // 스크립트 비활성화
                return;
            }
        }
        // 시작 시 현재 총알 수를 RevolverHealthSystem의 현재 발사 가능한 총알 수로 설정
        currentGun.gundata.currentAmmo = revolverHealthSystem.GetCurrentAvailableBulletsForFiring();
    }


    void Update()
    {
        if (currentGun.gundata.isReloading) return; // 재장전 중이면 아무것도 하지 않음

        //RevolverHealthSystem 연결
        // RevolverHealthSystem에서 현재 발사 가능한 총알 수를 가져와 업데이트
        if (revolverHealthSystem == null) return; // 안전장치
        int availableBulletsToFire = revolverHealthSystem.GetCurrentAvailableBulletsForFiring();

        // 만약 currentGun의 현재 총알 수가 RevolverHealthSystem에서 알려주는 발사 가능한 총알 수보다 많다면,
        // (예: 데미지를 받아 총구 수가 줄었는데 아직 발사하지 않은 경우)
        // currentGun의 총알 수를 맞춰줍니다.
        if (currentGun.gundata.currentAmmo > availableBulletsToFire)
        {
            currentGun.gundata.currentAmmo = availableBulletsToFire;
        }
        //RevolverHealthSystem 연결을 위한 추가 코드 끝

        if (Input.GetMouseButtonDown(0) && Time.time >= currentGun.gundata.nextFireTime)
        { //좌클릭 코드 구현
            // 체력연결용
            if (currentGun.gundata.currentAmmo <= 0) // 총알이 0개 이하일 경우 바로 재장전 필요 메시지를 띄움
            {
                Debug.Log("need to reload");
                return;
            }

            currentGun.Fire(player, tip); // 발사
            AudioManager.Instance.PlaySound(AudioManager.Instance.ShootingSound); // 총기 발사 사운드 재생

            //총알 수 감소는 WeaponController에서만 처리하도록 수정
            revolverHealthSystem.MarkBulletAsFired(); // RevolverHealthSystem에 총알 발사 알림 (UI 업데이트)
            currentGun.gundata.currentAmmo--; // 현재 총알 수 감소

            Debug.Log($"{currentGun.gundata.currentAmmo}");
            currentGun.gundata.nextFireTime = Time.time + currentGun.gundata.fireRate;  // 발사 주기를 관리하기 위해
        }

        
        // 기존의 ReloadAmmo()와 ResetFiredBullets()를 분리하지 않고
        // 하나의 코루틴에서 순차적으로 처리하여 -1 오류를 방지
        if (Input.GetMouseButtonDown(1))
        { // 우클릭 코드 구현
            StartCoroutine(currentGun.DelayedShoot(player, tip)); // 발사
            currentGun.gundata.currentAmmo = 0;
            revolverHealthSystem.AllFiredBullets();
            if (!currentGun.gundata.isReloading)
            {
                StartCoroutine(ReloadAndSyncAmmo());
            }
        }
        if (Input.GetKeyUp(KeyCode.R))
        { // R 키 재장전
            if (!currentGun.gundata.isReloading)
            {
                StartCoroutine(ReloadAndSyncAmmo());
            }
        }
        
    }

    public void setBullet(int[] bullets)
    {
        //기존 WeaponController.cs의 start()코드 중 총알을 설정하는 코드를 갖고왔습니다.
        //총알 구조가 바뀔때마다 호출해야하므로 start()에서 쓸 수 없기에 따로 메소드로 빼놓습니다.

        // 기본 탄환의 종류를 지정하는 배열. 
        // 0: x, 1: 기본, 2: 관통, 3: x, 4: 치유, 5: 약점
        // 나중에 휴식을 할 때, 바꿀수 있어야함
        for (int i = 6; i >= 1; i--) // 기본 탄환 6개.
        {
            myBulletObj[7 - i] = bulletPrefabs[bullets[i - 1]];
            myBulletObj[7 - i].GetComponent<BulletBase>().InitFromData();
            myBulletData[7 - i] = bulletDatas[bullets[i - 1]];
        }
        // 위 코드를 실행한 이후 myBullets 배열은 { ,F,E,D,C,B,A}가 된다
        myBulletObj[0] = bulletPrefabs[0]; // bulletPrefabs[0] 
        myBulletData[0] = bulletDatas[bullets[0]];
        currentGun.InitSetting(); // 총 초기화 설정
    }


    // --- 추가된 재장전 동기화 코루틴 시작 ---
    // 재장전이 끝난 후에 총알 수를 UI와 동기화하여 -1 오류를 방지함
    private IEnumerator ReloadAndSyncAmmo()
    {
        if (currentGun.gundata.isReloading) yield break;

        AudioManager.Instance.PlaySound(AudioManager.Instance.ReloadSound);

        // BaseGun의 재장전 코루틴이 완료될 때까지 대기
        yield return StartCoroutine(currentGun.ReloadAmmo());

        // 재장전이 완료된 후, RevolverHealthSystem의 발사된 총알 상태를 리셋
        revolverHealthSystem.ResetFiredBullets();

        // UI와 동기화된 총알 수를 가져와 currentAmmo에 정확히 설정
        int newAvailableBullets = revolverHealthSystem.GetCurrentAvailableBulletsForFiring();
        currentGun.gundata.currentAmmo = newAvailableBullets;

        Debug.Log($"재장전 완료! 현재 발사 가능 총알: {currentGun.gundata.currentAmmo}");
    }
    // --- 추가된 재장전 동기화 코루틴 끝 ---

    //250731 체력
    // 외부에서 데미지를 받을 때 호출될 메서드
    public void OnTakeDamage(int damage)
    {
        revolverHealthSystem.TakeDamage(damage); // RevolverHealthSystem에 데미지 처리 위임

        // 데미지를 받아 총구 슬롯이 막혔을 경우, 현재 총알 수가 막힌 슬롯보다 많으면 줄어듬.
        int newAvailableBullets = revolverHealthSystem.GetCurrentAvailableBulletsForFiring();
        if (currentGun.gundata.currentAmmo > newAvailableBullets)
        {
            currentGun.gundata.currentAmmo = newAvailableBullets;
        }

        Debug.Log($"총구 {revolverHealthSystem.maxBullets - revolverHealthSystem.GetCurrentUsableBullets()}개 파괴됨. 남은 총구: {revolverHealthSystem.GetCurrentUsableBullets()}");
    }
}