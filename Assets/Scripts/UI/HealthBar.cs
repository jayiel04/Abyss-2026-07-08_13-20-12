using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Imágenes de vida")]
    [SerializeField] private Image healthImage;

    [Header("Sprites por estado (0-3)")]
    [SerializeField] private Sprite[] healthSprites = new Sprite[4];

    private void OnEnable()
    {
        GameEvents.OnHealthChanged += UpdateHealthBar;
        Debug.Log($"[HealthBar] OnEnable: CurrentHealth={GameEvents.CurrentHealth}, healthImage={healthImage != null}, sprites={healthSprites.Length}");
        UpdateHealthBar(GameEvents.CurrentHealth);
    }

    private void OnDisable()
    {
        GameEvents.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar(int health)
    {
        if (healthImage == null)
        {
            Debug.LogWarning("[HealthBar] healthImage es null!");
            return;
        }

        int index = Mathf.Clamp(health, 0, healthSprites.Length - 1);
        Debug.Log($"[HealthBar] UpdateHealthBar: health={health}, index={index}, sprite={healthSprites[index] != null}");

        if (healthSprites[index] != null)
        {
            healthImage.sprite = healthSprites[index];
        }
        else
        {
            Debug.LogWarning($"[HealthBar] Sprite en índice {index} es null!");
        }
    }
}
