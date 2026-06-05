using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections;  // ← IMPORTANTE: Para IEnumerator

public class FuseBox : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    public string requiredItemName = "Fusible";
    public string requiredKeyId = "";
    
    [Header("Mensajes")]
    [TextArea(3, 5)]
    public string missingItemMessage = "🔌 ¡Falta un fusible! Necesito uno para restaurar la energía.";
    
    [TextArea(3, 5)]
    public string successMessage = "✅ ¡Fusible colocado! La energía ha sido restaurada.";
    
    [TextArea(3, 5)]
    public string alreadyCompletedMessage = "La energía ya está restaurada.";
    
    [Header("Efectos")]
    public GameObject onCompleteEffect;
    public AudioClip onCompleteSound;
    public AudioClip onErrorSound;
    
    [Header("Cinemática del Ascensor")]
    public CinemachineCamera camaraAscensor;
    public CinemachineCamera camaraJugador;
    public GameObject canvasUI;
    public Light luzAscensor;
    public float duracionCinematica = 3f;
    public bool mostrarCinematicaAlColocarFusible = true;

    [Header("Visual")]
    public float interactionRange = 3f;
    public bool showPrompt = true;
    public float promptHeight = 1.8f;
    
    [Header("Estado")]
    public bool isCompleted = false;
    
    public System.Action OnFusePlaced;
    
    private GameObject currentPrompt;
    private GameObject currentPlayer;
    private bool playerInRange = false;
    private bool enCinematica = false;

    void Start()
    {
        if (showPrompt)
            CreateInteractionPrompt();
        
        // Configurar estado inicial de las cámaras
        if (camaraJugador != null)
            camaraJugador.gameObject.SetActive(true);
        
        if (camaraAscensor != null)
            camaraAscensor.gameObject.SetActive(false);
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
        
        Inventory playerInventory = currentPlayer.GetComponent<Inventory>();
        
        if (playerInventory == null)
        {
            Debug.LogError("❌ No se encontró el inventario del jugador");
            return;
        }
        
        bool hasRequiredItem = playerInventory.HasItem(requiredItemName);
        
        if (hasRequiredItem)
        {
            playerInventory.RemoveItem(requiredItemName, 1);
            isCompleted = true;
            
            ShowMessage(successMessage, Color.green);
            Debug.Log($"✅ Fusible colocado en {gameObject.name}");
            
            if (onCompleteEffect != null)
                Instantiate(onCompleteEffect, transform.position, Quaternion.identity);
            
            if (onCompleteSound != null)
                AudioSource.PlayClipAtPoint(onCompleteSound, transform.position, 1f);
            
            if (mostrarCinematicaAlColocarFusible && !enCinematica)
            {
                StartCoroutine(CinematicaAscensor());
            }

            OnFusePlaced?.Invoke();
            
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
            ShowMessage(missingItemMessage, Color.yellow);
            Debug.Log($"🔌 Jugador intentó usar {gameObject.name} pero no tiene {requiredItemName}");
            
            if (onErrorSound != null)
                AudioSource.PlayClipAtPoint(onErrorSound, transform.position, 1f);
        }
    }

    IEnumerator CinematicaAscensor()
{
    enCinematica = true;

    if (canvasUI != null)
    {
        canvasUI.SetActive(false);
        Debug.Log("🖥️ Canvas ocultado durante la cinemática");
    }
    
    // 🔥 OBTENER REFERENCIA AL ARMA
    GameObject weaponObject = null;
    if (currentPlayer != null)
    {
        // Buscar el arma (ajusta el nombre según tu jerarquía)
        weaponObject = currentPlayer.transform.Find("Weapon Holder")?.gameObject;
        if (weaponObject == null)
            weaponObject = GameObject.FindGameObjectWithTag("Weapon");
        
        // Ocultar el arma
        if (weaponObject != null)
        {
            weaponObject.SetActive(false);
            Debug.Log("🔫 Arma ocultada para la cinemática");
        }
    }

    // Desactivar movimiento del jugador
    if (currentPlayer != null)
    {
        var playerMove = currentPlayer.GetComponent<PlayerMove>();
        if (playerMove != null)
            playerMove.SetMovementEnabled(false);
        
        var playerInput = currentPlayer.GetComponent<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = false;
    }
    
    // Cambiar cámara
    if (camaraAscensor != null && camaraJugador != null)
{
    camaraJugador.gameObject.SetActive(false);
    camaraAscensor.gameObject.SetActive(true);
    Debug.Log("📷 Cámara cambiada al ascensor");
}
    
    // Encender luz verde
    if (luzAscensor != null)
    {
        luzAscensor.enabled = true;
        luzAscensor.color = Color.green;
        luzAscensor.intensity = 5f;
    }
    
    yield return new WaitForSeconds(duracionCinematica);
    
    // Volver al jugador
   if (camaraAscensor != null && camaraJugador != null)
{
    camaraJugador.gameObject.SetActive(true);
    camaraAscensor.gameObject.SetActive(false);
    Debug.Log("📷 Cámara regresó al jugador");
}
    
    // 🔥 MOSTRAR EL ARMA NUEVAMENTE
    if (weaponObject != null)
    {
        weaponObject.SetActive(true);
        Debug.Log("🔫 Arma visible nuevamente");
    }

     if (canvasUI != null)
    {
        canvasUI.SetActive(true);
        Debug.Log("🖥️ Canvas visible nuevamente");
    }
    
    // Reactivar movimiento
    if (currentPlayer != null)
    {
        var playerMove = currentPlayer.GetComponent<PlayerMove>();
        if (playerMove != null)
            playerMove.SetMovementEnabled(true);
        
        var playerInput = currentPlayer.GetComponent<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = true;
    }
    
    enCinematica = false;
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
        
        transform.position = startPosition + Vector3.up * (progress * 1.5f);
        
        if (textMesh != null)
        {
            Color color = textMesh.color;
            color.a = Mathf.Lerp(1f, 0f, progress);
            textMesh.color = color;
        }
    }
}
