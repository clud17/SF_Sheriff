using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Savepoint : MonoBehaviour
{
    private bool isPlayerNearby = false;
    private bool isPlayerSitting = false;
    public RevolverHealthSystem rev;
    public WeaponController weaponController;

    public GameUIController GameUIController;

    void Start()
    {
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

            rev.Heal(6);
            rev.ResetFiredBullets();
            //Player의 정보에 사망시 세이브포인트 좌표를 저장하고, Die() 호출시
            //체력을 Max로 만들고, 재장전상태로 만들며, 씬이동 및 tp로 구현합니다.
            rev.savePoint = (SceneManager.GetActiveScene().name, transform.position);
            //세이브포인트가 위치한 씬 넘버와, 좌표를 rev에 저장합니다.

        }
        if (isPlayerNearby && isPlayerSitting && Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("인벤토리 UI가 열렸습니다.");
            //일단은 열기만 했습니다.
            GameUIController.SetBulletMenu("open");

            //UI를 아직 구현하지 않은 관계로, 총알을 임의로 바꿉니다.
            // 나중에 총알 바뀌는 코드는 UI 닫을때 바뀌게 하면 되므로 UI 게임오브젝트의 ondisable()에서 호출해서 하면 될 것 같습니다.
            weaponController.setBullet(new int[] { 1, 1, 1, 1, 1, 1 });
            
        }   
    }
}
