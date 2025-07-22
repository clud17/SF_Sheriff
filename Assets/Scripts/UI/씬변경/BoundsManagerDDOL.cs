// BoundsManagerDDOL.cs (����)
using UnityEngine;

public class BoundsManagerDDOL : MonoBehaviour
{
    public static BoundsManagerDDOL Instance;

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