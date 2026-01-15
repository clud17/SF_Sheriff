using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemImage;

    //아이템 설명
    [TextArea]
    public string itemDescription;
    public int damage;
}
