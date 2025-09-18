using Mono.Cecil.Cil;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class RevolverHealthSystem : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth; // 최대 체력 (총알 슬롯 개수)
    public int currentHealth; // 현재 플레이어의 체력 (public으로 수정하여 외부 스크립트에서 접근 가능하도록 함)

    [Header("총알 설정")]
    public int maxBullets; // 리볼버의 최대 총알 슬롯 개수 (총구 개수)
    private int currentUsableBullets; // 데미지로 인해 막히지 않은 총알 슬롯 개수
    private bool[] bulletBlockedStatus; // 각 총알 슬롯이 막혔는지 여부
    private bool[] bulletFiredStatus;    // 각 총알 슬롯이 발사되었는지 여부

    // 게임 오버 패널을 연결할 변수 추가
    [Header("게임 오버")]
    public GameObject gameOverPanel;
    public bool isInvincible; // 무적 상태 플래그

    // 기존 주석 유지: [Header("UI 연결")]
    [Header("UI 연결")]
    public Image[] bulletSlots;
    public Sprite filledBulletSprite;
    public Sprite blockedBulletSprite;
    public Sprite emptyBulletSprite;
    public SpriteRenderer playerSprite; // 플레이어 스프라이트 (무적 상태 시 반짝임 효과에 사용)

    public (string, Vector3) savePoint;

    // 초기화: Awake에서 변수와 배열을 모두 초기화합니다.
    void Awake()
    {
        savePoint = ("Room0", new Vector3(-5, -5, 0));
        playerSprite = GetComponent<SpriteRenderer>();
        isInvincible = false;

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

        // 게임 시작 시 패널이 비활성화되도록 설정
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
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
        if (isInvincible) return; // 무적 상태이면 데미지 무시
        Debug.Log("take damage 호출======");

        // 체력이 이미 0인 상태에서 데미지를 한 번 더 받으면 사망
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        currentHealth -= damage; // 현재 체력에서 데미지 감소

        isInvincible = true; // 무적 플래그 ON
        StartCoroutine(Invincibility()); // 무적 시간 시작

        // 체력이 0 미만으로 내려가지 않도록 보정
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("체력이 0이 되었습니다! 다음 공격에 사망합니다.");
        }
        // 체력에 따라 총알 슬롯 막기 (검은색 스프라이트로 변경)
        // 현재 체력으로 총을 쏠 수 있는 최대 총알 슬롯 개수 계산
        int newUsableBulletCount = currentHealth;

        if (newUsableBulletCount < currentUsableBullets)
        {
            for (int i = currentUsableBullets - 1; i >= newUsableBulletCount; i--)
            {
                bulletBlockedStatus[i] = true; // 해당 슬롯을 막힘 상태로 설정
            }
            currentUsableBullets = newUsableBulletCount; // 쏠 수 있는 총알 개수 업데이트

            Debug.Log($"데미지를 받아 총구 슬롯이 {currentUsableBullets}개 남았습니다. (총 {maxBullets - currentUsableBullets}개 막힘)");
        }
        UpdateUI(); // UI 업데이트 함수 호출 (막힌 총구를 검은색 스프라이트로 변경)
    }

    // 사망 처리 함수
    private void Die()
    {
        Debug.Log("게임 오버! 플레이어 사망!");

        /*

        // 게임 오버 패널을 활성화하고 게임 시간을 정지
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); // 게임 오버 UI 패널 활성화
        }
        Time.timeScale = 0f; // 게임 시간 정지 (게임 일시정지)


        */// 세이브포인트로 되돌아가는 거라서 윗 코드 주석처리하겠습니다.


        // 0914 추가 코드입니다. 플레이어 사망시 세이브포인트로 되돌아갑니다.
        ResetGame();
        SceneManager.LoadScene(savePoint.Item1);
        GetComponent<PlayerMove>().gameObject.transform.position = savePoint.Item2;
        

    }

    // 사망 후 게임 초기화
    public void ResetGame()
    {
        currentHealth = maxHealth;
        currentUsableBullets = maxBullets;

        // bulletBlockedStatus와 bulletFiredStatus 배열 초기화
        for (int i = 0; i < maxBullets; i++)
        {
            bulletBlockedStatus[i] = false;
            bulletFiredStatus[i] = false;
        }

        // 게임 오버 패널 비활성화
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // UI 업데이트
        UpdateUI();
        Debug.Log("게임 상태가 초기화되었습니다.");
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


    // 모든 총알을 다 발사했는지 확인하는 함수
    public bool AllFiredBullets()
    {
        // 사용 가능한 총알이 0개이면 true를 반환
        return GetCurrentAvailableBulletsForFiring() <= 0;
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

    // 피격시 무적 시간
    private IEnumerator Invincibility()
    {
        int countTime = 0;
        while (countTime < 10)
        {
            if (countTime % 2 == 0) playerSprite.color = new Color(1f, 1f, 1f, 0.3f);
            else playerSprite.color = new Color(1f, 1f, 1f, 0.7f);

            yield return new WaitForSeconds(0.2f);

            countTime++;
        }
        playerSprite.color = new Color(1f, 1f, 1f, 1f);

        isInvincible = false; // 무적 상태 해제
        
        yield return null;
    }
}