using UnityEngine;

public class UIDontDestroy : MonoBehaviour
{
    public static UIDontDestroy Instance; // 싱글톤 인스턴스

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 이미 인스턴스가 있다면 자신을 파괴
            return;
        }

        Instance = this; // 이 인스턴스를 유일한 인스턴스로 설정
        DontDestroyOnLoad(gameObject); // 씬 변경 시 파괴되지 않도록 설정
    }
}