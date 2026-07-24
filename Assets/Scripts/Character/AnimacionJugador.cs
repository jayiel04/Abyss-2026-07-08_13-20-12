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
        if (!movement.enabled)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("Grounded", true);
            animator.SetFloat("speedY", 0f);
            return;
        }

        UpdateMovementAnimation();
        UpdateJumpAnimation();
        UpdateVerticalSpeedAnimation();
        HandleInputLock();
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

    void UpdateVerticalSpeedAnimation()
    {
        animator.SetFloat(
            "speedY",
            movement.VerticalVelocity
        );
    }

    void HandleInputLock()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isLocked = stateInfo.IsName("falling") || stateInfo.IsName("getting up");
        movement.LockInput(isLocked);
    }
}
