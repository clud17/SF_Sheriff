using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("설정")]
    [SerializeField] private bool isEquipmentSlot;
    [SerializeField] private Image image;
    [SerializeField] private Image selectionVisual; // 선택 시 보일 테두리/이미지

    private Item _item;
    public Item item { get { return _item; } set { _item = value; UpdateSlotUI(); } }

    public static Slot targetEquipmentSlot; // 전역 선택 슬롯

    private void Awake()
    {
        if (selectionVisual != null) selectionVisual.gameObject.SetActive(false);
    }

    private void UpdateSlotUI()
    {
        if (_item != null)
        {
            image.sprite = _item.itemImage;
            image.color = Color.white;
        }
        else
        {
            image.sprite = null;
            image.color = new Color(1, 1, 1, 0);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isEquipmentSlot) // 장착칸(1~6) 클릭 시
        {
            if (targetEquipmentSlot == this) { CancelSelection(); return; }
            if (targetEquipmentSlot != null) targetEquipmentSlot.CancelSelection();

            targetEquipmentSlot = this;
            if (selectionVisual != null) selectionVisual.gameObject.SetActive(true);
        }
        else // 인벤토리(총알 소스) 클릭 시
        {
            if (targetEquipmentSlot != null && _item != null)
            {
                targetEquipmentSlot.item = this._item;
                targetEquipmentSlot.CancelSelection();
            }
        }
    }

    public void CancelSelection()
    {
        if (selectionVisual != null) selectionVisual.gameObject.SetActive(false);
        if (targetEquipmentSlot == this) targetEquipmentSlot = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_item != null) TooltipManager.instance.ShowTooltip(_item.itemName, _item.itemDescription, _item.itemImage, _item.damage);
    }

    public void OnPointerExit(PointerEventData eventData) { TooltipManager.instance.HideTooltip(); }
}