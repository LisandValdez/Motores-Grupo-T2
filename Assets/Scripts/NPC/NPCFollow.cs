using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))] // Asegura que el NPC tenga un Rigidbody
public class NPCFollow : MonoBehaviour, IInteractable
{
    [Header("Referencias")]
    public GameObject player;
    public GameObject interactionPanel;
    public Text interactionText;

    [Header("Configuración de Velocidad")]
    [Tooltip("Velocidad del NPC cuando el jugador camina.")]
    public float walkSpeed = 3f;
    [Tooltip("Velocidad del NPC cuando el jugador corre.")]
    public float runSpeed = 7f;
    [Tooltip("Distancia a la que el NPC se detendrá respecto al jugador.")]
    public float stopDistance = 2f;
    [Tooltip("Distancia máxima para poder interactuar con el NPC.")]
    public float interactionRange = 2.5f;

    [Header("Estado")]
    public bool isFollowing = false;

    [Header("Configuración de Teletransporte")]
    [Tooltip("Distancia máxima a la que puede alejarse el jugador antes de que el NPC se teletransporte.")]
    public float teleportDistance = 12f;
    [Tooltip("Radio alrededor del jugador donde puede aparecer el NPC al teletransportarse.")]
    public float teleportRadius = 1.5f;

    // Variables privadas para el sistema de interacción visual
    private GameObject currentPrompt;
    private GameObject currentWorldMessage;

    // Componentes cacheados
    private Rigidbody rb;
    private PlayerMove playerMoveScript;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // CONFIGURACIÓN CRÍTICA POR CÓDIGO: 
        // Congelamos las rotaciones en X y Z para que el NPC NUNCA se caiga de cabeza o de lado.
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Asegúrate de que isKinematic esté en FALSE en el inspector para que choque con las paredes.
        rb.isKinematic = false;

        // Intentar buscar al jugador dinámicamente si no está asignado en el inspector
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerMove>()?.gameObject;
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player");
        }

        // Cachear el componente de movimiento del jugador para optimizar rendimiento
        if (player != null)
        {
            playerMoveScript = player.GetComponent<PlayerMove>();
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

            // --- 1. LÓGICA DE TELETRANSPORTE ---
            if (distance > teleportDistance)
            {
                TeleportToPlayer();
                return; // Salimos del FixedUpdate en este frame para evitar cálculos de movimiento erráticos
            }

            // --- 2. LÓGICA DE SEGUIMIENTO ---
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

                // --- 3. DETERMINAR VELOCIDAD DINÁMICA ---
                float currentSpeed = walkSpeed;

                // Si el jugador está corriendo Y se está moviendo realmente, el NPC acelera
                if (playerMoveScript != null && playerMoveScript.IsRunning && playerMoveScript.IsMoving)
                {
                    currentSpeed = runSpeed;
                }

                // Mover al NPC aplicando velocidad al Rigidbody.
                // Mantenemos la rb.linearVelocity.y actual para que la gravedad normal siga funcionando.
                Vector3 targetVelocity = direction * currentSpeed;
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

    void TeleportToPlayer()
    {
        // Calculamos una posición aleatoria en un círculo horizontal alrededor del jugador
        Vector2 randomCircle = Random.insideUnitCircle.normalized * teleportRadius;
        Vector3 targetPosition = player.transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        // Ajustamos la altura a la del jugador para evitar que aparezca flotando o enterrado
        targetPosition.y = player.transform.position.y;

        // Limpiamos inercias para evitar que el NPC salga disparado con fuerzas acumuladas
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Modificamos rb.position directamente (mejor práctica para objetos con física activa)
        rb.position = targetPosition;

        Debug.Log("NPC rescatado: Teletransportado cerca del jugador porque se quedó atrás.");
    }

    void CreateInteractionPrompt()
    {
        GameObject promptObj = new GameObject("InteractionPrompt");
        promptObj.transform.SetParent(transform);
        promptObj.transform.localPosition = new Vector3(0, 2.0f, 0); // Altura sobre el NPC

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
        // Rango de interacción (Amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        // Rango límite de Teletransporte (Rojo) dibujado desde el jugador
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.transform.position, teleportDistance);
        }
    }
}