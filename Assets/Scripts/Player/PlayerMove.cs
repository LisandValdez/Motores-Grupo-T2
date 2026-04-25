using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float aimSpeed = 2.5f; // Velocidad reducida al apuntar
    public float gravity = -9.81f;

    private Vector2 moveInput;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isRunning = false;
    private bool isAiming = false;
    

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    // Recibe el estado desde el WeaponController
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

        // Determinación de la velocidad actual
        float currentSpeed = walkSpeed;

        if (isAiming)
        {
            currentSpeed = aimSpeed; // Prioridad máxima: apuntar es lento
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

    

    [Header("Evento muerte")]
    private int maxLife;
    private int currentLife;
    private Rigidbody rb;
    private bool isDead = false;

    private void FixedUpdate()
    {
        if (isDead) return;
    }

    public void SetMaxLife(int life)
    {
        maxLife = life;
        currentLife = life;
    }

    public void die()//
    {
        Debug.Log("PlayerMove: ya no puedo moverme, estoy muerto.");
        rb.linearVelocity = Vector3.zero;
        isDead = true;

    }

}