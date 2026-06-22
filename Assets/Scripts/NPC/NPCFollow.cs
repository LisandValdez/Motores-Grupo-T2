using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
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

    private GameObject currentPrompt;
    private GameObject currentWorldMessage;


    private Rigidbody rb;
    private PlayerMove playerMoveScript;

    void Start()
    {
        rb = GetComponent<Rigidbody>();


        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;


        rb.isKinematic = false;


        if (player == null)
        {
            player = FindFirstObjectByType<PlayerMove>()?.gameObject;
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player != null)
        {
            playerMoveScript = player.GetComponent<PlayerMove>();
        }

        CreateInteractionPrompt();

        if (interactionPanel != null)
            interactionPanel.SetActive(false);
    }

    void Update()
    {
   
        if (player != null && currentPrompt != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            bool isInRange = distanceToPlayer <= interactionRange;
            currentPrompt.SetActive(isInRange);
        }
    }

    void FixedUpdate()
    {
        if (isFollowing && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance > teleportDistance)
            {
                TeleportToPlayer();
                return;
            }

            if (distance > stopDistance)
            {
                
                Vector3 direction = (player.transform.position - transform.position);
                direction.y = 0;
                direction.Normalize();

                
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f));
                }

               
                float currentSpeed = walkSpeed;

                
                if (playerMoveScript != null && playerMoveScript.IsRunning && playerMoveScript.IsMoving)
                {
                    currentSpeed = runSpeed;
                }

                Vector3 targetVelocity = direction * currentSpeed;
                targetVelocity.y = rb.linearVelocity.y;

                rb.linearVelocity = targetVelocity;
            }
            else
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
        else
        {
            if (rb.linearVelocity.x != 0 || rb.linearVelocity.z != 0)
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
    }

    void TeleportToPlayer()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * teleportRadius;
        Vector3 targetPosition = player.transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        targetPosition.y = player.transform.position.y;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = targetPosition;

        Debug.Log("NPC rescatado: Teletransportado cerca del jugador porque se quedó atrás.");
    }

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
        ShowWorldMessage(isFollowing ? "Te sigo" : " Me quedo",
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

        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.transform.position, teleportDistance);
        }
    }
}