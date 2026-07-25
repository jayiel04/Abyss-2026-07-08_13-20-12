using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SonidoPasos : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip clipPasos;

    private AudioSource audioSource;
    private float currentHorizontalInput;
    private bool currentIsGrounded;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null && clipPasos != null)
        {
            audioSource.clip = clipPasos;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerMovementState += HandleMovementState;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerMovementState -= HandleMovementState;
    }

    private void HandleMovementState(float horizontalInput, bool isGrounded)
    {
        currentHorizontalInput = horizontalInput;
        currentIsGrounded = isGrounded;
    }

    private void Update()
    {
        if (clipPasos == null || audioSource == null)
            return;

        bool deberiaSonar = Mathf.Abs(currentHorizontalInput) > 0.01f && currentIsGrounded;

        if (deberiaSonar && !audioSource.isPlaying)
            audioSource.Play();
        else if (!deberiaSonar && audioSource.isPlaying)
            audioSource.Stop();
    }
}
