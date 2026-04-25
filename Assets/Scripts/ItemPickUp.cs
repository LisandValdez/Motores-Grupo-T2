using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickup : MonoBehaviour
{
    [Header("Configuración del Item")]
    public string itemName = "Item";
    public int itemAmount = 1;
    public Sprite itemIcon;  // Importante: así se verá en el inventario
    
    [Header("Efectos")]
    public GameObject pickupEffect;
    public AudioClip pickupSound;
    public float interactionRange = 2.5f;
    
    private GameObject currentPlayer;
    private bool playerInRange = false;
    private GameObject currentPrompt;

    void Start()
    {
        CreateInteractionPrompt();
    }

    void Update()
    {
        if (currentPlayer != null)
        {
            float distance = Vector3.Distance(transform.position, currentPlayer.transform.position);
            playerInRange = distance <= interactionRange;
            
            if (currentPrompt != null)
                currentPrompt.SetActive(playerInRange);
            
            if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                PickupItem();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayer = other.gameObject;
            playerInRange = true;
            if (currentPrompt != null)
                currentPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayer = null;
            playerInRange = false;
            if (currentPrompt != null)
                currentPrompt.SetActive(false);
        }
    }

    void CreateInteractionPrompt()
    {
        GameObject promptObj = new GameObject("InteractionPrompt");
        promptObj.transform.SetParent(transform);
        promptObj.transform.localPosition = new Vector3(0, 1.5f, 0);
        
        TextMesh textMesh = promptObj.AddComponent<TextMesh>();
        textMesh.text = $"Presiona <color=yellow>E</color> para agarrar\n<color=cyan>{itemName}</color>";
        textMesh.fontSize = 30;
        textMesh.characterSize = 0.03f;
        textMesh.color = Color.white;
        textMesh.alignment = TextAlignment.Center;
        
        promptObj.AddComponent<Billboard>();
        currentPrompt = promptObj;
        promptObj.SetActive(false);
    }

    void PickupItem()
    {
        Debug.Log($"🎁 Intentando agarrar {itemAmount}x {itemName}");
        
        // AGREGAR AL INVENTARIO DEL JUGADOR
        Inventory playerInventory = currentPlayer.GetComponent<Inventory>();
        
        if (playerInventory != null)
        {
            bool success = playerInventory.AddItem(itemName, itemAmount, itemIcon);
            
            if (success)
            {
                Debug.Log($"✅ Agarraste {itemAmount}x {itemName}");
                
                // Efectos visuales
                if (pickupEffect != null)
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                
                if (pickupSound != null)
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position, 1f);
                
                ShowPickupMessage();
                
                // Destruir el item
                Destroy(gameObject);
            }
            else
            {
                Debug.Log($"❌ Inventario lleno! No pudiste agarrar {itemName}");
                ShowInventoryFullMessage();
            }
        }
        else
        {
            Debug.LogError("❌ El jugador no tiene componente Inventory!");
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
        textMesh.color = Color.green;
        textMesh.fontStyle = FontStyle.Bold;
        
        messageObj.AddComponent<PickupMessageAnimator>();
        Destroy(messageObj, 1.5f);
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
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}

// Script auxiliar para el texto flotante
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