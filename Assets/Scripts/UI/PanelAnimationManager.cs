using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanelAnimationManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image fadePanel;

    [Header("Configuración")]
    [SerializeField] private float fadeDuration = 2f;

    private const string IntroPlayedKey = "Capa7IntroPlayed";
    private Coroutine activeCoroutine;

    private void OnEnable()
    {
        GameEvents.GoNextLevel += OnGoNextLevel;
        GameEvents.RestartLevel += OnRestartLevel;
    }

    private void OnDisable()
    {
        GameEvents.GoNextLevel -= OnGoNextLevel;
        GameEvents.RestartLevel -= OnRestartLevel;
    }

    private void Start()
    {
        Debug.Log("PanelAnimationManager Start - IntroPlayed: " + PlayerPrefs.GetInt(IntroPlayedKey, 0));

        if (PlayerPrefs.GetInt(IntroPlayedKey, 0) == 0)
        {
            Debug.Log("Iniciando intro sequence");
            StartCoroutine(IntroSequence());
        }
        else
        {
            fadePanel.gameObject.SetActive(false);
        }
    }

    private void OnGoNextLevel()
    {
        RestartActiveCoroutine();
        activeCoroutine = StartCoroutine(FadeToColor(Color.white));
    }

    private void OnRestartLevel()
    {
        RestartActiveCoroutine();
        activeCoroutine = StartCoroutine(FadeToColor(Color.black, () =>
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                PlayerMovement movement = player.GetComponent<PlayerMovement>();
                if (movement != null)
                {
                    movement.TeleportTo(CheckpointManager.Instance.GetCheckpointPosition());
                }
            }

            StartCoroutine(FadeOut());
        }));
    }

    private void RestartActiveCoroutine()
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        fadePanel.gameObject.SetActive(true);
    }

    private IEnumerator IntroSequence()
    {
        Debug.Log("IntroSequence iniciada");
        fadePanel.gameObject.SetActive(true);

        yield return StartCoroutine(Fade(Color.black, 1f, 0f));

        yield return StartCoroutine(Fade(Color.black, 0f, 1f));

        yield return StartCoroutine(Fade(Color.black, 1f, 0f));

        PlayerPrefs.SetInt(IntroPlayedKey, 1);
        PlayerPrefs.Save();

        fadePanel.gameObject.SetActive(false);
    }

    private IEnumerator FadeToColor(Color targetColor, System.Action onComplete = null)
    {
        fadePanel.color = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);

        yield return StartCoroutine(Fade(targetColor, 0f, 1f));

        onComplete?.Invoke();
    }

    private IEnumerator Fade(Color targetColor, float from, float to)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            fadePanel.color = new Color(targetColor.r, targetColor.g, targetColor.b, alpha);

            yield return null;
        }

        fadePanel.color = new Color(targetColor.r, targetColor.g, targetColor.b, to);
    }

    private IEnumerator FadeOut()
    {
        yield return StartCoroutine(Fade(fadePanel.color, fadePanel.color.a, 0f));
        fadePanel.gameObject.SetActive(false);
    }
}
