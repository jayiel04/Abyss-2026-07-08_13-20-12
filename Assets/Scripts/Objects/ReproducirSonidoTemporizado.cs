using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ReproducirSonidoTemporizado : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip clip;

    [Header("Tiempo")]
    [SerializeField] private float duracion = 2f;

    [Header("Opciones")]
    [SerializeField] private bool reproducirAlIniciar = true;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        if (reproducirAlIniciar)
            Reproducir();
    }

    public void Reproducir()
    {
        if (clip == null || audioSource == null)
            return;

        audioSource.Play();
        Invoke(nameof(Detener), duracion);
    }

    private void Detener()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}
