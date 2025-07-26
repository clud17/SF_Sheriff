using System.Runtime.CompilerServices;
using UnityEngine;

public class DroneEnemyAI : EnemyAI
{
    
    protected override void Init()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어의 Transform을 찾음
		HP = GetComponent<Health>();
        HP.maxHealth = 10f; //최대 체력 설정
        
        detectionRange = 11f;
        attackRange = 7.0f;
        moveSpeed = 4.0f;
    }
    protected override void MoveTowardsPlayer()   // 날아다니는 몹은 이동하는 방식이 다르므로 오버라이드
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime; // 이동
    }
}