using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NpcDisappearAfterDialogue : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private DialogueTrigger dialogueTrigger;
    [SerializeField] private GameObject npcToHide;
    [SerializeField] private Image fadePanel;

    [Header("Fundido")]
    [Min(0f)] [SerializeField] private float fadeInDuration = 1f;
    [Min(0f)] [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private bool useUnscaledTime = true;

    private bool isDisappearing;

    private void OnEnable()
    {
        GameEvents.DialogueFinished += HandleDialogueFinished;
    }

    private void OnDisable()
    {
        GameEvents.DialogueFinished -= HandleDialogueFinished;
    }

    private void HandleDialogueFinished(DialogueTrigger source)
    {
        if (isDisappearing || source != dialogueTrigger)
            return;

        if (npcToHide == null || fadePanel == null)
        {
            Debug.LogError(
                "NpcDisappearAfterDialogue necesita el NPC y el panel de fundido.",
                this);
            return;
        }

        isDisappearing = true;
        StartCoroutine(DisappearSequence());
    }

    private IEnumerator DisappearSequence()
    {
        fadePanel.gameObject.SetActive(true);
        yield return Fade(0f, 1f, fadeInDuration);

        Renderer[] npcRenderers = npcToHide.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer npcRenderer in npcRenderers)
            npcRenderer.enabled = false;

        Graphic[] npcGraphics = npcToHide.GetComponentsInChildren<Graphic>(true);
        foreach (Graphic npcGraphic in npcGraphics)
        {
            if (npcGraphic != fadePanel)
                npcGraphic.enabled = false;
        }

        yield return Fade(1f, 0f, fadeOutDuration);
        fadePanel.gameObject.SetActive(false);
        npcToHide.SetActive(false);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        Color color = fadePanel.color;

        if (duration <= 0f)
        {
            color.a = to;
            fadePanel.color = color;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float progress = Mathf.Clamp01(elapsed / duration);
            color.a = Mathf.Lerp(from, to, progress);
            fadePanel.color = color;

            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            yield return null;
        }

        color.a = to;
        fadePanel.color = color;
    }
}
