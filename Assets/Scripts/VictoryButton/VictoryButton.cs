using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class VictoryButton : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    public ElevatorDoor elevatorDoor;
    public VictoryManager victoryManager;
    
    [Header("Movimiento del Botón (Eje Z)")]
    public float pressedPositionZ;
    public float releasedPositionZ;
    public float moveSpeed = 5f;
    
    [Header("Mensajes")]
    [TextArea(3, 5)]
    public string victoryMessage;
    [TextArea(3, 5)]
    public string alreadyVictoryMessage;
    [TextArea(3, 5)]
    public string closingDoorMessage;
    
    [Header("Efectos")]
    public GameObject victoryEffect;
    public AudioClip victorySound;
    public AudioClip pressSound;
    public AudioClip doorCloseSound;
    
    [Header("Visual")]
    public float interactionRange = 3f;
    public float promptHeight = 1.5f;
    
    [Header("Estado")]
    public bool isVictoryAchieved = false;
    
    private GameObject currentPrompt;
    private GameObject currentPlayer;
    private bool playerInRange = false;
    private bool isPressed = false;
    private bool isMoving = false;
    private Vector3 targetPosition;

    void Start()
    {
        Vector3 pos = transform.localPosition;
        pos.z = releasedPositionZ;
        transform.localPosition = pos;
        targetPosition = transform.localPosition;
        
        if (victoryManager == null)
            victoryManager = FindFirstObjectByType<VictoryManager>();
        
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
        textMesh.text = $"Presiona <color=yellow>E</color> para activar\n<color=red>🔴 Botón de Emergencia</color>";
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
            Debug.Log("No estás en rango del botón");
            return;
        }
        
        if (isVictoryAchieved)
        {
            ShowMessage(alreadyVictoryMessage, Color.gray);
            return;
        }
        
        if (isPressed || isMoving)
        {
            Debug.Log("El botón ya está siendo usado");
            return;
        }
        
        isVictoryAchieved = true;
        PressButton(true);
        StartCoroutine(VictorySequence());
    }
    
    void PressButton(bool permanent)
    {
        isPressed = true;
        isMoving = true;
        targetPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, pressedPositionZ);
        
        if (pressSound != null)
            AudioSource.PlayClipAtPoint(pressSound, transform.position, 1f);
        
        Debug.Log($"🔘 Botón presionado: Z = {pressedPositionZ}");
    }
    
    IEnumerator VictorySequence()
    {
        Debug.Log("Iniciando secuencia de victoria");
        
        if (elevatorDoor != null)
        {
            Debug.Log("🚪 Cerrando puerta...");
            ShowMessage(closingDoorMessage, Color.yellow);
            
            elevatorDoor.CloseDoor();
            
            if (doorCloseSound != null)
                AudioSource.PlayClipAtPoint(doorCloseSound, transform.position, 1f);
        
            yield return StartCoroutine(WaitForDoorToClose());
        }
        
        yield return new WaitForSeconds(0.2f);
        
        Debug.Log("¡VICTORIA!");
        
        if (victoryEffect != null)
            Instantiate(victoryEffect, transform.position, Quaternion.identity);
        
        if (victorySound != null)
            AudioSource.PlayClipAtPoint(victorySound, transform.position, 1f);
        
        ShowMessage(victoryMessage, Color.green);
        
        // 4. PAUSA CORTA para ver el efecto
        yield return new WaitForSeconds(1f);
        
        // 5. FINALIZAR EL JUEGO
        EndGame();
    }
    
    IEnumerator WaitForDoorToClose()
    {
        // Esperar mientras la puerta se está moviendo
        float maxWaitTime = 3f;
        float elapsedTime = 0f;
        
        while (elevatorDoor != null && elevatorDoor.isMoving && elapsedTime < maxWaitTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Si la puerta sigue abierta después del tiempo máximo, forzar cierre
        if (elevatorDoor != null && elevatorDoor.isOpen)
        {
            Debug.LogWarning("Forzando cierre de puerta");
            elevatorDoor.ForceClose();
        }
        
        yield return null;
    }
    
    void EndGame()
{
    Debug.Log("JUEGO COMPLETADO");
    
    if (victoryManager != null)
    {
        victoryManager.Victory();
        
        StartCoroutine(PauseAfterVictory());
    }
    else
    {
        Debug.LogError("No se encontró VictoryManager!");
    }
}

IEnumerator PauseAfterVictory()
{
    yield return null;
    
    Time.timeScale = 0f;
    Debug.Log("Juego pausado");
}

    
    void ShowMessage(string message, Color color)
{
    FadeMessage[] oldMessages = FindObjectsOfType<FadeMessage>();
    foreach (FadeMessage msg in oldMessages)
    {
        Destroy(msg.gameObject);
    }
    
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
    fade.lifetime = 2.5f;
    
    Destroy(messageObj, 2.5f);
}
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}