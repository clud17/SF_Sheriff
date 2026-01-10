using System;
using System.Collections.Generic;
using UnityEngine;

public class BossHitbox : MonoBehaviour
{
    [SerializeField] private Collider2D hitboxCol;

    private int damage;
    private float knockbackRange;
    private bool isActive = false;

    // 대쉬 공격이 플레이어를 맞췄을 때, 실행할 액션 
    public Action OnHitPlayer;
    // 한번 켠 동안 1회만 히트 처리를 위한 플래그
    private bool hasHitThisActive = false;

    private void Awake()
    {
        if (hitboxCol == null) hitboxCol = GetComponent<Collider2D>();
        hitboxCol.isTrigger = true;
        hitboxCol.enabled = false;
    }

    public void GetValue(float knRange, int dmg)    
    {                                               
        this.knockbackRange = knRange;
        this.damage = dmg;
    }
    public void EnableHitbox()
    {
        isActive = true;
        hasHitThisActive = false;
        hitboxCol.enabled = true;
    }
    public void DisableHitbox()
    {
        isActive = false;
        hitboxCol.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive || hasHitThisActive) return;
        if (!collision.CompareTag("Player")) return;

        hasHitThisActive = true; // 이번 활성화 동안은 1회만 히트 처리
        // 플레이어를 맞췄을 때 실행할 액션 호출
        OnHitPlayer?.Invoke();

        if (collision.tag == "Player")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            RevolverHealthSystem revolverHealthSystem = player.GetComponent<RevolverHealthSystem>();

            revolverHealthSystem.TakeDamage(damage); // damage

            Vector2 direction = (player.transform.position - transform.position).normalized;   // 벡터 계산
            direction.y = 1.0f;                                                      // 벡터 계산
            Vector2 knockback = direction * knockbackRange;                          // knockback
            player.GetComponent<PlayerMove>().ApplyKnockback(knockback);
        }
    }
}