using UnityEngine;
using System.Collections.Generic;

// ✅ MOVER InventoryItem FUERA de la clase Inventory
[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public int amount;
    public Sprite icon;
    
    public InventoryItem(string name, int qty, Sprite itemIcon)
    {
        itemName = name;
        amount = qty;
        icon = itemIcon;
    }
}

public class Inventory : MonoBehaviour
{
    [Header("Configuración")]
    public int maxSlots = 20;           // Máximo de items diferentes
    public int maxStackSize = 99;       // Máximo por slot
    
    [Header("Lista de Items")]
    public List<InventoryItem> items = new List<InventoryItem>();
    
    // Eventos para la UI
    public System.Action OnInventoryChanged;
    
    // Singleton fácil de acceder
    public static Inventory Instance { get; private set; }
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Ejemplo: agregar items de prueba (borrar después)
        // AddItem("Manzana", 5, null);
        // AddItem("Poción", 2, null);
    }
    
    /// <summary>
    /// Agrega un item al inventario
    /// </summary>
    /// <returns>True si se pudo agregar, False si no hay espacio</returns>
    public bool AddItem(string itemName, int amount, Sprite icon = null)
    {
        // Buscar si ya tenemos ese item
        InventoryItem existingItem = items.Find(i => i.itemName == itemName);
        
        if (existingItem != null)
        {
            // Si ya existe, aumentar cantidad (respetando el máximo por slot)
            int spaceLeft = maxStackSize - existingItem.amount;
            if (spaceLeft >= amount)
            {
                existingItem.amount += amount;
                Debug.Log($"✅ Agregado {amount}x {itemName}. Total: {existingItem.amount}");
                OnInventoryChanged?.Invoke();
                return true;
            }
            else if (spaceLeft > 0)
            {
                // Llenar el slot existente y guardar el resto
                existingItem.amount = maxStackSize;
                int remaining = amount - spaceLeft;
                Debug.Log($"⚠️ Slot de {itemName} lleno. Restan {remaining} por agregar");
                return AddItem(itemName, remaining, icon); // Recursivo para nuevo slot
            }
        }
        
        // Si no existe o el slot está lleno, crear nuevo slot
        if (items.Count < maxSlots)
        {
            items.Add(new InventoryItem(itemName, amount, icon));
            Debug.Log($"✨ Nuevo item: {amount}x {itemName}");
            OnInventoryChanged?.Invoke();
            return true;
        }
        
        Debug.LogWarning($"❌ Inventario lleno! No se pudo agregar {amount}x {itemName}");
        return false;
    }
    
    /// <summary>
    /// Remueve una cantidad de un item
    /// </summary>
    public bool RemoveItem(string itemName, int amount)
    {
        InventoryItem item = items.Find(i => i.itemName == itemName);
        
        if (item != null)
        {
            if (item.amount >= amount)
            {
                item.amount -= amount;
                Debug.Log($"🗑️ Removido {amount}x {itemName}. Restan: {item.amount}");
                
                // Si la cantidad llega a 0, eliminar el slot
                if (item.amount <= 0)
                {
                    items.Remove(item);
                    Debug.Log($"📦 Slot de {itemName} vacío, eliminado");
                }
                
                OnInventoryChanged?.Invoke();
                return true;
            }
            else
            {
                Debug.LogWarning($"⚠️ No hay suficientes {itemName}. Tienes: {item.amount}, necesitas: {amount}");
                return false;
            }
        }
        
        Debug.LogWarning($"❌ No se encontró {itemName} en el inventario");
        return false;
    }
    
    /// <summary>
    /// Verifica si tienes cierta cantidad de un item
    /// </summary>
    public bool HasItem(string itemName, int amount = 1)
    {
        InventoryItem item = items.Find(i => i.itemName == itemName);
        return item != null && item.amount >= amount;
    }
    
    /// <summary>
    /// Obtiene la cantidad de un item
    /// </summary>
    public int GetItemAmount(string itemName)
    {
        InventoryItem item = items.Find(i => i.itemName == itemName);
        return item != null ? item.amount : 0;
    }
    
    /// <summary>
    /// Vacía todo el inventario
    /// </summary>
    public void ClearInventory()
    {
        items.Clear();
        OnInventoryChanged?.Invoke();
        Debug.Log("🧹 Inventario vaciado");
    }
}