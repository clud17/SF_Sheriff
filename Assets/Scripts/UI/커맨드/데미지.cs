using UnityEngine;
// UnityEngine.UI는 이제 버튼을 직접 참조하지 않으므로 필요 없습니다.
// using UnityEngine.UI; 
// using TMPro; // TextMeshPro를 사용한다면 이 using 문은 필요 없을 수 있습니다.

public class 데미지 : MonoBehaviour
{
    [Header("데미지 시스템 연결")]
    // 이 필드에 RevolverHealthSystem 컴포넌트가 붙어 있는 오브젝트를 할당해야 합니다.
    public RevolverHealthSystem playerHealthSystem;

    [Header("테스트 설정")]
    public int testDamageAmount = 10;   // Ctrl + '-' 키를 누를 때 줄 데미지 양
    public int testHealAmount = 10;     // Ctrl + '+' 키를 누를 때 회복할 체력 양

    // Start 함수는 이제 버튼 연결이 없으므로 비워둡니다.
    void Start()
    {
        // Debug.Log("DamageTester Start. 이 스크립트는 Ctrl + / - 키 입력을 감지합니다.");
    }

    // 매 프레임마다 키 입력을 감지
    void Update()
    {
        // --- 데미지 적용 (Ctrl + '-' 키) ---
        // Input.GetKey는 키를 누르고 있는 동안 계속 true, GetKeyDown은 처음 눌렀을 때만 true
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Minus)) // 왼쪽 Ctrl 키와 '-' 키를 동시에 눌렀을 때
        {
            if (playerHealthSystem != null)
            {
                playerHealthSystem.TakeDamage(testDamageAmount);
                Debug.Log($"Ctrl + '-' 키 입력: 데미지 {testDamageAmount} 적용!");
            }
            else
            {
                Debug.LogError("Error: RevolverHealthSystem이 연결되지 않았습니다. Inspector에서 'Player Health System' 필드를 채워주세요.");
            }
        }

        // --- 체력 회복 (Ctrl + '+' 키) ---
        // 오른쪽 Ctrl 키도 함께 고려하거나, 단순히 LeftControl만 사용해도 됩니다.
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Equals)) // 왼쪽 Ctrl 키와 '=' 키를 동시에 눌렀을 때 ('='은 Shift 없이 '+' 키)
        {
            if (playerHealthSystem != null)
            {
                playerHealthSystem.Heal(testHealAmount);
                Debug.Log($"Ctrl + '+' 키 입력: 체력 {testHealAmount} 회복!");
            }
            else
            {
                Debug.LogError("Error: RevolverHealthSystem이 연결되지 않았습니다.");
            }
        }
    }
}