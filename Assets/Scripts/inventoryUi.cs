using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Configuración del Panel")]
    public GameObject inventoryPanel;      // Panel principal del inventario
    public Transform slotsParent;          // GameObject padre que contiene los slots
    public GameObject slotPrefab;          // Prefab del slot (un botón con imagen)
    
    [Header("Teclas")]
    public KeyCode toggleKey = KeyCode.I;   // Tecla para abrir/cerrar inventario
    
    [Header("Detalles del Item")]
    public GameObject itemDetailPanel;     // Panel para mostrar detalles
    public Text itemNameText;
    public Text itemAmountText;
    public Text itemDescriptionText;
    
    private List<InventorySlot> slots = new List<InventorySlot>();
    private bool isInventoryOpen = false;
    private Inventory playerInventory;
    
    void Start()
    {
        // Buscar el inventario del jugador
        playerInventory = FindFirstObjectByType<Inventory>();
        
        if (playerInventory == null)
        {
            Debug.LogError("❌ No se encontró el componente Inventory en el jugador!");
            return;
        }
        
        // Crear los slots
        CreateSlots();
        
        // Ocultar paneles al inicio
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
        
        if (itemDetailPanel != null)
            itemDetailPanel.SetActive(false);
        
        // Suscribirse al evento de cambio de inventario
        playerInventory.OnInventoryChanged += RefreshUI;
        
        // Refrescar UI inicial
        RefreshUI();
    }
    
    void Update()
    {
        // Abrir/cerrar inventario con tecla I
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInventory();
        }
        
        // Cerrar inventario con ESC si está abierto
        if (isInventoryOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventory();
        }
    }
    
    void CreateSlots()
    {
        // Limpiar slots existentes
        foreach (Transform child in slotsParent)
        {
            Destroy(child.gameObject);
        }
        slots.Clear();
        
        // Crear nuevos slots según maxSlots del inventario
        for (int i = 0; i < playerInventory.maxSlots; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsParent);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            
            if (slot == null)
                slot = slotObj.AddComponent<InventorySlot>();
            
            slot.Initialize(i, this);
            slots.Add(slot);
        }
    }
    
    public void RefreshUI()
    {
        if (playerInventory == null) return;
        
        // Actualizar cada slot
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < playerInventory.items.Count)
            {
                // Slot con item
                InventoryItem item = playerInventory.items[i];
                slots[i].SetItem(item);
            }
            else
            {
                // Slot vacío
                slots[i].ClearSlot();
            }
        }
    }
    
    public void ToggleInventory()
    {
        if (isInventoryOpen)
            CloseInventory();
        else
            OpenInventory();
    }
    
    public void OpenInventory()
    {
        isInventoryOpen = true;
        inventoryPanel.SetActive(true);
        RefreshUI();
        
        // Opcional: pausar el juego
        // Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void CloseInventory()
    {
        isInventoryOpen = false;
        inventoryPanel.SetActive(false);
        
        if (itemDetailPanel != null)
            itemDetailPanel.SetActive(false);
        
        // Opcional: reanudar el juego
        // Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void ShowItemDetails(InventoryItem item)
    {
        if (itemDetailPanel == null) return;
        
        itemDetailPanel.SetActive(true);
        
        if (itemNameText != null)
            itemNameText.text = item.itemName;
        
        if (itemAmountText != null)
            itemAmountText.text = $"Cantidad: {item.amount}";
        
        if (itemDescriptionText != null)
            itemDescriptionText.text = GetItemDescription(item.itemName);
    }
    
    public void HideItemDetails()
    {
        if (itemDetailPanel != null)
            itemDetailPanel.SetActive(false);
    }
    
    string GetItemDescription(string itemName)
    {
        // Puedes expandir esto con una base de datos de items
        switch (itemName)
        {
            case "Poción":
                return "Restaura 20 de vida.";
            case "Manzana":
                return "Una fruta deliciosa. Restaura 5 de vida.";
            case "Llave":
                return "Abre puertas misteriosas.";
            case "Moneda":
                return "Brillante y valiosa. Úsala para comprar items.";
            default:
                return "Un item común.";
        }
    }
    
    public bool IsInventoryOpen()
    {
        return isInventoryOpen;
    }
}