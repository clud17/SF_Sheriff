using UnityEngine;

public enum ItemType { Gun, Ammo, Relic }

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public Sprite icon;
    public GameObject prefab; // 실제 생성할 오브젝트
    public string itemInfo;
}