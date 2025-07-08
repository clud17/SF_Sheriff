using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunController : MonoBehaviour
{
    public GameObject player;          // 플레이어 오브젝트
    public Transform firePoint;         // 총알 발사 위치
    public float fireRate;      // 발사 간격
    private float nextFireTime; // 총알 발사 딜레이
    bool isCharging;
    //HP 관리 필드
    public int maxHP;// 최대 체력
    public int currentHP;    // 현재 체력 (피격 시 감소)
    public int currentAmmo;  // 현재 남은 탄약의 갯수
    //
    public bool isReloading; // 재장전 중인지 아닌지
    public float AmmoreloadTime; // 재장전시간
    private bool isRKeyHeld; // R키가 눌려진 상태인지 확인하는 변수(왜냐하면 R키를 누르는 동안 총알 발사 막기위해)
    private float rKeyHoldTime; // R키를 누른 시간
    private float firstHealTime; // 회복 시작 시간 (R키를 누르고 2초가 지나면 회복 시작)
    private float extraHealInterval;  // 회복 인터벌 (R키 꾹 누르고 있을 때 회복 주기)
    private float extraHealTimer;  // 회복 타이머
    private bool firstHealDone; // 첫 번째 회복이 완료되었는지 확인하는 플래그
    private bool isHealing; // 회복 중인지 확인하는 플래그

    public GameObject[] bulletPrefabs;
    private GameObject ChargeBulletObj;

    BulletBase[] myBullets = new BulletBase[7];
    void Start()
    {
        fireRate = 0.25f; // 총알 발사 간격 설정
        nextFireTime = 0f; // 초기화
        isCharging = false;

        currentAmmo = currentHP; // 현재 체력을 현재 탄창으로   

        isReloading = false;
        AmmoreloadTime = 0.5f;

        isRKeyHeld = false;
        rKeyHoldTime = 0f;
        firstHealTime = 2f;
        extraHealInterval = 0.5f;
        extraHealTimer = 0f;
        firstHealDone = false;
        isHealing = false;

        // 기본 탄환의 종류를 지정하는 배열. 0: 기본, 1: charge(바꿔야함 우클릭에만 적용되게), 2: 관통 등등. 나중에 휴식을 할 때, 바꿀수 있어야함
        int[] startBullet = { 1,3,1,3,3,1 }; // ABCDEF.
        for (int i = 6; i >= 1; i--) // 기본 탄환 6개.
        {
            //총알 프리팹을 미리 만들어 두고, 클래스만 인큐해 넣는다.
            GameObject bulletObj = Instantiate(bulletPrefabs[startBullet[i - 1]]);
            BulletBase bullet = bulletObj.GetComponent<BulletBase>();
            myBullets[7-i] = bullet;
           //myBullets[i] = bullet;
        }
        // 위 코드를 실행한 이후 myBullets 배열은 { ,F,E,D,C,B,A}가 된다.
        ChargeBulletObj = Instantiate(bulletPrefabs[0]);
    }


    void Update()
    {
        if (isReloading) return; // 재장전 중이면 아무것도 하지 않음

        if (Input.GetKey(KeyCode.R))
        {
            isRKeyHeld = true; // R키가 눌려진 상태
            rKeyHoldTime += Time.deltaTime;

            if (!isHealing && rKeyHoldTime >= firstHealTime)
            {
                Debug.Log("체력 충전 가능!");
                // 2초 도달 → 체력 회복 모드로 전환
                isHealing = true;
                firstHealDone = false;
                extraHealTimer = 0f;
            }

            if (isHealing)
            {
                if (!firstHealDone)
                {
                    Heal(1);
                    firstHealDone = true;
                }
                else
                {
                    extraHealTimer += Time.deltaTime;
                    if (extraHealTimer >= (firstHealTime - extraHealInterval))
                    {
                        Heal(1);
                        extraHealTimer = 0f;
                        if (extraHealInterval <= 1.0f) extraHealInterval += 0.5f;
                    }
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            isRKeyHeld = false; // R키에서 손을 뗌(총 발사 가능)
            // R키에서 손을 뗐을 때
            if (!isHealing && rKeyHoldTime < firstHealTime)
            {
                // 짧게 눌렀을 경우 → 총알 재장전
                if (!isReloading)
                {
                    StartCoroutine(ReloadAmmo());
                }
            }

            // 초기화
            rKeyHoldTime = 0f;
            isHealing = false;
            firstHealDone = false;
            extraHealTimer = 0f;
            extraHealInterval = 0.5f;
        }

        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime && !isRKeyHeld)
        { //좌클릭 코드 구현
            if (currentAmmo==0) //탄약이 없으면
            {
                Debug.Log("need to reload");  // 탄약이 없을 때
            }
            else // 탄약이 있으면
            {
                Fire(); // 발사
                Debug.Log($"{currentAmmo}");
                nextFireTime = Time.time + fireRate;  // 발사 주기를 관리하기 위해

            }
        }
        if (Input.GetMouseButtonDown(1) && !isRKeyHeld)
        {
            if (currentAmmo==0)
            {
                Debug.Log("need to reload");  // 탄약이 없을 때
            }
            else
            {
                StartCoroutine(DelayedShoot());
                
                // 여기에 특수한 능력이 있어야 하나?
                Debug.Log($"{currentAmmo}");
                StartCoroutine(ReloadAmmo());  // 우클릭시 남은 총알 관계없이 재장전
            }

        }
    }
    IEnumerator ReloadAmmo()
    {
        isReloading = true;
        Debug.Log("장전중..");
        yield return new WaitForSeconds(AmmoreloadTime);
        Debug.Log("장전 완료");

        currentAmmo = currentHP; // 현재 체력을 현재 탄창으로
        isReloading = false;
    }

    IEnumerator DelayedShoot()     //만약 땅이 있을때? 어떻게 구현해야하나? --> 10시 방향으로 addforce해서 구현?(중력 적용 해야함)
    {
        if (isCharging || isReloading) yield break; // 이미 실행 중이면 무시
        isCharging = true;
        yield return new WaitForSeconds(0.3f); // 0.3초 대기  //차지샷 딜레이시간
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - firePoint.position).normalized;

        // 회전 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // (수정 필)나중에 우클릭을 누르면 검은색 탄환(chargeBullet prefabs)이 나가게 해야함. 
        ChargeBulletObj.SetActive(true);                                                    //1. 오브젝트 다시 활성화
        ChargeBulletObj.GetComponent<BulletBase>().transform.position = firePoint.position; // 2. 발사 위치
        ChargeBulletObj.GetComponent<BulletBase>().SetDirection(direction);                 // 3. 방향 설정
        ChargeBulletObj.GetComponent<BulletBase>().transform.rotation = rotation;           //4.회전 설정
        ChargeBulletObj.GetComponent<BulletBase>().bulletDamage = currentAmmo * currentAmmo;
        Debug.Log(currentAmmo + "개를 발사합니다");
        ChargeBulletObj.GetComponent<BulletBase>().Fire();                                  //5. 발사
        currentAmmo = 0;
        //플레이어 넉백
        Vector2 knockbackDir = new Vector2(-direction.x, -direction.y).normalized;     // 넉백 방향 설정 (x축은 왼쪽/오른쪽, y축은 위쪽으로 설정)

        // 현재 캐릭터의 rigidbody2D를 가져옴
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 knockbackForce = knockbackDir * 8f;                         // 넉백 벡터 설정(방향+힘)

            player.GetComponent<PlayerMove>().ApplyKnockback(knockbackForce);   // 플레이어 이동 스크립트에 넉백 적용 함수 호출
        }
        isCharging = false; // 다시 발사 가능해짐
    }

    void Fire()
    { // 발사 코드
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - firePoint.position).normalized;
        // 회전 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        //Debug.Log(currentAmmo + "이므로 " + myBullets[currentAmmo].bulletName + "이 발사됩니다.");
        Debug.Log(currentAmmo);
        BulletBase now = myBullets[currentAmmo--];
        Debug.Log(now);
        now.gameObject.SetActive(true);                         // 오브젝트 다시 활성화
        now.transform.position = firePoint.position;            // 발사 위치
        now.SetDirection(direction);                            // 방향 설정
        now.transform.rotation = rotation;                      // 회전 설정
        now.Fire();                                             // 발사
    }
    void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;

        Debug.Log($"[HP 회복] {currentHP} / {maxHP}");
        // UI 갱신 필요시 여기서 호출
    }

}


