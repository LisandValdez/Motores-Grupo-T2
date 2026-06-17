using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ElevatorButton : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    public ElevatorDoor elevatorDoor;  // Referencia a la puerta
    public FuseBox fuseBox;            // Referencia a la caja de fusibles
    public bool requireFuseBoxCompletion = true;
    
    [Header("Movimiento del Botón")]
    public float pressedPositionX = -96.30f;
    public float releasedPositionX = -96f;
    public float moveSpeed = 5f;
    
    [Header("Mensajes")]
    [TextArea(3, 5)]
    public string noPowerMessage = "🔌 No hay energía. Necesito colocar el fusible primero.";
    [TextArea(3, 5)]
    public string alreadyOpenMessage = "🚪 La puerta ya está abierta.";
    [TextArea(3, 5)]
    public string successMessage = "🔓 ¡Puerta abierta!";
    
    [Header("Efectos")]
    public AudioClip pressSound;
    public AudioClip errorSound;
    
    [Header("Visual")]
    public float interactionRange = 3f;
    public float promptHeight = 1.5f;
    
    [Header("Estado")]
    public bool isDoorOpen = false;
    
    private GameObject currentPrompt;
    private GameObject currentPlayer;
    private bool playerInRange = false;
    private bool isPressed = false;
    private bool isMoving = false;
    private Vector3 targetPosition;

    void Start()
    {
        Vector3 pos = transform.localPosition;
        pos.x = releasedPositionX;
        transform.localPosition = pos;
        targetPosition = transform.localPosition;
        
        // Buscar FuseBox automáticamente si no está asignado
        if (fuseBox == null)
            fuseBox = FindFirstObjectByType<FuseBox>();
        
        CreateInteractionPrompt();
    }
    
    void Update()
    {
        if (currentPrompt != null && currentPrompt.activeSelf != playerInRange)
        {
            currentPrompt.SetActive(playerInRange);
        }
        
        if (isMoving)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * moveSpeed);
            
            if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01f)
            {
                isMoving = false;
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
        promptObj.transform.localPosition = new Vector3(0, promptHeight, 0);
        
        TextMesh textMesh = promptObj.AddComponent<TextMesh>();
        textMesh.text = $"Presiona <color=yellow>E</color> para abrir puerta\n<color=cyan>🚪 Botón del Ascensor</color>";
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
            Debug.Log("⚠️ No estás en rango del botón");
            return;
        }
        
        if (isDoorOpen)
        {
            ShowMessage(alreadyOpenMessage, Color.gray);
            return;
        }
        
        if (isPressed || isMoving)
        {
            return;
        }
        
        // Verificar si hay energía
        bool hasPower = true;
        if (requireFuseBoxCompletion && fuseBox != null)
        {
            hasPower = fuseBox.isCompleted;
        }
        
        if (hasPower)
        {
            // Abrir la puerta
            StartCoroutine(PressAndOpenDoor());
        }
        else
        {
            // Sin energía
            ShowMessage(noPowerMessage, Color.red);
            if (errorSound != null)
                AudioSource.PlayClipAtPoint(errorSound, transform.position, 1f);
            StartCoroutine(ErrorPress());
        }
    }
    
    IEnumerator PressAndOpenDoor()
    {
        isPressed = true;
        isMoving = true;
        targetPosition = new Vector3(pressedPositionX, transform.localPosition.y, transform.localPosition.z);
        if (pressSound != null)
            AudioSource.PlayClipAtPoint(pressSound, transform.position, 1f);
        
        yield return new WaitForSeconds(0.3f);
        
        // Soltar botón
        isMoving = true;
        targetPosition = new Vector3(releasedPositionX, transform.localPosition.y, transform.localPosition.z);
        
        // Abrir la puerta
        if (elevatorDoor != null)
        {
            elevatorDoor.OpenDoor();
            ShowMessage(successMessage, Color.green);
            isDoorOpen = true;
            
            // Actualizar prompt
            if (currentPrompt != null)
            {
                TextMesh textMesh = currentPrompt.GetComponent<TextMesh>();
                if (textMesh != null)
                    textMesh.text = $"<color=green>✅ Puerta abierta</color>";
            }
        }
        else
        {
            Debug.LogError("❌ No se asignó la puerta del ascensor!");
        }
        
        yield return new WaitForSeconds(0.2f);
        isPressed = false;
    }
    
    IEnumerator ErrorPress()
    {
        isMoving = true;
        targetPosition = new Vector3(pressedPositionX, transform.localPosition.y, transform.localPosition.z);
        yield return new WaitForSeconds(0.15f);
        
        isMoving = true;
        targetPosition = new Vector3(releasedPositionX, transform.localPosition.y, transform.localPosition.z);
        yield return new WaitForSeconds(0.1f);
        isPressed = false;
    }
    
    void ShowMessage(string message, Color color)
    {
        GameObject messageObj = new GameObject("ButtonMessage");
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
        fade.lifetime = 3f;
        
        Destroy(messageObj, 3f);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}