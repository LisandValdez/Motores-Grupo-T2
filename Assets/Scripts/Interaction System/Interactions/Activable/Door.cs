using UnityEngine;

public class Door : ActivableBase
{
    public GameObject door;
    public Vector3 openRotationOffset = new Vector3(0, 90, 0); // El desfase (90 grados)

    [Header("Configuración de Llaves (Arrastra el Item aquí)")]
    [Tooltip("Arrastra aquí el Prefab o el GameObject de la llave que abre esta puerta.")]
    [SerializeField] private ItemPickup requiredKeyItem;

    // Variables ocultas que se rellenarán solas en el Start
    private string requiredKeyId = "";
    private string keyName = "";
    private bool isLocked = false;

    private bool playerInsideTrigger = false;

    // Variables para guardar las rotaciones calculadas dinámicamente
    private Vector3 baseRotation;
    private Vector3 calculatedOpenRotation;

    private void Start()
    {
        if (door != null)
        {
            // Guardamos la rotación exacta que tiene la puerta colocada en el inspector
            baseRotation = door.transform.localEulerAngles;

            // Calculamos la rotación abierta sumando el offset a su base original
            calculatedOpenRotation = baseRotation + openRotationOffset;
        }

        // VINCULACIÓN AUTOMÁTICA DE LA LLAVE
        if (requiredKeyItem != null)
        {
            isLocked = true; // Si hay una llave asignada, la puerta empieza cerrada
            requiredKeyId = requiredKeyItem.keyId;
            keyName = requiredKeyItem.itemName;
            Debug.Log($"🔑 Puerta vinculada exitosamente. Requiere: {keyName} (ID: {requiredKeyId})");
        }
        else
        {
            isLocked = false; // Si no arrastraste ninguna llave, la puerta se puede abrir libremente
        }
    }

    // Método obligatorio de ActivableBase (Se ejecuta al usar la E o el Raycast)
    protected override void Activate()
    {
        // 🚨 BLOQUEO DE PRÓLOGO: Si no se ha leído la nota, la puerta no se puede usar
        if (!IntroNote.HasReadIntroNote)
        {
            TriggerIntroBlockDialogue();
            return;
        }

        if (isLocked)
        {
            // Comprobamos si el jugador tiene la llave correcta en el inventario del grupo
            if (Inventory.Instance != null && Inventory.Instance.HasKey(requiredKeyId))
            {
                isLocked = false;
                Debug.Log("🔓 Puerta desbloqueada con la llave correcta.");

                // Si el jugador ya estaba dentro del trigger cuando la desbloqueó, se abre inmediatamente
                if (playerInsideTrigger)
                {
                    Open();
                }
            }
            else
            {
                Debug.LogWarning("🔑 La puerta está cerrada. Necesitas la llave: " + requiredKeyId);

                // 💬 Disparar diálogo dinámico usando el nombre extraído automáticamente
                TriggerLockedDoorDialogue();
            }
        }
    }

    void TriggerLockedDoorDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            Dialogue lockedDialogue = new Dialogue();
            lockedDialogue.lines = new DialogueLine[1];

            // 👤 ASIGNAMOS EL NOMBRE DEL PROTAGONISTA
            lockedDialogue.lines[0].characterName = "Damian";

            // Si por alguna razón no hay nombre asignado, usamos un respaldo genérico
            string finalKeyName = !string.IsNullOrEmpty(keyName) ? keyName : "una llave";
            lockedDialogue.lines[0].text = $"Está cerrada... Necesito la <color=#00FFFF>{finalKeyName}</color>.";

            DialogueManager.Instance.StartDialogue(lockedDialogue);
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró DialogueManager para mostrar el texto de la puerta cerrada.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Npc"))
        {
            playerInsideTrigger = true;

            // 🚨 BLOQUEO DE PRÓLOGO: Si se acerca caminando, se le niega el paso automático
            if (!IntroNote.HasReadIntroNote)
            {
                if (other.CompareTag("Player"))
                {
                    TriggerIntroBlockDialogue();
                }
                return;
            }

            // Solo se abre si está desbloqueada
            if (!isLocked)
            {
                Open();
            }
        }
    }

    void Open()
    {
        if (door != null)
        {
            door.transform.localEulerAngles = calculatedOpenRotation;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Npc"))
        {
            playerInsideTrigger = false;
            Close();
        }
    }

    void Close()
    {
        if (door != null)
        {
            door.transform.localEulerAngles = baseRotation;
        }
    }

    // Muestra el recordatorio narrativo en el DialogueManager
    private void TriggerIntroBlockDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            Dialogue blockDialogue = new Dialogue();
            blockDialogue.lines = new DialogueLine[1];

            // 👤 TAMBIÉN LE ASIGNAMOS EL NOMBRE AL RECORDATORIO DE LA NOTA
            blockDialogue.lines[0].characterName = "Damian";
            blockDialogue.lines[0].text = "Debería leer la nota que hay en mi mesa...";

            DialogueManager.Instance.StartDialogue(blockDialogue);
        }
    }
}