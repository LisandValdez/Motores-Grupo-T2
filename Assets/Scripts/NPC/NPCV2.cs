using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI; // 1. REQUISITO: Importar la librería de IA

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))] // 2. Asegura que tenga el NavMeshAgent
public class NPCV2 : MonoBehaviour, IInteractable
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

    private Rigidbody rb;
    private NavMeshAgent agent; // 3. Componente para la IA de movimiento

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        // Configuración del Rigidbody para que no interfiera negativamente con el agente
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.isKinematic = true; // RECOMENDACIÓN: El NavMeshAgent funciona mejor si el RB es kinematic

        // Configuración inicial del NavMeshAgent según tus variables
        agent.speed = followSpeed;
        agent.stoppingDistance = stopDistance;

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
        // Mostrar/ocultar prompt según distancia
        if (player != null && currentPrompt != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            bool isInRange = distanceToPlayer <= interactionRange;
            currentPrompt.SetActive(isInRange);
        }

        // LÓGICA DE MOVIMIENTO DE IA: El NavMesh se actualiza en el Update normal de Unity
        if (isFollowing && player != null)
        {
            // Habilitamos el agente si estaba apagado
            if (!agent.enabled) agent.enabled = true;

            // Le decimos al agente que su destino es la posición del jugador
            // El NavMesh automáticamente buscará la puerta o salida si está en otra habitación
            agent.SetDestination(player.transform.position);
        }
        else
        {
            // Si no debe seguir, detenemos el agente
            if (agent.enabled)
            {
                agent.ResetPath();
                agent.enabled = false;
            }
        }
    }

    // Removimos el FixedUpdate porque el NavMeshAgent se encarga de la física y la rotación por sí solo.

    void CreateInteractionPrompt()
    {
        GameObject promptObj = new GameObject("InteractionPrompt");
        promptObj.transform.SetParent(transform);
        promptObj.transform.localPosition = new Vector3(0, 2.0f, 0);

        TextMesh textMesh = promptObj.AddComponent<TextMesh>();
        textMesh.text = $"<color=yellow>E</color> - {(isFollowing ? "Detener" : "Seguir")}";
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
                textMesh.text = $"<color=yellow>E</color> - {(isFollowing ? "Detener" : "Seguir")}";
            }
        }
    }

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