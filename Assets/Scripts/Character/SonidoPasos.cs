using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SonidoPasos : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Audio")]
    [SerializeField] private AudioClip clipPasos;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (audioSource != null && clipPasos != null)
        {
            audioSource.clip = clipPasos;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        if (clipPasos == null || audioSource == null)
            return;

        bool deberiaSonar =
            playerMovement != null &&
            Mathf.Abs(playerMovement.HorizontalInput) > 0.01f &&
            playerMovement.IsGrounded;

        if (deberiaSonar && !audioSource.isPlaying)
            audioSource.Play();
        else if (!deberiaSonar && audioSource.isPlaying)
            audioSource.Stop();
    }
}
