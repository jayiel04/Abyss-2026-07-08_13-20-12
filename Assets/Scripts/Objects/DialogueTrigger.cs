using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if (destroyOnComplete)
            Destroy(gameObject);
    }
}
