using UnityEngine;
using UnityEngine.InputSystem;

public class FuseBox : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    public string requiredItemName = "Fusible";  // Nombre del item necesario
    public string requiredKeyId = "";            // ID de llave (opcional)
    
    [Header("Mensajes")]
    [TextArea(3, 5)]
    public string missingItemMessage = "🔌 ¡Falta un fusible! Necesito uno para restaurar la energía.";
    
    [TextArea(3, 5)]
    public string successMessage = "✅ ¡Fusible colocado! La energía ha sido restaurada.";
    
    [TextArea(3, 5)]
    public string alreadyCompletedMessage = "La energía ya está restaurada.";
    
    [Header("Efectos")]
    public GameObject onCompleteEffect;      // Efecto al completar (partículas, luz)
    public AudioClip onCompleteSound;        // Sonido al completar
    public AudioClip onErrorSound;           // Sonido si falta el item
    
    [Header("Visual")]
    public float interactionRange = 3f;
    public bool showPrompt = true;
    public float promptHeight = 1.8f;
    
    [Header("Estado")]
    public bool isCompleted = false;          // Si ya se completó
    
    // Eventos para otros sistemas
    public System.Action OnFusePlaced;
    
    private GameObject currentPrompt;
    private GameObject currentPlayer;
    private bool playerInRange = false;

    void Start()
    {
        if (showPrompt)
            CreateInteractionPrompt();
    }

    void Update()
    {
        if (currentPrompt != null && currentPrompt.activeSelf != playerInRange)
        {
            currentPrompt.SetActive(playerInRange);
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
        promptObj.transform.localPosition = new Vector3(0, promptHeight, 0);
        
        TextMesh textMesh = promptObj.AddComponent<TextMesh>();
        string status = isCompleted ? "✅ Completado" : "🔌 Caja de Fusibles";
        textMesh.text = $"Presiona <color=yellow>E</color> para interactuar\n<color=cyan>{status}</color>";
        textMesh.fontSize = 30;
        textMesh.characterSize = 0.03f;
        textMesh.color = Color.white;
        textMesh.alignment = TextAlignment.Center;
        
        promptObj.AddComponent<Billboard>();
        currentPrompt = promptObj;
        promptObj.SetActive(false);
    }

    public void Interact()
    {
        if (!playerInRange)
        {
            Debug.Log("⚠️ No estás en rango de la caja de fusibles");
            return;
        }
        
        if (isCompleted)
        {
            ShowMessage(alreadyCompletedMessage, Color.gray);
            return;
        }
        
        // Verificar si el jugador tiene el fusible
        Inventory playerInventory = currentPlayer.GetComponent<Inventory>();
        
        if (playerInventory == null)
        {
            Debug.LogError("❌ No se encontró el inventario del jugador");
            return;
        }
        
        // Verificar si tiene el item requerido
        bool hasRequiredItem = playerInventory.HasItem(requiredItemName);
        
        if (hasRequiredItem)
        {
            // Tiene el fusible - colocarlo
            playerInventory.RemoveItem(requiredItemName, 1);
            isCompleted = true;
            
            ShowMessage(successMessage, Color.green);
            Debug.Log($"✅ Fusible colocado en {gameObject.name}");
            
            // Efectos visuales y sonido
            if (onCompleteEffect != null)
                Instantiate(onCompleteEffect, transform.position, Quaternion.identity);
            
            if (onCompleteSound != null)
                AudioSource.PlayClipAtPoint(onCompleteSound, transform.position, 1f);
            
            // Disparar evento
            OnFusePlaced?.Invoke();
            
            // Actualizar prompt
            if (currentPrompt != null)
            {
                TextMesh textMesh = currentPrompt.GetComponent<TextMesh>();
                if (textMesh != null)
                {
                    textMesh.text = $"<color=green>✅ Completado</color>";
                }
            }
        }
        else
        {
            // No tiene el fusible
            ShowMessage(missingItemMessage, Color.yellow);
            Debug.Log($"🔌 Jugador intentó usar {gameObject.name} pero no tiene {requiredItemName}");
            
            if (onErrorSound != null)
                AudioSource.PlayClipAtPoint(onErrorSound, transform.position, 1f);
        }
    }
    
    void ShowMessage(string message, Color color)
    {
        GameObject messageObj = new GameObject("FuseBoxMessage");
        messageObj.transform.SetParent(transform);
        messageObj.transform.localPosition = new Vector3(0, promptHeight + 0.8f, 0);
        
        TextMesh textMesh = messageObj.AddComponent<TextMesh>();
        textMesh.text = message;
        textMesh.fontSize = 35;
        textMesh.characterSize = 0.04f;
        textMesh.color = color;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontStyle = FontStyle.Bold;
        
        messageObj.AddComponent<Billboard>();
        
        // Animación de desvanecimiento
        FadeMessage fade = messageObj.AddComponent<FadeMessage>();
        fade.lifetime = 2.5f;
        
        Destroy(messageObj, 2.5f);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}

// Script para desvanecer mensajes
public class FadeMessage : MonoBehaviour
{
    public float lifetime = 2.5f;
    private float timer = 0f;
    private TextMesh textMesh;
    private Vector3 startPosition;
    
    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        startPosition = transform.position;
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / lifetime;
        
        // Subir el texto
        transform.position = startPosition + Vector3.up * (progress * 1.5f);
        
        // Desvanecer
        if (textMesh != null)
        {
            Color color = textMesh.color;
            color.a = Mathf.Lerp(1f, 0f, progress);
            textMesh.color = color;
        }
    }
}