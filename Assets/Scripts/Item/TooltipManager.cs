using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;

    [SerializeField] private GameObject tooltipWindow;
    [SerializeField] private Image itemImage; // 오른쪽 설명창에 있는 이미지 UI
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI damageText;

    private void Awake()
    {
        instance = this;
        tooltipWindow.SetActive(false);
    }

    public void ShowTooltip(
        string itemName,
        string itemDesc,
        Sprite itemSprite,
        int itemDamage
    )
    {
        tooltipWindow.SetActive(true);

        nameText.text = itemName;
        descText.text = itemDesc;
        itemImage.sprite = itemSprite;
        damageText.text = "Damage: " + itemDamage.ToString();
    }

    public void HideTooltip()
    {
        tooltipWindow.SetActive(false);
    }
}