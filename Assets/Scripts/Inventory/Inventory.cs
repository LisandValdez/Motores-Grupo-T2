using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public int amount;
    public Sprite icon;
    public ItemType itemType;
    public int maxStack = 99;
    
    public int ammoCount;
    public string keyId;
    
    public InventoryItem(string name, int qty, Sprite itemIcon, ItemType type = ItemType.Collectible)
    {
        itemName = name;
        amount = qty;
        icon = itemIcon;
        itemType = type;
        maxStack = 99;
    }
}

public class Inventory : MonoBehaviour
{
    [Header("Configuración")]
    public int maxSlots = 20;
    public int maxStackSize = 99;
    
    [Header("Inventarios Especiales")]
    public Dictionary<string, int> ammoInventory = new Dictionary<string, int>();
    public List<string> keys = new List<string>();
    public Dictionary<string, InventoryItem> weapons = new Dictionary<string, InventoryItem>();
    
    [Header("Lista de Items")]
    public List<InventoryItem> items = new List<InventoryItem>();
    
    public System.Action OnInventoryChanged;
    public static Inventory Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public bool RemoveItem(string itemName, int amount)
    {
        InventoryItem item = items.Find(i => i.itemName == itemName);
        
        if (item != null)
        {
            if (item.amount >= amount)
            {
                item.amount -= amount;
                Debug.Log($"🗑️ Removido {amount}x {itemName}. Restan: {item.amount}");
                
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
    
    public bool AddItem(string itemName, int amount, Sprite icon = null, ItemType type = ItemType.Collectible)
    {
        Debug.Log($"📦 [INVENTORY] AddItem: {itemName}, cant: {amount}, icono: {(icon != null ? icon.name : "NULL")}, tipo: {type}");
        
        // Buscar si ya existe un item con el mismo nombre
        InventoryItem existingItem = items.Find(i => i.itemName == itemName);
        
        if (existingItem != null)
        {
            // Si existe, aumentar cantidad
            int spaceLeft = existingItem.maxStack - existingItem.amount;
            if (spaceLeft >= amount)
            {
                existingItem.amount += amount;
                Debug.Log($"✅ Item existente aumentado: {itemName} ahora tiene {existingItem.amount}");
                OnInventoryChanged?.Invoke();
                return true;
            }
            else if (spaceLeft > 0)
            {
                existingItem.amount = existingItem.maxStack;
                int remaining = amount - spaceLeft;
                return AddItem(itemName, remaining, icon, type);
            }
        }
        
        // Si no existe, crear nuevo slot
        if (items.Count < maxSlots)
        {
            InventoryItem newItem = new InventoryItem(itemName, amount, icon, type);
            items.Add(newItem);
            Debug.Log($"✨ Nuevo item agregado: {itemName}, icono: {(newItem.icon != null ? newItem.icon.name : "NULL")}");
            OnInventoryChanged?.Invoke();
            return true;
        }
        
        Debug.LogWarning($"❌ Inventario lleno! No se pudo agregar {amount}x {itemName}");
        return false;
    }
    
    // AMMO - CORREGIDO: guarda con el icono correcto
   public bool AddAmmo(string ammoType, int amount, Sprite icon = null)
{
    Debug.Log($"🔫 [AMMO] AddAmmo llamado:");
    Debug.Log($"   - Tipo: {ammoType}");
    Debug.Log($"   - Cantidad: {amount}");
    Debug.Log($"   - Icono recibido: {(icon != null ? icon.name : "NULL")}");
    
    if (ammoInventory.ContainsKey(ammoType))
        ammoInventory[ammoType] += amount;
    else
        ammoInventory[ammoType] = amount;
    
    // Agregar al inventario normal con su icono
    bool success = AddItem(ammoType, amount, icon, ItemType.Ammo);
    
    OnInventoryChanged?.Invoke();
    return success;
}
    // WEAPON
    public bool AddWeapon(string weaponName, int startingAmmo, Sprite icon)
    {
        Debug.Log($"⚔️ [WEAPON] Agregando: {weaponName}, icono: {(icon != null ? icon.name : "NULL")}");
        
        if (!weapons.ContainsKey(weaponName))
        {
            InventoryItem weapon = new InventoryItem(weaponName, 1, icon, ItemType.Weapon);
            weapon.ammoCount = startingAmmo;
            weapons[weaponName] = weapon;
            items.Add(weapon);
            OnInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }
    
    // KEY - CORREGIDO
    public bool AddKey(string keyId, int amount, Sprite icon = null, string keyName = null)
    {
        string displayName = keyName != null ? keyName : keyId;
        Debug.Log($"🔑 [KEY] Agregando: {displayName}, icono: {(icon != null ? icon.name : "NULL")}");
        
        for (int i = 0; i < amount; i++)
        {
            if (!keys.Contains(keyId))
            {
                keys.Add(keyId);
                InventoryItem keyItem = new InventoryItem(displayName, 1, icon, ItemType.Key);
                keyItem.keyId = keyId;
                items.Add(keyItem);
            }
        }
        OnInventoryChanged?.Invoke();
        return true;
    }
    
    public bool HasKey(string keyId)
    {
        return keys.Contains(keyId);
    }
    
    public int GetAmmo(string ammoType)
    {
        return ammoInventory.ContainsKey(ammoType) ? ammoInventory[ammoType] : 0;
    }
    
    public bool UseAmmo(string ammoType, int amount)
    {
        if (GetAmmo(ammoType) >= amount)
        {
            ammoInventory[ammoType] -= amount;
            
            // También reducir del item normal
            InventoryItem ammoItem = items.Find(i => i.itemName == ammoType && i.itemType == ItemType.Ammo);
            if (ammoItem != null)
            {
                ammoItem.amount -= amount;
                if (ammoItem.amount <= 0)
                    items.Remove(ammoItem);
            }
            
            OnInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }
    
    public bool HasItem(string itemName, int amount = 1)
    {
        InventoryItem item = items.Find(i => i.itemName == itemName);
        return item != null && item.amount >= amount;
    }
    
    public int GetItemAmount(string itemName)
    {
        InventoryItem item = items.Find(i => i.itemName == itemName);
        return item != null ? item.amount : 0;
    }
    
    public void ClearInventory()
    {
        items.Clear();
        ammoInventory.Clear();
        keys.Clear();
        weapons.Clear();
        OnInventoryChanged?.Invoke();
        Debug.Log("🧹 Inventario vaciado");
    }
}