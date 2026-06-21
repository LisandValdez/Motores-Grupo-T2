using UnityEngine;
using TMPro;

public class WeaponAmmoUI : MonoBehaviour
{
    [Header("Componentes de UI")]
    public TextMeshProUGUI ammoText;

    [Header("Referencia del Arma Activa")]
    [SerializeField] private FireWeaponBase activeWeapon;

    private void Start()
    {
        // Escuchar si el inventario cambia (por ejemplo, al levantar munición del suelo)
        if (Inventory.Instance != null)
        {
            Inventory.Instance.OnInventoryChanged += UpdateAmmoDisplay;
        }

        if (activeWeapon == null)
        {
            activeWeapon = FindFirstObjectByType<FireWeaponBase>();
        }

        if (activeWeapon != null)
        {
            SetupWeaponUI(activeWeapon);
            UpdateAmmoDisplay();
        }
        else
        {
            ammoText.text = "- / -";
        }
    }

    private void OnDestroy()
    {
        if (Inventory.Instance != null)
        {
            Inventory.Instance.OnInventoryChanged -= UpdateAmmoDisplay;
        }

        if (activeWeapon != null)
        {
            activeWeapon.OnWeaponAmmoChanged -= UpdateAmmoDisplay;
        }
    }

    public void SetupWeaponUI(FireWeaponBase newWeapon)
    {
        if (activeWeapon != null)
        {
            activeWeapon.OnWeaponAmmoChanged -= UpdateAmmoDisplay;
        }

        activeWeapon = newWeapon;

        if (activeWeapon != null)
            activeWeapon.OnWeaponAmmoChanged += UpdateAmmoDisplay;

        UpdateAmmoDisplay();
    }

    public void UpdateAmmoDisplay()
    {
        if (activeWeapon == null || ammoText == null) return;

        int currentWeaponAmmo = activeWeapon.GetCurrentAmmo();
        int inventoryAmmo = Inventory.Instance != null ? Inventory.Instance.GetAmmo(activeWeapon.ammoTypeName) : 0;

        ammoText.text = $"{currentWeaponAmmo} / {inventoryAmmo}";
    }
}