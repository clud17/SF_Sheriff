// CinemachineManagerDDOL.cs
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CinemachineManagerDDOL : MonoBehaviour
{
    public static CinemachineManagerDDOL Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 살아남기

    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateCameraConfiner();
    }
    void UpdateCameraConfiner()
    {
        var boundObj = GameObject.Find("CameraBound");

        if (boundObj != null)
        {
            PolygonCollider2D collider = boundObj.GetComponent<PolygonCollider2D>();
            CinemachineConfiner2D confiner = GetComponent<CinemachineConfiner2D>();
            confiner.BoundingShape2D = collider;
            confiner.InvalidateBoundingShapeCache();
        }
    }


}