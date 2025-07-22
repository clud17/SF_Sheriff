using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PlayerHealthManager : MonoBehaviour
{
    public BaseGun guncontrol; // guncontrol을 사용하기 위해 추가
    public Image[] bullets; // 총알 UI 연결

    public Sprite fullBulletSprite;    // 체력 있는 총알 이미지
    public Sprite emptyBulletSprite;  // 체력 없는 총알 이미지

    public GameObject gameOverPanel;  // 게임 오버 UI

    void Start()
    {
        guncontrol = GetComponent<BaseGun>();       // ? 총마다 다르게(히트스캔 or 투사체) inspector 창에 player를 불러오려면 어떻게 해야할까?이게 맞나?
        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i].sprite = fullBulletSprite;
        }
        //UpdateBullets();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
    public void TakeDamage(int damage)
    {
        guncontrol.gundata.currentHP -= 1;
        Debug.Log($"현재 체력 : {guncontrol.gundata.currentHP}");
        Debug.Log($"현재 총알 : {guncontrol.gundata.currentAmmo}");
        UpdateBullets();
        CheckDeath(); // ✅ 여기서 체력 검사
    }

    // public으로 변경
    public void UpdateBullets()
    {
        int bulletsToShow = Mathf.CeilToInt((float)Mathf.Clamp(guncontrol.gundata.currentHP, 0, guncontrol.gundata.maxHP));

        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i].sprite = (i < bulletsToShow) ? fullBulletSprite : emptyBulletSprite;
        }
    }

    // public으로 변경
    public void CheckDeath()
    {
        // ❗ 변경된 부분: 체력이 0보다 "작을 때" 죽음 처리
        if (guncontrol.gundata.currentHP < 1)
        {
            Debug.Log("Player Died");

            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);

            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}