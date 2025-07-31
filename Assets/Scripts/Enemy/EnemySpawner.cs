using UnityEngine;

[System.Serializable]
public class SpawnInfo
{
    public GameObject enemyPrefab;
    public Vector2 spawnPosition;
}

public class EnemySpawner : MonoBehaviour
{
    public SpawnInfo[] spawnInfos;

    void Start()
    {
        foreach (SpawnInfo info in spawnInfos)
        {
            if (info.enemyPrefab != null)
                Instantiate(info.enemyPrefab, info.spawnPosition, Quaternion.identity);
        }
    }
}