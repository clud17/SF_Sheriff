using UnityEngine;

public class Box : MonoBehaviour
{
    private bool isPlayerNearby = false;
    private bool isOpened = false;

    public Sprite closedBoxSprite;
    public Sprite openedBoxSprite;

    [Header("보상 설정")]
    public Item itemToGive;

    [Tooltip("보상이 들어갈 인벤토리를 지정하세요. 비워두면 기본 인스턴스를 사용합니다.")]
    public GunInventory targetInventory;

    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = closedBoxSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = false;
    }

    void Update()
    {
        if (isPlayerNearby && !isOpened && Input.GetKeyDown(KeyCode.E))
        {
            OpenBox();
        }
    }

    void OpenBox()
    {
        if (itemToGive == null)
        {
            Debug.LogWarning(gameObject.name + " 상자에 할당된 Item이 없습니다!");
            return;
        }

        isOpened = true;
        gameObject.GetComponent<SpriteRenderer>().sprite = openedBoxSprite;
        Debug.Log($"{gameObject.name} 상자가 열렸습니다.");

        GunInventory inventory = (targetInventory != null) ? targetInventory : GunInventory.instance;

        if (inventory != null)
        {
            inventory.AddItem(itemToGive);
        }
        else
        {
            Debug.LogError("보상을 넣을 인벤토리를 찾을 수 없습니다!");
        }
    }
}