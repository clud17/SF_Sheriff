using UnityEngine;

public class LongEnemyAI : EnemyAI
{
    
    protected override void Init()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어의 Transform을 찾음
        HP = GetComponent<Health>();
        HP.maxHealth = 15f; //최대 체력 설정

        detectionRange = 11f;
        attackRange = 7.0f;
        moveSpeed = 3.0f;
    }
    
}