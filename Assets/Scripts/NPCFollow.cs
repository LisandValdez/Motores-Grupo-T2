using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))] // Asegura que el NPC tenga un Rigidbody
public class NPCFollow : MonoBehaviour, IInteractable
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

    // Variables para la física
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 1. CONFIGURACIÓN CRÍTICA POR CÓDIGO: 
        // Congelamos las rotaciones en X y Z para que el NPC NUNCA se caiga de cabeza o de lado.
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Asegúrate de que isKinematic esté en FALSE en el inspector para que choque con las paredes.
        rb.isKinematic = false;

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

    // Dejamos el Update solo para la lógica visual y detección de distancia del prompt
    void Update()
    {
        // Mostrar/ocultar prompt según distancia
        if (player != null && currentPrompt != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            bool isInRange = distanceToPlayer <= interactionRange;
            currentPrompt.SetActive(isInRange);
        }
    }

    // Todo lo que sea movimiento físico DEBE ir en FixedUpdate
    void FixedUpdate()
    {
        if (isFollowing && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance > stopDistance)
            {
                // Calcular dirección ignorando el eje Y (para que no intente volar hacia el jugador)
                Vector3 direction = (player.transform.position - transform.position);
                direction.y = 0;
                direction.Normalize();

                // Rotar suavemente hacia el jugador
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f));
                }

                // Mover al NPC aplicando velocidad al Rigidbody.
                // Mantenemos la rb.velocity.y actual para que la gravedad normal siga funcionando.
                Vector3 targetVelocity = direction * followSpeed;
                targetVelocity.y = rb.linearVelocity.y;

                rb.linearVelocity = targetVelocity;
            }
            else
            {
                // Si está cerca, frenamos el movimiento horizontal pero dejamos que actúe la gravedad
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
        else
        {
            // Si no está siguiendo, que se quede quieto (respetando la gravedad)
            if (rb.linearVelocity.x != 0 || rb.linearVelocity.z != 0)
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
    }

    void CreateInteractionPrompt()
    {
        GameObject promptObj = new GameObject("InteractionPrompt");
        promptObj.transform.SetParent(transform);
        promptObj.transform.localPosition = new Vector3(0, 2.0f, 0); // Lo subí un poco para que no spawnee en el suelo

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