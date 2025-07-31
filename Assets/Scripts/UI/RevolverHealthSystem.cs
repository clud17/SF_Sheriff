using UnityEngine;
using UnityEngine.UI; // UI 요소를 사용하기 위해 필요

public class RevolverHealthSystem : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 60; // 최대 체력 (총알 슬롯 개수 * 10으로 생각, 예: 60)
    private int currentHealth; // 현재 플레이어의 체력

    [Header("총알 설정")]
    public int maxBullets = 6; // 리볼버의 최대 총알 슬롯 개수 (총구 개수)
    private int currentUsableBullets; // 현재 총을 쏠 수 있는 (막히지 않은) 총알 슬롯 개수
    private bool[] bulletBlockedStatus; // 각 총알 슬롯이 막혔는지 여부를 저장할 배열 (true = 막힘)

    [Header("UI 연결")]
    // 인스펙터에서 순서대로 총알 슬롯 UI Image를 할당해야 합니다 (예: bulletSlot1, bulletSlot2...)
    public Image[] bulletSlots;
    public Sprite filledBulletSprite;   // 총알이 채워진 스프라이트 (시작 시)
    public Sprite blockedBulletSprite;  // 총구가 막혔을 때의 스프라이트 (데미지 시)

    // 초기화
    void Start()
    {
        currentHealth = maxHealth; // 체력을 최대로 설정
        currentUsableBullets = maxBullets; // 시작 시 모든 총구가 사용 가능 (막히지 않음)

        // 각 총알 슬롯의 막힘 상태를 초기화 (모두 '막히지 않음' 상태로 시작)
        bulletBlockedStatus = new bool[maxBullets];
        for (int i = 0; i < maxBullets; i++)
        {
            bulletBlockedStatus[i] = false; // 기본적으로 막히지 않은 상태
        }

        UpdateUI(); // 시작 시 UI를 초기 상태(모두 채워진 총구)로 업데이트
    }

    // 데미지를 받는 함수
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // 현재 체력에서 데미지 감소

        // 체력이 0 미만으로 내려가지 않도록 보정
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        // --- 핵심 로직: 체력에 따라 총알 슬롯 막기 (검은색 스프라이트로 변경) ---
        // 총알 1개당 필요한 체력(데미지) 단위 계산 (예: 60 / 6 = 10 데미지당 총알 1개)
        int healthPerBullet = maxHealth / maxBullets;

        // 현재 체력으로 총을 쏠 수 있는 최대 총알 슬롯 개수 계산
        int newUsableBulletCount = Mathf.CeilToInt((float)currentHealth / healthPerBullet);

        // 현재 쏠 수 있는 총알 개수가 줄어들었다면 (즉, 데미지를 받아 총구가 막혀야 한다면)
        if (newUsableBulletCount < currentUsableBullets)
        {
            // 줄어든 개수만큼 총알 슬롯을 뒤에서부터 막음 (bulletBlockedStatus를 true로 설정)
            for (int i = currentUsableBullets - 1; i >= newUsableBulletCount; i--)
            {
                bulletBlockedStatus[i] = true; // 해당 슬롯을 막힘 상태로 설정
            }
            currentUsableBullets = newUsableBulletCount; // 쏠 수 있는 총알 개수 업데이트

            Debug.Log($"데미지를 받아 총구 슬롯이 {currentUsableBullets}개 남았습니다. (총 {maxBullets - currentUsableBullets}개 막힘)");

            // 총구 슬롯이 모두 막혔을 때 게임 오버 처리
            if (currentUsableBullets <= 0)
            {
                Debug.Log("게임 오버! 모든 총구 슬롯이 막혔습니다.");
                // 여기에 게임 오버 처리 로직 추가 (예: 게임 재시작, 특정 UI 활성화 등)
            }
        }

        UpdateUI(); // UI 업데이트 함수 호출 (막힌 총구를 검은색 스프라이트로 변경)
    }

    // 체력을 회복하는 함수
    public void Heal(int amount)
    {
        currentHealth += amount; // 현재 체력에 회복량 추가

        // 체력이 최대 체력을 넘지 않도록 보정
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // --- 핵심 로직: 체력 회복에 따라 막힌 총구 다시 활성화 ---
        // 총알 1개당 필요한 체력(데미지) 단위 계산
        int healthPerBullet = maxHealth / maxBullets;

        // 현재 체력으로 총을 쏠 수 있는 최대 총알 슬롯 개수 계산
        int newUsableBulletCount = Mathf.CeilToInt((float)currentHealth / healthPerBullet);

        // 현재 쏠 수 있는 총알 개수가 늘어났다면 (즉, 체력을 회복하여 총구를 활성화해야 한다면)
        if (newUsableBulletCount > currentUsableBullets)
        {
            // 늘어난 개수만큼 총알 슬롯을 앞에서부터 활성화 (bulletBlockedStatus를 false로 설정)
            // 즉, 막혔던 총구를 다시 채워진 상태로 되돌림
            for (int i = currentUsableBullets; i < newUsableBulletCount; i++) // 주의: currentUsableBullets부터 시작
            {
                if (i < maxBullets) // 배열 범위를 벗어나지 않도록 확인
                {
                    bulletBlockedStatus[i] = false; // 해당 슬롯을 막히지 않은 상태로 설정
                }
            }
            currentUsableBullets = newUsableBulletCount; // 쏠 수 있는 총알 개수 업데이트

            Debug.Log($"체력 회복으로 총구 슬롯이 {currentUsableBullets}개 남았습니다.");
        }

        UpdateUI(); // UI 업데이트 함수 호출 (막힌 총구가 다시 채워진 스프라이트로 변경)
    }


    // 총을 쏘는 함수 (현재는 데미지/회복 테스트에 집중)
    public void Shoot()
    {
        Debug.Log("총을 쏘는 기능은 아직 구현되지 않았습니다. 데미지/회복 테스트에 집중합니다.");
        // 나중에 총을 쏘는 로직이 여기에 추가됩니다.
    }

    // UI를 업데이트하는 함수 (총구 슬롯 스프라이트 변경)
    void UpdateUI()
    {
        for (int i = 0; i < maxBullets; i++)
        {
            if (bulletBlockedStatus[i]) // 해당 총구 슬롯이 막혔다면
            {
                bulletSlots[i].sprite = blockedBulletSprite; // 막힌 스프라이트 (검은색 동그라미) 표시
            }
            else // 슬롯이 막히지 않았다면 (사용 가능한 총구라면)
            {
                bulletSlots[i].sprite = filledBulletSprite; // 채워진 스프라이트 표시
            }
        }
    }

    // 외부에서 현재 남은 총구 개수를 가져가는 함수 (필요시 사용)
    public int GetCurrentUsableBullets()
    {
        return currentUsableBullets;
    }
}