using UnityEngine;

public class BulletUIManager : MonoBehaviour
{
    //BulletUI에 들어가는 스크립트입니다.
    private GameObject selectedBullet;
    private GameObject bulletInventory;
    private GameObject bulletInfo;
    public WeaponController weaponController;

    void Start()
    {
        selectedBullet = transform.GetChild(0).gameObject;
        bulletInventory = transform.GetChild(1).gameObject;
        bulletInfo = transform.GetChild(2).gameObject;
    }
    void OnEnable()
    {
        //OnEnable은 스크립트가 setActive(true)될때마다 호출되므로,
        //기존의 총알 정보 및 해금 정보를 가져옵니다.

        // -메모-

        /*
        Enable에선 두 개 해야함.
        weaponController에서 사용하고 있는 총알 가져와서 selectedBullet 채우기
        weaponController에서 모든 총알이랑 bool 배열 갖고와서 Inventory 채우기
        Disable에선 한 개 해야함.
        weaponController.setBullet(selectedBullet에 담긴걸로 만든 배열)
        리팩토링 하려면 배열의 파라미터를 정수가 아닌 data 자체로 하면 좋을듯.
        */
    }
    void OnDisable()
    {
        //OnDisable은 스크립트가 setActive(false)될때마다 호출되므로,
        //변경된 총알 정보를 실제 총에 적용시킵니다.

    }
    
}
