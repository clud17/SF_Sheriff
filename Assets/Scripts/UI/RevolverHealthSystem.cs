using UnityEngine;
using UnityEngine.UI;

public class RevolverHealthSystem : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth; // 최대 체력 (총알 슬롯 개수 * 10으로 생각, 예: 60)
    public int currentHealth; // 현재 플레이어의 체력 (public으로 수정하여 외부 스크립트에서 접근 가능하도록 함)

    [Header("총알 설정")]
    public int maxBullets; // 리볼버의 최대 총알 슬롯 개수 (총구 개수)
    private int currentUsableBullets; // 데미지로 인해 막히지 않은 총알 슬롯 개수
    private bool[] bulletBlockedStatus; // 각 총알 슬롯이 막혔는지 여부
    private bool[] bulletFiredStatus;    // 각 총알 슬롯이 발사되었는지 여부

    // 기존 주석 유지: [Header("UI 연결")]
    [Header("UI 연결")]
    public Image[] bulletSlots;
    public Sprite filledBulletSprite;
    public Sprite blockedBulletSprite;
    public Sprite emptyBulletSprite;

    // 초기화: Awake에서 변수와 배열을 모두 초기화합니다.
    void Awake()
    {
        maxHealth = 6;
        maxBullets = 6;
        currentHealth = maxHealth;
        currentUsableBullets = maxBullets;

        bulletBlockedStatus = new bool[maxBullets];
        bulletFiredStatus = new bool[maxBullets];

        for (int i = 0; i < maxBullets; i++)
        {
            bulletBlockedStatus[i] = false;
            bulletFiredStatus[i] = false;
        }

        UpdateUI();
    }

    public void Shoot()
    {
        Debug.Log("총을 쏘는 기능은 아직 구현되지 않았습니다.");
    }

    // 데미지를 받는 함수
    public void TakeDamage(int damage)
    {
        Debug.Log("take damage 호출======");
        currentHealth -= damage; // 현재 체력에서 데미지 감소
        
        // 체력이 0 미만으로 내려가지 않도록 보정
            if (currentHealth < 0)
            {
                currentHealth = 0;
            }

        // --- 핵심 로직: 체력에 따라 총알 슬롯 막기 (검은색 스프라이트로 변경) ---
        // // 총알 1개당 필요한 체력(데미지) 단위 계산 (예: 60 / 6 = 10 데미지당 총알 1개)
        // int healthPerBullet = maxHealth / maxBullets;

        // 현재 체력으로 총을 쏠 수 있는 최대 총알 슬롯 개수 계산
        int newUsableBulletCount = currentHealth;

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

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        // int healthPerBullet = maxHealth / maxBullets;
        int newUsableBulletCount = currentHealth;
        
        if (newUsableBulletCount > currentUsableBullets)
        {
            for (int i = currentUsableBullets; i < newUsableBulletCount; i++)
            {
                if (i < maxBullets)
                {
                    bulletBlockedStatus[i] = false;
                }
            }
            currentUsableBullets = newUsableBulletCount;
            Debug.Log($"체력 회복으로 총구 슬롯이 {currentUsableBullets}개 남았습니다.");
        }
        UpdateUI();
    }

    public void MarkBulletAsFired()
    {
        for (int i = maxBullets - 1; i >= 0; i--)
        {
            if (!bulletBlockedStatus[i] && !bulletFiredStatus[i])
            {
                bulletFiredStatus[i] = true;
                UpdateUI();
                return;
            }
        }
        Debug.LogWarning("모든 총알이 발사되었거나 막혀있습니다.");
    }

    public void ResetFiredBullets()
    {
        for (int i = 0; i < maxBullets; i++)
        {
            if (!bulletBlockedStatus[i])
            {
                bulletFiredStatus[i] = false;
            }
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < maxBullets; i++)
        {
            if (bulletBlockedStatus[i])
            {
                bulletSlots[i].sprite = blockedBulletSprite;
            }
            else if (bulletFiredStatus[i])
            {
                bulletSlots[i].sprite = emptyBulletSprite;
            }
            else
            {
                bulletSlots[i].sprite = filledBulletSprite;
            }
        }
    }

    public int GetCurrentUsableBullets()
    {
        return currentUsableBullets;
    }

    public int GetCurrentAvailableBulletsForFiring()
    {
        if (bulletBlockedStatus == null || bulletFiredStatus == null)
        {
            Debug.LogError("총알 상태 배열이 초기화되지 않았습니다. RevolverHealthSystem의 Awake() 함수를 확인해주세요.");
            return 0;
        }

        int count = 0;
        for (int i = 0; i < maxBullets; i++)
        {
            if (!bulletBlockedStatus[i] && !bulletFiredStatus[i])
            {
                count++;
            }
        }
        return count;
    }
}