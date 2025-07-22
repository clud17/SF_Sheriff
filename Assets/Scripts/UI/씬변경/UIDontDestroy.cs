using UnityEngine;

public class UIDontDestroy : MonoBehaviour
{
    public static UIDontDestroy Instance; // �̱��� �ν��Ͻ�

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �ִٸ� �ڽ��� �ı�
            return;
        }

        Instance = this; // �� �ν��Ͻ��� ������ �ν��Ͻ��� ����
        DontDestroyOnLoad(gameObject); // �� ���� �� �ı����� �ʵ��� ����
    }
}