using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class DialogueTrigger : MonoBehaviour
{
    [Header("Diálogo")]
    [SerializeField] private List<AudioClip> dialogueClips;

    [Header("Configuración")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float delayBeforeStart;
    [SerializeField] private float delayBetweenClips = 0.5f;
    [SerializeField] private float delayAfterFinish;
    [SerializeField] private bool destroyOnComplete = true;

    [Header("Transición")]
    [SerializeField] private GameObject npcToHide;
    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeDuration = 1f;

    private AudioSource audioSource;
    private bool triggered;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag(playerTag)) return;

        triggered = true;
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        GameEvents.InvokeOnPlayerInputLock(true);
        GameEvents.InvokeOnCinematic(true);

        if (delayBeforeStart > 0f)
            yield return new WaitForSecondsRealtime(delayBeforeStart);

        foreach (AudioClip clip in dialogueClips)
        {
            if (clip == null) continue;

            audioSource.clip = clip;
            audioSource.Play();

            while (audioSource.isPlaying)
                yield return null;

            if (delayBetweenClips > 0f)
                yield return new WaitForSecondsRealtime(delayBetweenClips);
        }

        if (delayAfterFinish > 0f)
            yield return new WaitForSecondsRealtime(delayAfterFinish);

        if (fadePanel != null && npcToHide != null)
        {
            yield return StartCoroutine(Fade(0f, 1f));

            npcToHide.SetActive(false);

            yield return StartCoroutine(Fade(1f, 0f));
        }

        GameEvents.InvokeOnCinematic(false);
        GameEvents.InvokeOnPlayerInputLock(false);

        if (destroyOnComplete)
            Destroy(gameObject);
    }

    private IEnumerator Fade(float from, float to)
    {
        fadePanel.gameObject.SetActive(true);
        Color color = fadePanel.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(from, to, elapsed / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = to;
        fadePanel.color = color;

        if (to == 0f)
            fadePanel.gameObject.SetActive(false);
    }
}
