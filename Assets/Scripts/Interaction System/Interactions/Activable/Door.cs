using UnityEngine;

public class Door : ActivableBase
{
    public GameObject door;
    public Vector3 openRotationOffset = new Vector3(0, 90, 0); // El desfase (90 grados)

    [Header("Configuración de Llaves")]
    [SerializeField] private bool isLocked = false;
    [SerializeField] private string requiredKeyId = "";

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
    }

    // Método obligatorio de ActivableBase (Se ejecuta al usar la E o el Raycast)
    protected override void Activate()
    {
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
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Npc"))
        {
            playerInsideTrigger = true;

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
            // Va a la rotación base original + los 90 grados configurados
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
            // Vuelve exactamente a su rotación original del inspector, sin importar cuál era
            door.transform.localEulerAngles = baseRotation;
        }
    }
}