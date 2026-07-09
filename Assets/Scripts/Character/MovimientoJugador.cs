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


    private CharacterController controller;

    private Vector2 moveInput;
    private Vector3 velocity;


    // Datos para animaciones
    public float HorizontalInput { get; private set; }
    public bool IsGrounded => controller.isGrounded;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }


    void Update()
    {
        HandleMovement();
        LockZAxis();
        HandleGravity();

        // Único Move por frame
        Vector3 finalMovement = velocity;
        controller.Move(finalMovement * Time.deltaTime);
    }


    // Input de movimiento
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }


    // Input de salto
    public void OnJump(InputValue value)
    {
        if (value.isPressed && IsGrounded)
        {
            velocity.y = jumpForce;
        }
    }


    void HandleMovement()
    {
        HorizontalInput = moveInput.x;


        Vector3 movement = new Vector3(
            moveInput.x * speed,
            0,
            0
        );


        velocity.x = movement.x;
        velocity.z = movement.z;


        RotatePlayer();
    }


    void HandleGravity()
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


    void RotatePlayer()
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
}

