using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 8f;

    [Header("Salto")]
    public float jumpForce = 12f;

    [Header("Gravedad")]
    public float gravity = -9.8f;

    [Header("Dash")]
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;

    private CharacterController controller;

    private Vector2 moveInput;
    private bool inputLocked = false;
    private Vector3 velocity;

    // Movimiento externo (plataformas, empujes, etc.)
    private Vector3 externalMovement;

    // Dash
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float lastDashTime = -Mathf.Infinity;
    private Vector3 dashDirection;


    public float HorizontalInput { get; private set; }
    public bool IsGrounded => controller.isGrounded;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }


    private void Update()
    {
        if (isDashing)
        {
            HandleDash();
        }
        else
        {
            HandleMovement();
            HandleGravity();
        }

        Vector3 finalMovement = velocity * Time.deltaTime;

        // Movimiento de plataforma
        finalMovement += externalMovement;

        // Durante el dash, usar la dirección del dash
        if (isDashing)
        {
            finalMovement = dashDirection * (dashDistance / dashDuration) * Time.deltaTime;
        }

        // Único Move por frame
        controller.Move(finalMovement);

        // Limpiar movimiento externo
        externalMovement = Vector3.zero;

        LockZAxis();
    }


    public void AddExternalMovement(Vector3 movement)
    {
       
        externalMovement += movement;
    }


    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }


    public void OnJump(InputValue value)
    {
        if (value.isPressed && IsGrounded)
        {
            velocity.y = jumpForce;
        }
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed && !isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            float horizontalInput = moveInput.x;
            if (horizontalInput != 0)
            {
                StartDash(horizontalInput > 0 ? Vector3.right : Vector3.left);
            }
        }
    }

    private void StartDash(Vector3 direction)
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashDirection = direction;
        lastDashTime = Time.time;

        // Congelar gravedad durante el dash
        velocity.y = 0;
    }

    private void HandleDash()
    {
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            isDashing = false;
        }
    }


    private void HandleMovement()
    {
        if (inputLocked)
        {
            velocity.x = 0;
            velocity.z = 0;
            return;
        }


        HorizontalInput = moveInput.x;


        velocity.x = moveInput.x * speed;
        velocity.z = 0;


        RotatePlayer();
    }


    private void HandleGravity()
    {
        if (IsGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }


        velocity.y += gravity * Time.deltaTime;
    }


    private void LockZAxis()
    {
        Vector3 position = transform.position;
        position.z = 0f;
        transform.position = position;
    }


    private void RotatePlayer()
    {
        if (HorizontalInput > 0)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (HorizontalInput < 0)
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }
    }
    public void LockInput(bool state)
    {
        inputLocked = state;

        if (state)
        {
            moveInput = Vector2.zero;
            velocity.x = 0;
            velocity.z = 0;
        }
    }

    public void TeleportTo(Vector3 position)
    {
        controller.enabled = false;
        transform.position = position;
        controller.enabled = true;

        velocity = Vector3.zero;
        isDashing = false;
    }
}