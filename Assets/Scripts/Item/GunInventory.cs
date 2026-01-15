using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class GunInventory : MonoBehaviour

{
    public static GunInventory instance;
    public List<Item> items;

    // 인벤토리 총알 6개 배열

    public Item[] magazine = new Item[6];

    [SerializeField]

    private Transform slotParent;

    [SerializeField]

    private Slot[] slots;

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (slotParent != null)
        {
            slots = slotParent.GetComponentsInChildren<Slot>();
        }
    }

#endif


    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);   //중복 제거
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (items == null)
            items = new List<Item>();

        if (slotParent != null && (slots == null || slots.Length == 0))
        {
            slots = slotParent.GetComponentsInChildren<Slot>();
        }

        FreshSlot();
    }



    public void FreshSlot()

    {

        int i = 0;

        for (; i < items.Count && i < slots.Length; i++)

        {

            slots[i].item = items[i];

        }

        for (; i < slots.Length; i++)

        {

            slots[i].item = null;

        }

    }



    public void AddItem(Item _item)

    {

        if (_item == null)

        {

            Debug.LogWarning("추가하려는 Item이 null입니다.");

            return;

        }



        if (items.Count < slots.Length)

        {

            items.Add(_item);

            FreshSlot();

            Debug.Log("GunInventory에 아이템 추가됨: " + _item.name);

        }

        else

        {

            print("슬롯이 가득 차 있습니다.");

        }

    }

}