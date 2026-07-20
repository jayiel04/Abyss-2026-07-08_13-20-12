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
    private Vector3 externalMovement;

    private bool isDashing;
    private float dashTimer;
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
        Debug.Log($"[Frame {Time.frameCount}] IsGrounded: {IsGrounded} | velocity.y: {velocity.y:F2} | isDashing: {isDashing} | inputLocked: {inputLocked}");

        UpdateDash();
        HandleMovement();
        ApplyMovement();
        HandleGravity();
    }

    private void UpdateDash()
    {
        if (!isDashing)
            return;

        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            isDashing = false;
        }
    }

    private void HandleMovement()
    {
        if (inputLocked || isDashing)
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
        if (isDashing)
        {
            Debug.Log("[Gravity] Saltado por dash");
            return;
        }

        Debug.Log($"[Gravity] Before - IsGrounded: {IsGrounded}, velocity.y: {velocity.y:F2}");

        if (IsGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            Debug.Log("[Gravity] Reset velocity.y a -2f (en suelo)");
        }

        velocity.y += gravity * Time.deltaTime;

        Debug.Log($"[Gravity] After - velocity.y: {velocity.y:F2}");
    }

    private void ApplyMovement()
    {
        Vector3 finalMovement;

        if (isDashing)
        {
            float dashSpeed = dashDistance / dashDuration;
            finalMovement = dashDirection * dashSpeed * Time.deltaTime;
            Debug.Log($"[Movement] Dash activo - finalMovement: {finalMovement}");
        }
        else
        {
            finalMovement = velocity * Time.deltaTime;
            finalMovement += externalMovement;
            externalMovement = Vector3.zero;
            Debug.Log($"[Movement] Normal - velocity: {velocity} | finalMovement antes de Z: {finalMovement}");
        }

        finalMovement.z = 0f;
        Debug.Log($"[Movement] finalMovement final: {finalMovement}");

        controller.Move(finalMovement);

        Debug.Log($"[Movement] Después de Move - IsGrounded: {IsGrounded}");
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
        Debug.Log($"[Jump] Input received: {value.isPressed} | IsGrounded: {IsGrounded} | isDashing: {isDashing} | inputLocked: {inputLocked}");

        if (value.isPressed && IsGrounded && !isDashing)
        {
            velocity.y = jumpForce;
            Debug.Log($"[Jump] Ejecutado! jumpForce: {jumpForce}");
        }
        else
        {
            Debug.Log($"[Jump] NO ejecutado - Razón: isPressed={value.isPressed}, IsGrounded={IsGrounded}, !isDashing={!isDashing}");
        }
    }

    public void OnDash(InputValue value)
    {
        if (!value.isPressed || isDashing)
            return;

        if (Time.time < lastDashTime + dashCooldown)
            return;

        float horizontalInput = moveInput.x;
        if (horizontalInput == 0)
            return;

        StartDash(horizontalInput > 0 ? Vector3.right : Vector3.left);
    }

    private void StartDash(Vector3 direction)
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashDirection = direction;
        lastDashTime = Time.time;
        velocity.y = 0;
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
