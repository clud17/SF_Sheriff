using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameUIController : MonoBehaviour
{
    // RevolverHealthSystem을 참조하기 위한 변수
    [SerializeField] private RevolverHealthSystem revolverHealthSystem;

    // UI 패널들을 관리하기 위한 변수
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverPanel; 
    [SerializeField] private GameObject[] buttons;

    void Start()
    {
        // 게임 시작 시, 일시정지 패널을 비활성화
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        bool isActive = pauseMenu.activeSelf;
        pauseMenu.SetActive(!isActive);
        Time.timeScale = isActive ? 1f : 0f;

        if (!isActive)
        {
            if (buttons.Length > 0 && buttons[0] != null)
            {
                EventSystem.current.SetSelectedGameObject(buttons[0]); // resume 버튼 선택
            }
        }
    }

    // 게임 재시작 함수
    public void RestartGame()
    {
        Time.timeScale = 1f;
        if (revolverHealthSystem != null)
        {
            revolverHealthSystem.ResetGame();
        }
        else
        {
            Debug.LogError("RevolverHealthSystem이 할당되지 않았습니다.");
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 메인 화면으로 이동하는 함수
    public void GoToMainMenu()
    {
        // 게임 씬에서 DontDestroyOnLoad로 설정된 모든 오브젝트를 파괴
        string[] tagsToDestroy = { "Player", "MainCamera", "MainUI" };

        foreach (string tag in tagsToDestroy)
        {
            GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objectsToDestroy)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }

        // 게임 시간 재개
        Time.timeScale = 1f;

        // 메인 화면 씬으로 이동
        SceneManager.LoadScene("MainMenu");
    }


    // 게임 시작 함수 (메인 화면에서 사용)
    public void StartGame()
    {
        Time.timeScale = 1f;
        // "NewHappyLine" 씬으로 이동 (나중에 게임 시작 부분으로 씬 이름 바꾸기)
        SceneManager.LoadScene("NewHappyLine");
    }

    // 게임 종료 함수 (메인 화면이나 인게임에서 모두 사용 가능)
    public void ExitGame()
    {
        Application.Quit();
        // 유니티 에디터에서는 동작하지 않으며, 빌드된 게임에서만 작동합니다.
        // 에디터에서 테스트하려면 Debug.Log("게임 종료"); 를 대신 사용하세요.
    }
}