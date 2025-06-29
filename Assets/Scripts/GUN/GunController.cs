using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public GameObject bulletPrefab;     // 총알 프리팹
    public GameObject player;          // 플레이어 오브젝트
    public Transform firePoint;         // 총알 발사 위치
    public float fireRate;      // 발사 간격
    private float nextFireTime; // 총알 발사 딜레이
    bool isCharging;


    public int maxHP;        // 최대 체력 (= 장전 가능한 총알 한도)
    public int currentHP;    // 현재 체력 (피격 시 감소)   // 여기 나중에 수정 필요할듯? hardcoding되어있음. 다른 곳에서 받아와야 하나?
    public int currentAmmo;  // 현재 탄약
    public bool isReloading; // 재장전 중인지 아닌지
    public float AmmoreloadTime; // 재장전시간
    
    private bool isRKeyHeld; // R키가 눌려진 상태인지 확인하는 변수(왜냐하면 R키를 누르는 동안 총알 발사 막기위해)
    private float rKeyHoldTime; // R키를 누른 시간
    private float firstHealTime; // 회복 시작 시간 (R키를 누르고 2초가 지나면 회복 시작)
    private float extraHealInterval;  // 회복 인터벌 (R키 꾹 누르고 있을 때 회복 주기)
    private float extraHealTimer;  // 회복 타이머
    private bool firstHealDone; // 첫 번째 회복이 완료되었는지 확인하는 플래그
    private bool isHealing; // 회복 중인지 확인하는 플래그


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
        {
            if (currentAmmo <= 0)
            {
                Debug.Log("need to reload");  // 탄약이 없을 때
            }
            else
            {
                Fire();

                Debug.Log($"{currentAmmo}");
                currentAmmo--; // 발사 시 탄약 감소
                nextFireTime = Time.time + fireRate;  // 발사 주기를 관리하기 위해

            }
        }
        if (Input.GetMouseButtonDown(1) && !isRKeyHeld)
        {
            if (currentAmmo <= 0)
            {
                Debug.Log("need to reload");  // 탄약이 없을 때
            }
            else
            {
                StartCoroutine(DelayedShoot());
                currentAmmo = 0;
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
        yield return new WaitForSeconds(0.2f); // 0.2초 대기
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - firePoint.position).normalized;
        float playerScale = transform.localScale.x;  // 캐릭터의 크기를 동적으로 받음

        // 회전 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);
        bullet.GetComponent<Bullet>().SetDirection(direction);

        // 현재 캐릭터의 rigidbody2D를 가져옴
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(-direction * playerScale * 0.5f, ForceMode2D.Impulse);
        }
        isCharging = false; // 다시 발사 가능해짐
    }

    void Fire()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - firePoint.position).normalized;

        // 회전 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);
        bullet.GetComponent<Bullet>().SetDirection(direction);
    }
    void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;

        Debug.Log($"[HP 회복] {currentHP} / {maxHP}");
        // UI 갱신 필요시 여기서 호출
    }

}


