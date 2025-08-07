using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentEnemyHealth;

    void Awake() => currentEnemyHealth = maxHealth;

    public void ApplyDamage(float damage)
    {
        //인코딩 깨지는 거 테스트
        Debug.Log("applydamage 실행됨");
        currentEnemyHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage!");

        if (currentEnemyHealth <= 0f) Die();
    }

    public void Heal(float amount)
    {
        currentEnemyHealth = Mathf.Min(currentEnemyHealth + amount, maxHealth);
        Debug.Log($"{gameObject.name} healed {amount} HP!");
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        Destroy(gameObject);
    }
}
