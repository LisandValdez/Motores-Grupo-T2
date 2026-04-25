using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Componentes UI")]
    public Image itemIcon;
    public Text amountText;
    public GameObject highlightBorder;
    
    private int slotIndex;
    private InventoryUI parentUI;
    private InventoryItem currentItem;
    
    public void Initialize(int index, InventoryUI ui)
    {
        slotIndex = index;
        parentUI = ui;
        
        // Buscar componentes si no están asignados
        if (itemIcon == null)
            itemIcon = GetComponentInChildren<Image>();
        
        if (amountText == null)
            amountText = GetComponentInChildren<Text>();
        
        if (highlightBorder != null)
            highlightBorder.SetActive(false);
    }
    
    public void SetItem(InventoryItem item)
    {
        currentItem = item;
        
        if (itemIcon != null && item.icon != null)
            itemIcon.sprite = item.icon;
        
        if (amountText != null)
        {
            amountText.text = item.amount > 1 ? item.amount.ToString() : "";
            amountText.gameObject.SetActive(item.amount > 1);
        }
    }
    
    public void ClearSlot()
    {
        currentItem = null;
        
        if (itemIcon != null)
            itemIcon.sprite = null;
        
        if (amountText != null)
        {
            amountText.text = "";
            amountText.gameObject.SetActive(false);
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null) return;
        
        // Click derecho para usar/equipar item
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            UseItem();
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            if (highlightBorder != null)
                highlightBorder.SetActive(true);
            
            if (parentUI != null)
                parentUI.ShowItemDetails(currentItem);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (highlightBorder != null)
            highlightBorder.SetActive(false);
        
        if (parentUI != null)
            parentUI.HideItemDetails();
    }
    
    void UseItem()
    {
        Debug.Log($"Usando item: {currentItem.itemName}");
        
        // Aquí puedes agregar lógica para usar items
        switch (currentItem.itemName)
        {
            case "Poción":
                // Curar al jugador
                // PlayerHealth.Instance.Heal(20);
                break;
            case "Manzana":
                // Curar un poco
                break;
        }
        
        // Remover un item del inventario
        Inventory playerInventory = FindFirstObjectByType<Inventory>();
        if (playerInventory != null)
        {
            playerInventory.RemoveItem(currentItem.itemName, 1);
        }
    }
}