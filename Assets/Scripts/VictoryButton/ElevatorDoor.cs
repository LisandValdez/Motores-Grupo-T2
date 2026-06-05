using UnityEngine;
using System.Collections;

public class ElevatorDoor : MonoBehaviour
{
    [Header("Configuración de Puerta")]
    public Transform leftDoor;      // Puerta izquierda
    public Transform rightDoor;     // Puerta derecha
    public float openDistance = 1.5f;  // Distancia que se abre cada puerta
    public float openSpeed = 2f;        // Velocidad de apertura
    
    [Header("Posiciones")]
    public Vector3 leftDoorOpenPos;
    public Vector3 rightDoorOpenPos;
    public Vector3 leftDoorClosedPos;
    public Vector3 rightDoorClosedPos;
    
    [Header("Efectos")]
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;
    public ParticleSystem doorEffect;   // Efecto de humo o chispas
    
    [Header("Estado")]
    public bool isOpen = false;
    public bool isMoving = false;
    
    private Vector3 leftTarget;
    private Vector3 rightTarget;
    private AudioSource audioSource;
    
    void Start()
    {
        // Guardar posiciones iniciales (cerradas)
        if (leftDoor != null)
            leftDoorClosedPos = leftDoor.localPosition;
        if (rightDoor != null)
            rightDoorClosedPos = rightDoor.localPosition;
        
        // Calcular posiciones abiertas
        leftDoorOpenPos = leftDoorClosedPos + Vector3.left * openDistance;
        rightDoorOpenPos = rightDoorClosedPos + Vector3.right * openDistance;
        
        // Configurar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (doorOpenSound != null || doorCloseSound != null))
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    void Update()
    {
        if (isMoving)
        {
            // Mover puertas suavemente
            if (leftDoor != null)
                leftDoor.localPosition = Vector3.Lerp(leftDoor.localPosition, leftTarget, Time.deltaTime * openSpeed);
            
            if (rightDoor != null)
                rightDoor.localPosition = Vector3.Lerp(rightDoor.localPosition, rightTarget, Time.deltaTime * openSpeed);
            
            // Verificar si llegó al destino
            float leftDistance = leftDoor != null ? Vector3.Distance(leftDoor.localPosition, leftTarget) : 0;
            float rightDistance = rightDoor != null ? Vector3.Distance(rightDoor.localPosition, rightTarget) : 0;
            
            if (leftDistance < 0.01f && rightDistance < 0.01f)
            {
                isMoving = false;
                Debug.Log($"🚪 Puerta {(isOpen ? "abierta" : "cerrada")} completamente");
            }
        }
    }
    
    public void OpenDoor()
    {
        if (isOpen || isMoving)
        {
            Debug.Log("🚪 La puerta ya está abierta o en movimiento");
            return;
        }
        
        Debug.Log("🚪 Abriendo puerta del ascensor...");
        isOpen = true;
        isMoving = true;
        
        leftTarget = leftDoorOpenPos;
        rightTarget = rightDoorOpenPos;
        
        // Reproducir sonido
        if (doorOpenSound != null && audioSource != null)
            audioSource.PlayOneShot(doorOpenSound);
        
        // Activar efecto de partículas
        if (doorEffect != null)
            doorEffect.Play();
    }
    
    public void CloseDoor()
    {
        if (!isOpen || isMoving)
        {
            Debug.Log("🚪 La puerta ya está cerrada o en movimiento");
            return;
        }
        
        Debug.Log("🚪 Cerrando puerta del ascensor...");
        isOpen = false;
        isMoving = true;
        
        leftTarget = leftDoorClosedPos;
        rightTarget = rightDoorClosedPos;
        
        if (doorCloseSound != null && audioSource != null)
            audioSource.PlayOneShot(doorCloseSound);
    }
    
    // Método para abrir/cerrar automáticamente después de un tiempo
    public IEnumerator OpenAndCloseAfterDelay(float openTime)
    {
        OpenDoor();
        yield return new WaitForSeconds(openTime);
        CloseDoor();
    }
    
    // Método para verificar si la puerta está completamente abierta
    public bool IsFullyOpen()
    {
        if (!isMoving && isOpen)
            return true;
        return false;
    }
    
    // Método para forzar la posición de la puerta (útil para reset)
    public void ForceClose()
    {
        if (leftDoor != null)
            leftDoor.localPosition = leftDoorClosedPos;
        if (rightDoor != null)
            rightDoor.localPosition = rightDoorClosedPos;
        
        isOpen = false;
        isMoving = false;
    }
}