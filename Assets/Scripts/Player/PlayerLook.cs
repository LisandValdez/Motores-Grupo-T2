using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public float sensitivity = 2f;
    [Range(0.1f, 1f)]
    public float aimSensitivityMultiplier = 0.5f; 
    public Transform player;

    private Vector2 lookInput;
    private float xRotation = 0f;
    private bool isAiming = false;

    
    public void SetAiming(bool state)
    {
        isAiming = state;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    void Update()
    {
        float currentSensitivity = isAiming ? (sensitivity * aimSensitivityMultiplier) : sensitivity;

        float mouseX = lookInput.x * currentSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * currentSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.Rotate(Vector3.up * mouseX);
    }
}