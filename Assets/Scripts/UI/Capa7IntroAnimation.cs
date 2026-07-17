using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Capa7IntroAnimation : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image fadePanel;

    [Header("Configuración")]
    [SerializeField] private float fadeDuration = 2f;

    [Tooltip("Tiempo que permanece la pantalla negra al iniciar la escena.")]
    [SerializeField] private float initialDarkScreenDuration = 2f;

    [Tooltip("Tiempo que permanece la pantalla negra durante el segundo parpadeo.")]
    [SerializeField] private float blinkDarkScreenDuration = 0.3f;

    private const string PlayedKey = "Capa7IntroPlayed";

    private void Start()
    {
        // Descomenta este bloque cuando quieras que la intro
        // solo se reproduzca una vez.
        /*
        if (PlayerPrefs.GetInt(PlayedKey, 0) == 1)
        {
            fadePanel.gameObject.SetActive(false);
            return;
        }
        */

        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        Color color = Color.black;
        color.a = 1f;
        fadePanel.color = color;

        // Mantener la pantalla negra al inicio
        yield return new WaitForSeconds(initialDarkScreenDuration);

        // Abrir los ojos
        yield return StartCoroutine(Fade(color, 1f, 0f));

        // Cerrar los ojos (parpadeo)
        yield return StartCoroutine(Fade(color, 0f, 1f));

        // Mantener la pantalla negra durante el parpadeo
        yield return new WaitForSeconds(blinkDarkScreenDuration);

        // Abrir los ojos definitivamente
        yield return StartCoroutine(Fade(color, 1f, 0f));

        PlayerPrefs.SetInt(PlayedKey, 1);
        PlayerPrefs.Save();

        fadePanel.gameObject.SetActive(false);
    }

    private IEnumerator Fade(Color color, float from, float to)
    {
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
    }
}
