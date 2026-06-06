using UnityEngine;
using TMPro;

public class WeaponAmmoUI : MonoBehaviour
{
    [Header("Componentes de UI")]
    // 2. CAMBIA 'Text' por 'TextMeshProUGUI'
    public TextMeshProUGUI ammoText;

    [Header("Referencia del Arma Activa")]
    [SerializeField] private FireWeaponBase activeWeapon;

    private void Start()
    {
        // Escuchar si el inventario cambia (por ejemplo, al recoger balas del suelo)
        if (Inventory.Instance != null)
        {
            Inventory.Instance.OnInventoryChanged += UpdateAmmoDisplay;
        }

        // Intentar buscar de forma activa el controlador de armas si no se asignó en el inspector
        if (activeWeapon == null)
        {
            // Buscamos cualquier arma de fuego base que esté activa en la escena al iniciar
            activeWeapon = FindFirstObjectByType<FireWeaponBase>();
        }

        if (activeWeapon != null)
        {
            SetupWeaponUI(activeWeapon);
        }
        else
        {
            // Si aún no encuentra el arma, dejamos un texto temporal realista
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
        {
            activeWeapon.OnWeaponAmmoChanged += UpdateAmmoDisplay;
            UpdateAmmoDisplay();
        }
        else
        {
            ammoText.text = "- / -";
        }
    }

    public void UpdateAmmoDisplay()
    {
        if (activeWeapon == null || ammoText == null) return;

        int currentWeaponAmmo = activeWeapon.GetCurrentAmmo();
        int inventoryAmmo = Inventory.Instance != null ? Inventory.Instance.GetAmmo(activeWeapon.ammoTypeName) : 0;

        ammoText.text = $"{currentWeaponAmmo} / {inventoryAmmo}";
    }
}