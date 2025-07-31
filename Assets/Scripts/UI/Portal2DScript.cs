using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal2DScript : MonoBehaviour
{
    // 이동할 씬 이름과 이동 좌표
    public string targetSceneName;
    public Vector2 spawnPositionInTargetScene;

    // 포털 위치 정보만 static으로 저장 (오브젝트 참조는 저장하지 않음) 가나다
    private static Vector2? pendingSpawnPosition = null;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 이동할 위치만 미리 저장
            pendingSpawnPosition = spawnPositionInTargetScene;

            // 씬 로드 이벤트 연결
            SceneManager.sceneLoaded += OnSceneLoaded;

            // 씬 이동
            SceneManager.LoadScene(targetSceneName);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 이동 완료 후, 위치 설정
        if (pendingSpawnPosition.HasValue)
        {
            GameObject player = GameObject.FindWithTag("Player");

            if (player != null)
            {
                player.transform.position = pendingSpawnPosition.Value;
            }
            else
            {
                Debug.LogWarning("Player not found in the loaded scene.");
            }

            // 위치 정보 초기화
            pendingSpawnPosition = null;
        }

        // 이벤트 등록 해제 (중복 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
