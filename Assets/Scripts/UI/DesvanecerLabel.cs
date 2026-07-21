using System.Collections;
using TMPro;
using UnityEngine;

public class DesvanecerLabel : MonoBehaviour
{
    [Header("Referencia")]
    [SerializeField] private TMP_Text label;

    [Header("Aparición")]
    [Tooltip("Tiempo que espera antes de comenzar a aparecer.")]
    [SerializeField] private float retrasoAparicion = 1f;

    [Tooltip("Tiempo que tarda el label en aparecer completamente.")]
    [SerializeField] private float duracionAparicion = 1f;

    [Header("Desaparición")]
    [Tooltip("Tiempo que permanece completamente visible.")]
    [SerializeField] private float tiempoVisible = 3f;

    [Tooltip("Tiempo que tarda en desaparecer.")]
    [SerializeField] private float duracionDesvanecimiento = 1.5f;

    [Header("Configuración")]
    [Tooltip("Desactiva el objeto cuando termina la animación.")]
    [SerializeField] private bool desactivarAlFinal = true;

    private Coroutine animacionActual;

    private void Start()
    {
        if (label == null)
        {
            label = GetComponent<TMP_Text>();
        }

        if (label == null)
        {
            Debug.LogError(
                "No se encontró un componente TMP_Text.",
                gameObject
            );

            enabled = false;
            return;
        }

        animacionActual = StartCoroutine(SecuenciaLabel());
    }

    private IEnumerator SecuenciaLabel()
    {
        // Comienza completamente invisible.
        CambiarTransparencia(0f);

        // Espera antes de comenzar la aparición.
        if (retrasoAparicion > 0f)
        {
            yield return new WaitForSeconds(retrasoAparicion);
        }

        // Animación de aparición.
        yield return CambiarTransparenciaGradualmente(
            0f,
            1f,
            duracionAparicion
        );

        // Permanece completamente visible.
        if (tiempoVisible > 0f)
        {
            yield return new WaitForSeconds(tiempoVisible);
        }

        // Animación de desaparición.
        yield return CambiarTransparenciaGradualmente(
            1f,
            0f,
            duracionDesvanecimiento
        );

        if (desactivarAlFinal)
        {
            label.gameObject.SetActive(false);
        }

        animacionActual = null;
    }

    private IEnumerator CambiarTransparenciaGradualmente(
        float transparenciaInicial,
        float transparenciaFinal,
        float duracion
    )
    {
        if (duracion <= 0f)
        {
            CambiarTransparencia(transparenciaFinal);
            yield break;
        }

        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.deltaTime;

            float progreso = Mathf.Clamp01(
                tiempoTranscurrido / duracion
            );

            float transparencia = Mathf.Lerp(
                transparenciaInicial,
                transparenciaFinal,
                progreso
            );

            CambiarTransparencia(transparencia);

            yield return null;
        }

        CambiarTransparencia(transparenciaFinal);
    }

    private void CambiarTransparencia(float transparencia)
    {
        Color colorActual = label.color;
        colorActual.a = transparencia;
        label.color = colorActual;
    }
}