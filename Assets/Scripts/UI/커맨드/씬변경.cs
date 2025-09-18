using UnityEngine;
using UnityEngine.SceneManagement; // For scene management
using System.Collections.Generic; // For List

public class 씬변경 : MonoBehaviour
{
    // Debug key combination: T
    public KeyCode mainDebugKey = KeyCode.T;
    // Debug key combination: Y
    public KeyCode subDebugKey = KeyCode.Y;

    // List of scene names to load based on number keys
    public List<string> sceneNames = new List<string>();

    // Reference to the Player GameObject's Transform
    // This needs to be assigned in the Inspector if PlayerDontDestroy isn't always active,
    // or found dynamically. For simplicity, we'll find it dynamically.
    private Transform playerTransform;

    void Awake()
    {
        // ⭐ 중요한 변경: 씬 로드 이벤트를 구독합니다.
        // 이 스크립트가 DontDestroyOnLoad 되어 있어야 씬 전환 후에도 이 이벤트를 받을 수 있습니다.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // 오브젝트가 파괴될 때 이벤트 구독 해지
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        // Check if both main and sub debug keys are pressed
        if (Input.GetKey(mainDebugKey) && Input.GetKey(subDebugKey))
        {
            // Check number keys from 1 to 9
            for (int i = 1; i <= 9; i++)
            {
                KeyCode numberKey = KeyCode.Alpha0 + i;

                if (Input.GetKeyDown(numberKey)) // Check if number key is pressed down once
                {
                    int sceneIndex = i - 1; // List index is 0-based

                    // Check if the scene name exists in the list
                    if (sceneIndex >= 0 && sceneIndex < sceneNames.Count)
                    {
                        string targetScene = sceneNames[sceneIndex];
                        Debug.Log($"Debug: Loading scene: {targetScene} (Key combo: {mainDebugKey} + {subDebugKey} + {i})");
                        SceneManager.LoadScene(targetScene);
                    }
                    else
                    {
                        Debug.LogWarning($"Debug: Scene for key {i} not defined in sceneNames list.");
                    }
                }
            }
        }
    }

    // ⭐ 씬이 로드된 후에 호출될 메서드입니다.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 플레이어 트랜스폼을 찾습니다. "Player" 태그를 사용합니다.
        // 매번 씬이 로드될 때마다 다시 찾아야 합니다.
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            // 0918 이용진 - 밑에거 다 주석처리했는데 해결됐습니다.

            //playerTransform = player.transform;
            // 플레이어의 위치를 (0,0,0)으로 설정합니다.
            //playerTransform.position = Vector3.zero;
            //Debug.Log($"Player moved to (0,0,0) in scene: {scene.name} via 씬변경 script.");
        }
        else
        {
            Debug.LogWarning("Player object with tag 'Player' not found in the new scene. Cannot move to (0,0,0).");
        }
    }
}