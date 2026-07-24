using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioSceneTransition : MonoBehaviour
{
    [Header("Escena siguiente")]
    [SerializeField] private string nextSceneName;

    [Header("Configuración")]
    [SerializeField] private float delayAfterAudio = 0.5f;

    private AudioSource audioSource;
    private bool changingScene;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(PlayAudioAndChangeScene());
    }

    private IEnumerator PlayAudioAndChangeScene()
    {
        if (audioSource.clip == null)
        {
            Debug.LogError(
                "AudioSceneTransition: el AudioSource no tiene un AudioClip asignado."
            );

            yield break;
        }

        audioSource.Play();

        // Espera mientras el audio continúa reproduciéndose.
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(delayAfterAudio);

        ChangeScene();
    }

    private void ChangeScene()
    {
        if (changingScene)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogError(
                "AudioSceneTransition: no se ha indicado el nombre de la siguiente escena."
            );

            return;
        }

        changingScene = true;
        SceneManager.LoadScene(nextSceneName);
    }
}
