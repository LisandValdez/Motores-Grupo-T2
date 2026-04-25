using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NPCFollow : MonoBehaviour
{
    public GameObject player;
    public float followSpeed = 3f;
    public float stopDistance = 2f;
    public float interactionRange = 2.5f;
    public bool isFollowing = false;

    private bool playerInRange = false;
    private GameObject currentPrompt;
    private TextMesh promptText;

    void Start()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerMovement>()?.gameObject;
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player");
        }
        
        CreateInteractionPrompt();
        
    }

   void Update()
{
    // 🔹 Interacción con tecla E
    if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
    {
        isFollowing = !isFollowing;
    }

    // 🔹 Movimiento del NPC
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

    // 🔹 Mostrar prompt
    if (player != null)
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        playerInRange = distance <= interactionRange;

        if (currentPrompt != null)
            currentPrompt.SetActive(playerInRange);
    }
    if (promptText != null)
{
    if (isFollowing)
        promptText.text = "<color=yellow>E</color> dejar de seguir";
    else
        promptText.text = "<color=yellow>E</color> seguir";
}
}
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            playerInRange = true;
            if (currentPrompt != null)
                currentPrompt.SetActive(true);
        }
    }

    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            playerInRange = false;
            if (currentPrompt != null)
                currentPrompt.SetActive(false);
        }
    }

    void CreateInteractionPrompt()
{
    GameObject promptObj = new GameObject("InteractionPrompt");
    promptObj.transform.SetParent(transform);
    promptObj.transform.localPosition = new Vector3(0, 2f, 0);
    
    promptText = promptObj.AddComponent<TextMesh>();

    promptText.text = " <color=yellow>E</color> seguir";
    promptText.fontSize = 30;
    promptText.characterSize = 0.03f;
    promptText.color = Color.cyan;
    promptText.alignment = TextAlignment.Center;

    promptObj.AddComponent<Billboard>();
    currentPrompt = promptObj;
    promptObj.SetActive(false);
    
    Debug.Log("✅ Prompt del NPC creado correctamente");
}
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
        
        if (isFollowing && player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.transform.position);
        }
    }
}