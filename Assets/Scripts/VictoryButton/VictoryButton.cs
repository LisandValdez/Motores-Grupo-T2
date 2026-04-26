using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class VictoryButton : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    public string requiredFuseBoxName = "FuseBox";
    public bool requireFuseBoxCompletion = true;
    
    [Header("Movimiento del Botón")]
    public float pressedPositionX = -96.30f;   // Posición X cuando está presionado
    public float releasedPositionX = -96f;     // Posición X cuando está en reposo
    public float moveSpeed = 5f;               // Velocidad de movimiento
    
    [Header("Mensajes")]
    [TextArea(3, 5)]
    public string victoryMessage = "🎉 ¡VICTORIA! ¡Has restaurado la energía! 🎉";
    
    [TextArea(3, 5)]
    public string noPowerMessage = "🔌 No hay energía. Necesito colocar el fusible primero.";
    
    [TextArea(3, 5)]
    public string alreadyVictoryMessage = "El sistema ya está activado. ¡Victoria!";
    
    [Header("Efectos")]
    public GameObject victoryEffect;
    public AudioClip victorySound;
    public AudioClip errorSound;
    public AudioClip pressSound;
    
    [Header("Visual")]
    public float interactionRange = 3f;
    public float promptHeight = 1.5f;
    
    [Header("Estado")]
    public bool isVictoryAchieved = false;
    
    private GameObject currentPrompt;
    private GameObject currentPlayer;
    private bool playerInRange = false;
    private FuseBox fuseBox;
    
    // Variables para el movimiento
    private bool isPressed = false;
    private bool isMoving = false;
    private Vector3 targetPosition;

    void Start()
    {
        // Asegurar posición inicial
        Vector3 pos = transform.localPosition;
        pos.x = releasedPositionX;
        transform.localPosition = pos;
        targetPosition = transform.localPosition;
        
        FindFuseBox();
        CreateInteractionPrompt();
    }

    void Update()
    {
        if (currentPrompt != null && currentPrompt.activeSelf != playerInRange)
        {
            currentPrompt.SetActive(playerInRange);
        }
        
        // Movimiento suave del botón
        if (isMoving)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * moveSpeed);
            
            if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01f)
            {
                isMoving = false;
            }
        }
    }

    void FindFuseBox()
    {
        GameObject fuseBoxObj = GameObject.Find(requiredFuseBoxName);
        
        if (fuseBoxObj == null)
        {
            fuseBox = FindFirstObjectByType<FuseBox>();
            if (fuseBox != null)
                Debug.Log($"✅ Caja de fusibles encontrada: {fuseBox.gameObject.name}");
        }
        else
        {
            fuseBox = fuseBoxObj.GetComponent<FuseBox>();
            if (fuseBox != null)
                Debug.Log($"✅ Caja de fusibles encontrada: {fuseBoxObj.name}");
        }
        
        if (fuseBox == null)
            Debug.LogWarning("⚠️ No se encontró la caja de fusibles en la escena!");
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
        textMesh.text = $"Presiona <color=yellow>E</color> para activar\n<color=cyan>🔘 Botón de Energía</color>";
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
        
        if (isVictoryAchieved)
        {
            ShowMessage(alreadyVictoryMessage, Color.gray);
            return;
        }
        
        if (isPressed || isMoving)
        {
            Debug.Log("⏰ El botón ya está siendo usado");
            return;
        }
        
        // Verificar si la caja de fusibles está completada
        bool hasPower = CheckPowerStatus();
        
        if (hasPower)
        {
            // VICTORIA - El botón se queda presionado
            isVictoryAchieved = true;
            PressButton(true);
            ShowVictory();
        }
        else
        {
            // SIN ENERGÍA - Solo animación de error (presiona y vuelve rápido)
            Debug.Log("🔌 Botón presionado pero NO hay energía");
            ShowMessage(noPowerMessage, Color.red);
            
            if (errorSound != null)
                AudioSource.PlayClipAtPoint(errorSound, transform.position, 1f);
            
            StartCoroutine(ErrorPress());
        }
    }
    
    void PressButton(bool permanent)
    {
        isPressed = true;
        isMoving = true;
        targetPosition = new Vector3(pressedPositionX, transform.localPosition.y, transform.localPosition.z);
        
        if (pressSound != null)
            AudioSource.PlayClipAtPoint(pressSound, transform.position, 1f);
        
        Debug.Log($"🔘 Botón presionado: X = {pressedPositionX}");
        
        if (!permanent)
        {
            StartCoroutine(ReturnButton());
        }
    }
    
    IEnumerator ReturnButton()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (!isVictoryAchieved)
        {
            isMoving = true;
            targetPosition = new Vector3(releasedPositionX, transform.localPosition.y, transform.localPosition.z);
            isPressed = false;
            Debug.Log($"🔘 Botón liberado: X = {releasedPositionX}");
        }
    }
    
    IEnumerator ErrorPress()
    {
        // Presionar rápidamente
        isMoving = true;
        targetPosition = new Vector3(pressedPositionX, transform.localPosition.y, transform.localPosition.z);
        
        yield return new WaitForSeconds(0.15f);
        
        // Volver inmediatamente
        isMoving = true;
        targetPosition = new Vector3(releasedPositionX, transform.localPosition.y, transform.localPosition.z);
        
        yield return new WaitForSeconds(0.1f);
        isPressed = false;
    }
    
    bool CheckPowerStatus()
    {
        if (fuseBox != null)
        {
            return fuseBox.isCompleted;
        }
        
        GameObject fuseBoxObj = GameObject.FindGameObjectWithTag("FuseBox");
        if (fuseBoxObj != null)
        {
            FuseBox fb = fuseBoxObj.GetComponent<FuseBox>();
            if (fb != null)
                return fb.isCompleted;
        }
        
        return false;
    }
    
    void ShowVictory()
    {
        Debug.Log("🎉 ¡VICTORIA! El botón fue presionado con energía.");
        
        ShowMessage(victoryMessage, Color.green);
        
        if (victoryEffect != null)
            Instantiate(victoryEffect, transform.position, Quaternion.identity);
        
        if (victorySound != null)
            AudioSource.PlayClipAtPoint(victorySound, transform.position, 1f);
        
        EndGame();
    }
    
    void EndGame()
    {
        Debug.Log("🏆 JUEGO COMPLETADO 🏆");
        
        // Opcional: Mostrar cursor y pausar
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
        // Time.timeScale = 0f;
        
        // Opcional: Cargar escena de victoria
        // SceneManager.LoadScene("VictoryScene");
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
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}