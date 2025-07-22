using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager ����� ���� �߰�

public class PlayerDontDestroy : MonoBehaviour
{
    public static PlayerDontDestroy Instance; // �̱��� �ν��Ͻ�

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �ִٸ� �ڽ��� �ı�
            return;
        }

        Instance = this; // �� �ν��Ͻ��� ������ �ν��Ͻ��� ����
        DontDestroyOnLoad(gameObject); // �� ���� �� �ı����� �ʵ��� ����

        // �� �ε� �̺�Ʈ ���� (�� ������ �÷��̾� ��ġ �缳���� ����)
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // ������Ʈ�� �ı��� �� �̺�Ʈ ���� ����
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}