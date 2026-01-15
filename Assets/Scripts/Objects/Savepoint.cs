using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Savepoint : MonoBehaviour
{
    private bool isPlayerNearby = false;
    private bool isPlayerSitting = false;
    public RevolverHealthSystem rev;
    public WeaponController weaponController;
    public GameObject gunInventoryPanel;

    public GameUIController GameUIController;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        rev = player.GetComponent<RevolverHealthSystem>();
        weaponController = player.GetComponent<WeaponController>();
        GameUIController = player.GetComponent<GameUIController>();

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;

        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (isPlayerSitting)
            {
                isPlayerSitting = false;
                Debug.Log("의자에서 나왔습니다");
                GameUIController.SetBulletMenu("close");
            }
        }
    }
    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Savepoint가 설정되었습니다. 의자에 앉은 상태입니다.");
            isPlayerSitting = true;
            //0914 RevolverHealthSystem.cs의 Die()에서 사망시 로직을 구현합니다.
            rev.savePoint = (SceneManager.GetActiveScene().name, transform.position);
            rev.Heal(6);
            rev.ResetFiredBullets();
            //Player의 정보에 사망시 세이브포인트 좌표를 저장하고, Die() 호출시
            //체력을 Max로 만들고, 재장전상태로 만들며, 씬이동 및 tp로 구현합니다.
            
            //세이브포인트가 위치한 씬 넘버와, 좌표를 rev에 저장합니다.

        }
        if (isPlayerNearby && isPlayerSitting && Input.GetKeyDown(KeyCode.I))
        {
            // 패널이 꺼져있으면 켜고, 켜져있으면 끄기 (토글)
            bool isActive = !gunInventoryPanel.activeSelf;
            gunInventoryPanel.SetActive(isActive);

            if (isActive)
            {
                Debug.Log("인벤토리 UI가 열렸습니다.");
                GameUIController.SetBulletMenu("open");

                // UI를 아직 구현하지 않은 관계로, 총알을 임의로 바꿉니다.
                weaponController.setBullet(new int[] { 1, 1, 1, 1, 1, 1 });
                Time.timeScale = 0f;
            }
            else
            {
                Debug.Log("인벤토리 UI가 닫혔습니다.");
                GameUIController.SetBulletMenu("close");
                Time.timeScale = 1.0f;
            }
        }

        // [ESC] 키: 인벤토리 패널이 열려있을 때 닫기
        if (gunInventoryPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            gunInventoryPanel.SetActive(false);
            GameUIController.SetBulletMenu("close");
            Time.timeScale = 1f;
        }
    }
}