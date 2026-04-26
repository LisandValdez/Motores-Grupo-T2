using UnityEngine;
using UnityEngine.InputSystem;

public class VictoryButton : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    public string requiredFuseBoxName = "FuseBox";  // Nombre de la caja de fusibles
    public bool requireFuseBoxCompletion = true;     // Si requiere que la caja esté completada
    
    [Header("Mensajes")]
    [TextArea(3, 5)]
    public string victoryMessage = "🎉 ¡VICTORIA! ¡Has restaurado la energía! 🎉";
    
    [TextArea(3, 5)]
    public string noPowerMessage = "🔌 No hay energía. Necesito colocar el fusible primero.";
    
    [TextArea(3, 5)]
    public string alreadyVictoryMessage = "El sistema ya está activado. ¡Victoria!";
    
    [Header("Efectos")]
    public GameObject victoryEffect;      // Efecto de victoria (partículas, luz)
    public AudioClip victorySound;        // Sonido de victoria
    public AudioClip errorSound;          // Sonido de error
    
    [Header("Visual")]
    public float interactionRange = 3f;
    public float promptHeight = 1.5f;
    
    [Header("Estado")]
    public bool isVictoryAchieved = false;  // Si ya se ganó
    
    private GameObject currentPrompt;
    private GameObject currentPlayer;
    private bool playerInRange = false;
    private FuseBox fuseBox;  // Referencia a la caja de fusibles

    void Start()
    {
        // Buscar la caja de fusibles
        FindFuseBox();
        CreateInteractionPrompt();
    }

    void Update()
    {
        if (currentPrompt != null && currentPrompt.activeSelf != playerInRange)
        {
            currentPrompt.SetActive(playerInRange);
        }
    }

    void FindFuseBox()
    {
        // Buscar la caja de fusibles por nombre
        GameObject fuseBoxObj = GameObject.Find(requiredFuseBoxName);
        
        if (fuseBoxObj == null)
        {
            // Si no la encuentra por nombre, buscar por componente
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
        
        // Verificar si la caja de fusibles está completada
        bool hasPower = CheckPowerStatus();
        
        if (hasPower)
        {
            // VICTORIA
            isVictoryAchieved = true;
            ShowVictory();
        }
        else
        {
            // SIN ENERGÍA
            ShowMessage(noPowerMessage, Color.red);
            
            if (errorSound != null)
                AudioSource.PlayClipAtPoint(errorSound, transform.position, 1f);
                
            Debug.Log("🔌 Botón presionado pero NO hay energía");
        }
    }
    
    bool CheckPowerStatus()
    {
        if (fuseBox != null)
        {
            return fuseBox.isCompleted;
        }
        
        // Si no hay caja de fusibles, verificar por etiqueta o nombre
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
        
        // Mostrar mensaje de victoria
        ShowMessage(victoryMessage, Color.green);
        
        // Efectos visuales
        if (victoryEffect != null)
            Instantiate(victoryEffect, transform.position, Quaternion.identity);
        
        // Sonido de victoria
        if (victorySound != null)
            AudioSource.PlayClipAtPoint(victorySound, transform.position, 1f);
        
        // Aquí puedes agregar la lógica de final del juego
        EndGame();
    }
    
    void EndGame()
    {
        // Opción 1: Mostrar panel de victoria y pausar
        Debug.Log("🏆 JUEGO COMPLETADO 🏆");
        
        // Opción 2: Cargar escena de victoria
        // SceneManager.LoadScene("VictoryScene");
        
        // Opción 3: Mostrar mensaje y desactivar controles
        // Time.timeScale = 0f;
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
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