using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal2DScript : MonoBehaviour
{
    public string targetSceneName;
    public Vector2 spawnPositionInTargetScene; // ���� ������ �÷��̾ ��Ÿ�� ��ġ

    // ���������� ������ ��Ż�� ������ �����Ͽ� ���� ������ ������ �� �ֵ��� �մϴ�.
    // (���� ���������� �����մϴ�)
    public static Portal2DScript LastEnteredPortal;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LastEnteredPortal = this; // ���� ��Ż ������ ����

            // �� �ε� ���� �̺�Ʈ ������ �߰�
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(targetSceneName); // �� �ε�
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �� ��Ż�� �ε��� ���� �´��� Ȯ���ϰ�, �ش� ���� �ε�Ǿ��� ���� ó��
        if (scene.name == targetSceneName)
        {
            // �÷��̾� ������Ʈ�� ã���ϴ�. DontDestroyOnLoadManager�� ���������
            // PlayerDontDestroy.Instance.transform �� ������ �� �ֽ��ϴ�.
            // �ƴϸ� GameObject.FindWithTag("Player"); �� �۵��մϴ�.
            GameObject player = GameObject.FindWithTag("Player");

            if (player != null)
            {
                // �÷��̾��� ��ġ�� ��Ż�� ������ ���� ���� ���� ��ġ�� �����մϴ�.
                player.transform.position = LastEnteredPortal.spawnPositionInTargetScene;
            }

            // �̺�Ʈ �����ʴ� �� �� ��� �� �ݵ�� �����ؾ� �ߺ� ȣ���� �����մϴ�.
            SceneManager.sceneLoaded -= OnSceneLoaded;
            LastEnteredPortal = null; // ��� �� �ʱ�ȭ
        }
    }
}