using UnityEngine;

public class ShortEnemyAI : EnemyAI
{
    
    protected override void Init()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어의 Transform을 찾음
        HP = GetComponent<Health>();
        HP.maxHealth = 25f; //최대 체력 설정

        detectionRange = 11f;
        attackRange = 3.0f;
        moveSpeed = 4f;
        isPlayerDetected = false;

    }
    
}