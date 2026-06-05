using UnityEngine;
using UnityEngine.InputSystem;

public enum ItemType
{
    Consumable,
    Weapon,
    Ammo,
    Key,
    Quest,
    Collectible
}

public class ItemPickup : MonoBehaviour, IInteractable
{
    [Header("Configuración del Item")]
    public string itemName = "Item";
    public int itemAmount = 1;
    public Sprite itemIcon;
    public ItemType itemType = ItemType.Collectible;
    
    [Header("Propiedades Específicas")]
    public int ammoCount = 0;
    public int healAmount = 0;
    public string keyId = "";
    
    [Header("Efectos")]
    public GameObject pickupEffect;
    public AudioClip pickupSound;
    
    [Header("Visual")]
    public float promptHeight = 1.5f;
    public float promptFontSize = 30;
    public float promptCharacterSize = 0.03f;
    
    private GameObject currentPrompt;
    private bool playerInRange = false;
    private GameObject currentPlayer;

    void Start()
    {
        // Verificar que el collider sea trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"⚠️ El collider de {itemName} no es trigger. Configurando automáticamente...");
            col.isTrigger = true;
        }
        
        CreateInteractionPrompt();
        
        if (currentPrompt != null)
            currentPrompt.SetActive(false);
        
        Debug.Log($"✨ Item {itemName} inicializado. Collider: {(col != null ? (col.isTrigger ? "Trigger OK" : "NO Trigger!") : "No collider!")}");
    }

    void Update()
    {
        // Actualizar visibilidad del prompt según si el jugador está en rango
        if (currentPrompt != null && currentPrompt.activeSelf != playerInRange)
        {
            currentPrompt.SetActive(playerInRange);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayer = other.gameObject;  // ← GUARDAR REFERENCIA
            playerInRange = true;
            Debug.Log($"✅ Jugador entró en rango del item: {itemName}");
            
            if (currentPrompt != null)
                currentPrompt.SetActive(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayer = null;  // ← LIMPIAR REFERENCIA
            playerInRange = false;
            Debug.Log($"❌ Jugador salió del rango del item: {itemName}");
            
            if (currentPrompt != null)
                currentPrompt.SetActive(false);
        }
    }

    void CreateInteractionPrompt()
{
    if (currentPrompt != null) return;
    
    GameObject promptObj = new GameObject("InteractionPrompt");
    promptObj.transform.SetParent(transform);
    
    // 🔧 CALCULAR ALTURA AUTOMÁTICAMENTE
    float calculatedHeight = CalculatePromptHeight();
    promptObj.transform.localPosition = new Vector3(0, calculatedHeight, 0);
    
    TextMesh textMesh = promptObj.AddComponent<TextMesh>();
    string typeIcon = GetTypeIcon();
    textMesh.text = $"Presiona <color=yellow>E</color> para agarrar\n<color=cyan>{typeIcon} {itemName}</color>";
    textMesh.fontSize = (int)promptFontSize;
    textMesh.characterSize = promptCharacterSize;
    textMesh.color = Color.white;
    textMesh.alignment = TextAlignment.Center;
    
    promptObj.AddComponent<Billboard>();
    currentPrompt = promptObj;
    promptObj.SetActive(false);
    
    Debug.Log($"✨ Prompt creado para item: {itemName}, altura: {calculatedHeight}");
}

float CalculatePromptHeight()
{
    float height = promptHeight; // Valor por defecto (1.5f)
    
    // Método 1: Usar Collider
    Collider col = GetComponent<Collider>();
    if (col != null)
    {
        height = col.bounds.extents.y + 0.5f;
        Debug.Log($"📏 Altura calculada por Collider: {height}");
        return height;
    }
    
    // Método 2: Usar MeshRenderer
    MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
    if (meshRenderer != null)
    {
        height = meshRenderer.bounds.extents.y + 0.5f;
        Debug.Log($"📏 Altura calculada por MeshRenderer: {height}");
        return height;
    }
    
    // Método 3: Usar SpriteRenderer
    SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    if (spriteRenderer != null && spriteRenderer.sprite != null)
    {
        height = spriteRenderer.bounds.extents.y + 0.5f;
        Debug.Log($"📏 Altura calculada por SpriteRenderer: {height}");
        return height;
    }
    
    // Método 4: Usar transform.localScale
    height = transform.localScale.y + 0.5f;
    Debug.Log($"📏 Altura calculada por Scale: {height}");
    
    return height;
}
    
    string GetTypeIcon()
    {
        switch (itemType)
        {
            case ItemType.Consumable: return "💊";
            case ItemType.Weapon: return "⚔️";
            case ItemType.Ammo: return "🔫";
            case ItemType.Key: return "🔑";
            case ItemType.Quest: return "⭐";
            default: return "📦";
        }
    }

   public void Interact()
{
    if (!playerInRange)
    {
        Debug.Log($"⚠️ No estás en rango de {itemName}");
        return;
    }
    
    // 🔍 DEBUG CRÍTICO
    Debug.Log($"🎯 [PICKUP] Interact con: {itemName}");
    Debug.Log($"🎨 [PICKUP] itemIcon: {(itemIcon != null ? itemIcon.name : "NULL")}");
    Debug.Log($"🎨 [PICKUP] itemIcon tipo: {(itemIcon != null ? itemIcon.GetType().ToString() : "NULL")}");
    
    Inventory playerInventory = FindFirstObjectByType<Inventory>();
    
    if (playerInventory != null)
    {
        bool success = false;
        
        switch (itemType)
        {
            case ItemType.Ammo:
                Debug.Log($"🔫 [PICKUP] Llamando a AddAmmo con: {itemName}, {itemAmount}, icono: {(itemIcon != null ? itemIcon.name : "NULL")}");
                success = playerInventory.AddAmmo(itemName, itemAmount, itemIcon);
                break;
            case ItemType.Weapon:
                success = playerInventory.AddWeapon(itemName, ammoCount, itemIcon);
                break;
            case ItemType.Key:
                success = playerInventory.AddKey(keyId, itemAmount, itemIcon, itemName);
                break;
            default:
                success = playerInventory.AddItem(itemName, itemAmount, itemIcon, itemType);
                break;
        }
        
        if (success)
        {
            Debug.Log($"✅ Agarraste {itemAmount}x {itemName}");
            
            if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            
            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, 1f);
            
            ShowPickupMessage();
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"❌ No pudiste agarrar {itemName}");
            ShowInventoryFullMessage();
        }
    }
}
    
    void ShowPickupMessage()
    {
        GameObject messageObj = new GameObject("PickupMessage");
        messageObj.transform.position = transform.position + Vector3.up * 2f;
        
        TextMesh textMesh = messageObj.AddComponent<TextMesh>();
        textMesh.text = $"+{itemAmount} {itemName}";
        textMesh.fontSize = 40;
        textMesh.characterSize = 0.05f;
        textMesh.color = GetColorByType();
        textMesh.fontStyle = FontStyle.Bold;
        
        messageObj.AddComponent<PickupMessageAnimator>();
        Destroy(messageObj, 1.5f);
    }
    
    Color GetColorByType()
    {
        switch (itemType)
        {
            case ItemType.Consumable: return Color.red;
            case ItemType.Weapon: return new Color(1f, 0.5f, 0f);
            case ItemType.Ammo: return Color.yellow;
            case ItemType.Key: return Color.gold;
            case ItemType.Quest: return Color.magenta;
            default: return Color.green;
        }
    }
    
    void ShowInventoryFullMessage()
    {
        GameObject messageObj = new GameObject("InventoryFullMessage");
        messageObj.transform.position = transform.position + Vector3.up * 2f;
        
        TextMesh textMesh = messageObj.AddComponent<TextMesh>();
        textMesh.text = "❌ Inventario lleno!";
        textMesh.fontSize = 40;
        textMesh.characterSize = 0.05f;
        textMesh.color = Color.red;
        textMesh.fontStyle = FontStyle.Bold;
        
        messageObj.AddComponent<PickupMessageAnimator>();
        Destroy(messageObj, 1.5f);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}

public class PickupMessageAnimator : MonoBehaviour
{
    private float timer = 0f;
    private TextMesh textMesh;
    
    void Start()
    {
        textMesh = GetComponent<TextMesh>();
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        transform.position += Vector3.up * Time.deltaTime * 1.5f;
        
        if (textMesh != null)
        {
            Color color = textMesh.color;
            color.a = Mathf.Lerp(1f, 0f, timer / 1.5f);
            textMesh.color = color;
        }
    }
}