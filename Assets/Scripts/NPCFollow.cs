using UnityEngine;
using UnityEngine.UI;

public class NPCFollow : MonoBehaviour, IInteractable  // ← Implementar interfaz
{
    public GameObject player;
    public float followSpeed = 3f;
    public float stopDistance = 2f;
    public float interactionRange = 2.5f;
    public bool isFollowing = false;
    
    public GameObject interactionPanel;
    public Text interactionText;
    
    private GameObject currentPrompt;
    private GameObject currentWorldMessage;

    void Start()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerMovement>()?.gameObject;
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player");
        }
        
        CreateInteractionPrompt();
        
        if (interactionPanel != null)
            interactionPanel.SetActive(false);
    }

    void Update()
    {
        if (isFollowing && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance > stopDistance)
            {
                Vector3 direction = (player.transform.position - transform.position).normalized;
                direction.y = 0;
                transform.position += direction * followSpeed * Time.deltaTime;
            }
        }
        
        // Mostrar/ocultar prompt según distancia (opcional)
        if (player != null && currentPrompt != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            bool isInRange = distanceToPlayer <= interactionRange;
            currentPrompt.SetActive(isInRange);
        }
    }

    void CreateInteractionPrompt()
    {
        GameObject promptObj = new GameObject("InteractionPrompt");
        promptObj.transform.SetParent(transform);
        promptObj.transform.localPosition = new Vector3(0, 2f, 0);
        
        TextMesh textMesh = promptObj.AddComponent<TextMesh>();
        textMesh.text = $"<color=yellow>E</color> - { (isFollowing ? "Detener" : "Seguir") }";
        textMesh.fontSize = 30;
        textMesh.characterSize = 0.03f;
        textMesh.color = Color.white;
        textMesh.alignment = TextAlignment.Center;
        
        promptObj.AddComponent<Billboard>();
        currentPrompt = promptObj;
        promptObj.SetActive(false);
    }
    
    void UpdatePromptText()
    {
        if (currentPrompt != null)
        {
            TextMesh textMesh = currentPrompt.GetComponent<TextMesh>();
            if (textMesh != null)
            {
                textMesh.text = $"<color=yellow>E</color> - { (isFollowing ? "Detener" : "Seguir") }";
            }
        }
    }
    
    // ✅ INTERACCIÓN (llamada por PlayerInteraction)
    public void Interact()
    {
        isFollowing = !isFollowing;
        UpdatePromptText();
        ShowWorldMessage(isFollowing ? "✨ Te sigo" : "🛑 Me quedo", 
                        isFollowing ? Color.green : Color.yellow);
        Debug.Log(isFollowing ? "NPC: ¡Te sigo!" : "NPC: Me quedo aquí");
    }
    
    void ShowWorldMessage(string message, Color color)
    {
        if (currentWorldMessage != null)
            Destroy(currentWorldMessage);
        
        GameObject messageObj = new GameObject("FloatingMessage");
        messageObj.transform.SetParent(transform);
        messageObj.transform.localPosition = new Vector3(0, 2.5f, 0);
        
        TextMesh textMesh = messageObj.AddComponent<TextMesh>();
        textMesh.text = message;
        textMesh.fontSize = 40;
        textMesh.characterSize = 0.05f;
        textMesh.color = color;
        textMesh.fontStyle = FontStyle.Bold;
        textMesh.alignment = TextAlignment.Center;
        
        messageObj.AddComponent<PickupMessageAnimator>();
        currentWorldMessage = messageObj;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}