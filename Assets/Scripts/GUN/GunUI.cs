using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunUI : MonoBehaviour
{
    public TextMeshProUGUI maxhpText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI ammoText;


    private GunController gun;

    void Start()
    {
        gun = FindObjectOfType<GunController>();
    }

    void Update()
    {
        maxhpText.text = $"Max HP: {gun.maxHP}";
        hpText.text = $"HP: {gun.currentHP}";
        ammoText.text = $"Ammo: {gun.currentAmmo}";
    }
}
