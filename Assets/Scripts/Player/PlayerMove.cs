using UnityEngine;
using UnityEngine.InputSystem;
// No incluyas ningún using de Cinemachine

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float aimSpeed = 2.5f;
    public float gravity = -9.81f;

    private Vector2 moveInput;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isRunning = false;
    private bool isAiming = false;
    
    private Transform cameraTransform;
    
    private int maxLife = 100;
    private int currentLife = 100;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    
    void Start()
    {
        // Simplemente obtener la cámara principal
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            // Buscar cualquier cámara como fallback
            Camera anyCamera = FindObjectOfType<Camera>();
            if (anyCamera != null)
            {
                cameraTransform = anyCamera.transform;
                Debug.Log("Cámara encontrada: " + anyCamera.name);
            }
            else
            {
                Debug.LogError("No se encontró ninguna cámara en la escena");
            }
        }
    }

    public void SetMaxLife(int maxLifeValue)
    {
        maxLife = maxLifeValue;
        currentLife = maxLifeValue;
        Debug.Log($"✅ Vida máxima establecida en: {maxLife}");
    }

    public void die()
    {
        Debug.Log("💀 El jugador ha muerto");
        enabled = false;
    }

    public void SetAiming(bool state)
    {
        isAiming = state;
    }
    
    public void SetMovementEnabled(bool enabled)
    {
        this.enabled = enabled;
        
        if (controller != null)
            controller.enabled = enabled;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started) isRunning = true;
        else if (context.canceled) isRunning = false;
    }

    void Update()
    {
        if (cameraTransform == null)
        {
            // Intentar obtener la cámara nuevamente
            if (Camera.main != null)
                cameraTransform = Camera.main.transform;
            else
                return;
        }
        
        // Movimiento relativo a la cámara
        Vector3 moveDirection = Vector3.zero;
        
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        
        // Ignorar inclinación vertical
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        
        // Construir vector de movimiento
        moveDirection = (forward * moveInput.y) + (right * moveInput.x);
        
        float currentSpeed = walkSpeed;

        if (isAiming)
        {
            currentSpeed = aimSpeed;
        }
        else if (isRunning)
        {
            currentSpeed = runSpeed;
        }

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        // Gravedad
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}