using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private float horizontalInput;
    private bool isGrounded;
    private float verticalVelocity;
    private bool isCinematic;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerMovementState += HandleMovementState;
        GameEvents.OnCinematic += HandleCinematic;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerMovementState -= HandleMovementState;
        GameEvents.OnCinematic -= HandleCinematic;
    }

    private void HandleMovementState(float horizontalInput, bool isGrounded)
    {
        this.horizontalInput = horizontalInput;
        this.isGrounded = isGrounded;
    }

    private void HandleCinematic(bool isStarting)
    {
        isCinematic = isStarting;
    }

    void Update()
    {
        UpdateMovementAnimation();
        UpdateJumpAnimation();
        UpdateVerticalSpeedAnimation();
        if (!isCinematic)
            HandleInputLock();
    }

    void UpdateMovementAnimation()
    {
        float speed = Mathf.Abs(horizontalInput);
        animator.SetFloat("Speed", speed);
    }

    void UpdateJumpAnimation()
    {
        animator.SetBool("Grounded", isGrounded);
    }

    void UpdateVerticalSpeedAnimation()
    {
        animator.SetFloat("speedY", verticalVelocity);
    }

    void HandleInputLock()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isLocked = stateInfo.IsName("falling") || stateInfo.IsName("getting up");
        GameEvents.InvokeOnPlayerInputLock(isLocked);
    }
}
