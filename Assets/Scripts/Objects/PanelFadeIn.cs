using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanelFadeIn : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private Image panelImage;

    private void Start()
    {
        if (panelImage == null)
            panelImage = GetComponent<Image>();

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        Color color = panelImage.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = 1f - (elapsed / duration);
            panelImage.color = color;
            yield return null;
        }

        color.a = 0f;
        panelImage.color = color;
        gameObject.SetActive(false);
    }
}
