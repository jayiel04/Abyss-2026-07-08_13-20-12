using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 8f;

    [Tooltip("Impide que el personaje pueda desplazarse en el eje Z.")]
    public bool bloquearEjeZ = true;

    [Header("Salto")]
    public float jumpForce = 12f;

    [Header("Gravedad")]
    public float gravity = -9.8f;

    [Header("Dash")]
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    [Min(0f)] public float dashEnergyCost = 25f;

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
    public float VerticalVelocity => velocity.y;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        GameEvents.OnCinematic += HandleCinematic;
    }

    private void OnDisable()
    {
        GameEvents.OnCinematic -= HandleCinematic;
    }

    private void HandleCinematic(bool isStarting)
    {
        LockInput(isStarting);
    }

    private void Update()
    {
        UpdateDash();
        HandleMovement();
        HandleGravity();
        ApplyMovement();
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

        if (bloquearEjeZ)
            velocity.z = 0;

        RotatePlayer();
    }

    private void HandleGravity()
    {
        if (isDashing)
        {
            return;
        }

        if (IsGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
    }

    private void ApplyMovement()
    {
        Vector3 finalMovement;

        if (isDashing)
        {
            float dashSpeed = dashDistance / dashDuration;
            finalMovement = dashDirection * dashSpeed * Time.deltaTime;
            finalMovement += externalMovement;
            externalMovement = Vector3.zero;
        }
        else
        {
            finalMovement = velocity * Time.deltaTime;
            finalMovement += externalMovement;
            externalMovement = Vector3.zero;
        }

        if (bloquearEjeZ)
            finalMovement.z = 0f;

        controller.Move(finalMovement);
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
        if (value.isPressed && IsGrounded && !isDashing && !inputLocked)
            velocity.y = jumpForce;
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

        if (!GameEvents.TryConsumeDashEnergy(dashEnergyCost))
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
            velocity.x = 0;
            velocity.z = 0;
            HorizontalInput = 0;
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
