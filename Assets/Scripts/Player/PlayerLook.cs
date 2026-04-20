using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public float sensitivity = 2f;
    public Transform player;

    private Vector2 lookInput;
    private float xRotation = 0f;

    // Unity Events
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    void Update()
    {
        // Rotación en el eje X (vertical, rota la camara)
        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación en el eje Y (horizontal, rota el player)
        player.Rotate(Vector2.up * mouseX);
    }
}
