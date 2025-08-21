using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager 사용을 위해 추가가

public class PlayerDontDestroy : MonoBehaviour
{
    public static PlayerDontDestroy Instance; // 싱글톤 인스턴스

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 이미 인스턴스가 있다면 자신을 파괴
            return;
        }

        Instance = this; // 이 인스턴스를 유일한 인스턴스로 설정
        DontDestroyOnLoad(gameObject); // 씬 변경 시 파괴되지 않도록 설정

        // 씬 로드 이벤트 구독 (새 씬에서 플레이어 위치 재설정을 위함)
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 오브젝트가 파괴될 때 이벤트 구독 해지
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}