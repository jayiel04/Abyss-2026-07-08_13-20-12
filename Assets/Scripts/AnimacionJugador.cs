using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement movement;


    void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
    }


    void Update()
    {
        UpdateMovementAnimation();
        UpdateJumpAnimation();
    }


    void UpdateMovementAnimation()
    {
        float speed = Mathf.Abs(
            movement.HorizontalInput
        );


        animator.SetFloat(
            "Speed",
            speed
        );
    }


    void UpdateJumpAnimation()
    {
        animator.SetBool(
            "Grounded",
            movement.IsGrounded
        );
    }
}