using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public float sensitivity = 2f;
    [Range(0.1f, 1f)]
    public float aimSensitivityMultiplier = 0.5f; // 0.5 significa la mitad de sensibilidad
    public Transform player;

    private Vector2 lookInput;
    private float xRotation = 0f;
    private bool isAiming = false;

    // Recibe el estado desde el WeaponController
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
        // Aplicar reducción si estamos apuntando
        float currentSensitivity = isAiming ? (sensitivity * aimSensitivityMultiplier) : sensitivity;

        float mouseX = lookInput.x * currentSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * currentSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.Rotate(Vector3.up * mouseX);
    }
}