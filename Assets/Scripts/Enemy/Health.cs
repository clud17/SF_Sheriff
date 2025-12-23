using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentEnemyHealth;
    // 약점 상태 플래그
    public bool weakness = false;
    // 임시 데미지 저장 변수, 약점탄을 맞췄을 때, 데미지 1.5배 적용하기 위함
    private float weaknessdamage;

    void Awake() => currentEnemyHealth = maxHealth;

    public void ApplyDamage(float damage)
    {
        // 약점탄이 적중하면 데미지 1.5배 적용
        if (weakness) {
            weaknessdamage = damage * 1.5f;
            currentEnemyHealth -= weaknessdamage;
            SetWeakness(false); // 약점 상태 초기화
            Debug.Log($"{gameObject.name} took {weaknessdamage} damage!");
        }
        else {
            currentEnemyHealth -= damage;
            Debug.Log($"{gameObject.name} took {damage} damage!");
        }

        if (currentEnemyHealth <= 0f) Die();
    }

    public void Heal(float amount)
    {
        currentEnemyHealth = Mathf.Min(currentEnemyHealth + amount, maxHealth);
        Debug.Log($"{gameObject.name} healed {amount} HP!");
    }
    // 약점 상태 접근자(getter, setter) 메서드
    public bool GetWeakness() {
        return weakness;
    }
    public void SetWeakness(bool value) {
        weakness = value;
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        Destroy(gameObject);
    }
}
