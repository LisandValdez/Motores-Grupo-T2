using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlaterMove : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float gravity = -9.81f;

    private Vector2 moveInput;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isRunning = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    //Unity Events
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isRunning = true;
        }
        else if (context.canceled)
        {
            isRunning = false;
        }
    }

    void Update()
    {
        // Movimiento
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        // Mover segun orientacion
        move = transform.TransformDirection(move);

        // Movimiento con CharacterController
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Gravedad (por si se necesita a futuro)
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}