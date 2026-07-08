using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;

    [Header("Salto")]
    public float jumpForce = 8f;

    [Header("Gravedad")]
    public float gravity = -20f;


    private CharacterController controller;

    private Vector2 moveInput;
    private Vector3 velocity;


    // Datos públicos para animaciones
    public float HorizontalInput { get; private set; }
    public bool IsGrounded => controller.isGrounded;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }


    void Update()
    {
        Move();
        Jump();
        ApplyGravity();
    }


    // Recibe el movimiento del Input System
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }


    // Recibe el salto del Input System
  public void OnJump(InputValue value)
{
    Debug.Log("Salto presionado");

    if(value.isPressed && IsGrounded)
    {
        velocity.y = jumpForce;
    }
}


    void Move()
    {
        HorizontalInput = moveInput.x;


        Vector3 movement = new Vector3(
            moveInput.x * speed,
            0,
            0
        );


        controller.Move(
            movement * Time.deltaTime
        );


        RotatePlayer();
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


    void ApplyGravity()
    {
        if (IsGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }


        velocity.y += gravity * Time.deltaTime;


        controller.Move(
            velocity * Time.deltaTime
        );
    }


    void Jump()
    {
        // Evita acumulación de velocidad vertical
        if (IsGrounded && velocity.y < 0)
        {
            velocity.y = -4f;
        }
    }
}