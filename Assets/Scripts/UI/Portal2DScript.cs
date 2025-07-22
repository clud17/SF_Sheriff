using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal2DScript : MonoBehaviour
{
    public string targetSceneName;
    public Vector2 spawnPositionInTargetScene; // 다음 씬에서 플레이어가 나타날 위치

    // 마지막으로 진입한 포탈의 정보를 저장하여 다음 씬에서 참조할 수 있도록 합니다.
    // (선택 사항이지만 유용합니다)
    public static Portal2DScript LastEnteredPortal;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LastEnteredPortal = this; // 현재 포탈 정보를 저장

            // 씬 로드 전에 이벤트 리스너 추가
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(targetSceneName); // 씬 로드
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 이 포탈이 로드한 씬이 맞는지 확인하고, 해당 씬이 로드되었을 때만 처리
        if (scene.name == targetSceneName)
        {
            // 플레이어 오브젝트를 찾습니다. DontDestroyOnLoadManager를 사용했으면
            // PlayerDontDestroy.Instance.transform 에 접근할 수 있습니다.
            // 아니면 GameObject.FindWithTag("Player"); 도 작동합니다.
            GameObject player = GameObject.FindWithTag("Player");

            if (player != null)
            {
                // 플레이어의 위치를 포탈에 지정된 다음 씬의 스폰 위치로 설정합니다.
                player.transform.position = LastEnteredPortal.spawnPositionInTargetScene;
            }

            // 이벤트 리스너는 한 번 사용 후 반드시 제거해야 중복 호출을 방지합니다.
            SceneManager.sceneLoaded -= OnSceneLoaded;
            LastEnteredPortal = null; // 사용 후 초기화
        }
    }
}