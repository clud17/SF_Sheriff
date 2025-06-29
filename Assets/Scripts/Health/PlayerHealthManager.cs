using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PlayerHealthManager : MonoBehaviour
{
    public GunController guncontrol; // guncontrol을 사용하기 위해 추가

    public Image[] bullets; // 총알 UI 연결

    public Sprite fullBulletSprite;   // 체력 있는 총알 이미지
    public Sprite emptyBulletSprite;  // 체력 없는 총알 이미지

    public GameObject gameOverPanel;  // 게임 오버 UI

    void Start()
    {
        guncontrol.maxHP = 6; // 최대 체력 설정(일단 hardcording)
        guncontrol.currentHP = guncontrol.maxHP; //현재 체력을 최대 체력으로 초기화
        UpdateBullets();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        guncontrol.currentHP -= 1;
        Debug.Log($"현재 체력 : {guncontrol.currentHP}");
        Debug.Log($"현재 총알 : {guncontrol.currentAmmo}");
        UpdateBullets();
        CheckDeath(); // ✅ 여기서 체력 검사
    }

    private void UpdateBullets()
    {
        int bulletsToShow = Mathf.CeilToInt((float)Mathf.Clamp(guncontrol.currentHP, 0, guncontrol.maxHP));

        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i].sprite = (i < bulletsToShow) ? fullBulletSprite : emptyBulletSprite;
        }
    }

    private void CheckDeath()
    {
        // ❗ 변경된 부분: 체력이 0보다 "작을 때" 죽음 처리
        if (guncontrol.currentHP < 0)
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
