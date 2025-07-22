// CinemachineManagerDDOL.cs
using UnityEngine;

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
    }
}