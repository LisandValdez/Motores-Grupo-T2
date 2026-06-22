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
        
        
        Transform iconTransform = transform.Find("Icon");
        if (iconTransform != null)
        {
            itemIcon = iconTransform.GetComponent<Image>();
            Debug.Log($"Slot {index}: Encontrado hijo 'Icon'");
        }
        else
        {
            Debug.LogError($"Slot {index}: No se encontró hijo 'Icon'. Los hijos son: {GetChildNames()}");
        }
        
        Transform amountTransform = transform.Find("Amount");
        if (amountTransform != null)
        {
            amountText = amountTransform.GetComponent<Text>();
            Debug.Log($"Slot {index}: Encontrado hijo 'Amount'");
        }
        else
        {
            Debug.LogWarning($"Slot {index}: No se encontró hijo 'Amount'");
        }
        
        Transform highlightTransform = transform.Find("HighlightBorder");
        if (highlightTransform != null)
        {
            highlightBorder = highlightTransform.gameObject;
        }
        
        if (highlightBorder != null)
            highlightBorder.SetActive(false);
    }
    
    string GetChildNames()
    {
        string names = "";
        foreach (Transform child in transform)
        {
            names += child.name + ", ";
        }
        return names;
    }
    
    public void SetItem(InventoryItem item)
    {
        currentItem = item;
        
        Debug.Log($" [SLOT] SetItem: {item.itemName}, Icono: {(item.icon != null ? item.icon.name : "NULL")}");
        
        if (itemIcon != null)
        {
            if (item.icon != null)
            {
                itemIcon.sprite = item.icon;
                itemIcon.color = Color.white;
                Debug.Log($" Icono asignado al hijo 'Icon': {item.icon.name}");
            }
            else
            {
                Debug.LogWarning($" El item {item.itemName} no tiene icono");
                itemIcon.sprite = null;
            }
        }
        else
        {
            Debug.LogError($"itemIcon es NULL en slot {slotIndex}!");
        }
        
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
        {
            itemIcon.sprite = null;
        }
        
        if (amountText != null)
        {
            amountText.text = "";
            amountText.gameObject.SetActive(false);
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null) return;
        
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
        
        switch (currentItem.itemType)
        {
            case ItemType.Consumable:
                Debug.Log($" Usaste {currentItem.itemName}");
                break;
            case ItemType.Weapon:
                Debug.Log($" Equipaste {currentItem.itemName}");
                break;
        }
        
        Inventory playerInventory = Inventory.Instance;
        if (playerInventory != null)
        {
            playerInventory.RemoveItem(currentItem.itemName, 1);
        }
    }
}