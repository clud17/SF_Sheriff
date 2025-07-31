using UnityEngine;
using UnityEngine.EventSystems;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject[] buttons; // resume, setting 등

    void Start()
    {
        pauseMenu.SetActive(false);
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
            EventSystem.current.SetSelectedGameObject(buttons[0]); // resume 버튼 선택
    }
}