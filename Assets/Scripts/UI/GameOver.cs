using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverUI; // 게임 오버 UI 패널
    private RevolverHealthSystem revolverHealthSystem; // RevolverHealthSystem 참조 변수

    void Awake()
    {
        // 게임 시작 시 RevolverHealthSystem 컴포넌트를 미리 찾아서 저장
        revolverHealthSystem = FindObjectOfType<RevolverHealthSystem>();
        if (revolverHealthSystem == null)
        {
            Debug.LogError("씬에 RevolverHealthSystem 컴포넌트가 존재하지 않습니다. 게임 오버 기능을 사용할 수 없습니다.");
        }
    }

    void Update()
    {
        // 미리 찾아둔 참조 변수가 null이 아니고, currentHealth가 0 이하일 때
        if (revolverHealthSystem != null && revolverHealthSystem.currentHealth <= 0)
        {
            // 게임 오버 UI 활성화
            gameOverUI.SetActive(true);

            // 게임 시간 정지
            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        // 게임 재시작
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}