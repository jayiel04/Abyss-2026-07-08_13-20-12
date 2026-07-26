using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenFadeReturnCheckpoint : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image fadePanel;

    [Header("Configuración")]
    [SerializeField] private float fadeDuration = 2f;

    private Coroutine fadeCoroutine;

    private void OnDisable()
    {
        GameEvents.ReturnCheckpoint -= StartFade;
    }

    private void OnEnable()
    {
        Debug.Log("ScreenFadeReturnCheckpoint se suscribió");
        GameEvents.ReturnCheckpoint += StartFade;
    }

    private void StartFade()
    {
        Debug.Log("StartFadeRestart ejecutado");

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadePanel.gameObject.SetActive(true);
        fadeCoroutine = StartCoroutine(FadeToBlack());
    }

    private IEnumerator FadeToBlack()
    {
        Color color = Color.black;
        color.a = 0f;
        fadePanel.color = color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadePanel.color = color;

            yield return null;
        }

        color.a = 1f;
        fadePanel.color = color;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
