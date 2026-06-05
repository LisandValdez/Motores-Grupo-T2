using UnityEngine;
using UnityEngine.InputSystem;

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
    
    // 🔥 NUEVA VARIABLE PARA LA VIDA
    private int maxLife = 100;
    private int currentLife = 100;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // 🔥 NUEVO MÉTODO - SetMaxLife
    public void SetMaxLife(int maxLifeValue)
    {
        maxLife = maxLifeValue;
        currentLife = maxLifeValue;
        Debug.Log($"✅ Vida máxima establecida en: {maxLife}");
    }

    // 🔥 MÉTODO PARA MORIR
    public void die()
    {
        Debug.Log("💀 El jugador ha muerto");
        // Aquí puedes agregar lógica de muerte (desactivar control, animación, etc.)
        enabled = false; // Desactiva el movimiento
    }

    public void SetAiming(bool state)
    {
        isAiming = state;
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
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = transform.TransformDirection(move);

        float currentSpeed = walkSpeed;

        if (isAiming)
        {
            currentSpeed = aimSpeed;
        }
        else if (isRunning)
        {
            currentSpeed = runSpeed;
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}