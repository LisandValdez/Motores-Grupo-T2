using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Configuración")]
    public float mouseSensitivity = 2f;
    [Range(0.1f, 1f)]
    public float aimSensitivityMultiplier = 0.5f;
    public Vector2 rotationLimits = new Vector2(-90f, 90f);
    
    [Header("Referencias")]
    public Transform playerBody;
    
    private Vector2 lookInput;
    private bool isAiming = false;
    private CinemachinePanTilt panTilt;
    private float currentPan = 0f;
    private float currentTilt = 0f;
    
    void Start()
    {
        if (playerBody == null)
        {
            playerBody = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        var virtualCamera = GetComponent<CinemachineCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("No hay CinemachineCamera en este GameObject");
            return;
        }
        
        // Configurar seguimiento
        virtualCamera.Follow = playerBody;
        virtualCamera.LookAt = playerBody;
        
        // 🔥 CAMBIO IMPORTANTE: En CM3 se usa GetOrAddComponent en el GameObject
        panTilt = GetComponent<CinemachinePanTilt>();
        if (panTilt == null)
        {
            panTilt = gameObject.AddComponent<CinemachinePanTilt>();
        }
        
        // Configurar rangos
        if (panTilt != null)
        {
            panTilt.PanAxis.Range = new Vector2(-360f, 360f);
            panTilt.TiltAxis.Range = new Vector2(rotationLimits.x, rotationLimits.y);
            panTilt.PanAxis.Wrap = true;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        if (panTilt == null) return;
        
        float currentSensitivity = isAiming ? (mouseSensitivity * aimSensitivityMultiplier) : mouseSensitivity;
        
        float mouseX = lookInput.x * currentSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * currentSensitivity * Time.deltaTime;
        
        currentPan += mouseX;
        currentTilt -= mouseY;
        currentTilt = Mathf.Clamp(currentTilt, rotationLimits.x, rotationLimits.y);
        
        panTilt.PanAxis.Value = currentPan;
        panTilt.TiltAxis.Value = currentTilt;
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    
    public void SetAiming(bool state)
    {
        isAiming = state;
    }
}