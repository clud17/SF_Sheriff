using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Savepoint : MonoBehaviour
{
    private bool isPlayerNearby = false;
    private bool isPlayerSitting = false;
    public RevolverHealthSystem rev;
    public WeaponController weaponController;

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
            
            //Player의 정보에 사망시 세이브포인트 좌표를 저장하고, Die() 호출시
            //체력을 Max로 만들고, 재장전상태로 만들며, 씬이동 및 tp로 구현합니다.
            rev.savePoint = (SceneManager.GetActiveScene().name, transform.position);
            Debug.Log(rev.savePoint.Item1);
            Debug.Log(rev.savePoint.Item2);
            //세이브포인트가 위치한 씬 넘버와, 좌표를 rev에 저장합니다.

        }
        if (isPlayerNearby && isPlayerSitting && Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("인벤토리 UI가 열렸습니다.");
            //UI를 아직 구현하지 않은 관계로, 총알을 임의로 바꿉니다.
            //원래는 인벤토리를 열고, 수정할 거 한 이후 UI를 닫으면 변경사항이 반영되어야 합니다.
            //UI 구현 이후에 하단에 로직을 구현하겠습니다.
            weaponController.setBullet(new int[] { 1, 1, 1, 1, 1, 1 });
            //원래는 UI가 열리고 닫힐때 탄환 정보를 가져와서 배열에 넣고. 그 배열이 setBullet의 파라미터가 되어야 합니다.
        }   
    }
}
