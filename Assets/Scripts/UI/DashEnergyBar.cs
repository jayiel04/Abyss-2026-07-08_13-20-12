using UnityEngine;
using UnityEngine.UI;

public class DashEnergyBar : MonoBehaviour
{
    [Header("Energia del dash")]
    [SerializeField] private Image fillImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField, Min(0.01f)] private float maxEnergy = 100f;
    [SerializeField, Min(0f)] private float regenerationPerSecond = 20f;

    [Header("Feedback visual")]
    [SerializeField] private Color normalColor = new Color(0.1f, 0.65f, 1f);
    [SerializeField] private Color insufficientColor = Color.red;
    [SerializeField, Min(0f)] private float insufficientColorDuration = 2f;

    private float currentEnergy;
    private float insufficientColorTimer;
    private Color normalBackgroundColor;

    public float CurrentEnergy => currentEnergy;
    public float MaxEnergy => maxEnergy;

    private void Awake()
    {
        maxEnergy = Mathf.Max(0.01f, maxEnergy);
        regenerationPerSecond = Mathf.Max(0f, regenerationPerSecond);
        insufficientColorDuration = Mathf.Max(0.01f, insufficientColorDuration);
        currentEnergy = maxEnergy;

        if (fillImage != null)
        {
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = 0;
            fillImage.color = normalColor;
        }

        if (backgroundImage != null)
            normalBackgroundColor = backgroundImage.color;

        UpdateFillAmount();
    }

    private void OnEnable()
    {
        GameEvents.DashEnergyRequested += HandleDashEnergyRequest;
        UpdateFillAmount();
    }

    private void OnDisable()
    {
        GameEvents.DashEnergyRequested -= HandleDashEnergyRequest;
    }

    private void Update()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy = Mathf.MoveTowards(
                currentEnergy,
                maxEnergy,
                regenerationPerSecond * Time.deltaTime
            );
            UpdateFillAmount();
        }

        if (insufficientColorTimer <= 0f)
            return;

        insufficientColorTimer -= Time.unscaledDeltaTime;

        if (insufficientColorTimer <= 0f)
            RestoreNormalColors();
    }

    private void HandleDashEnergyRequest(GameEvents.DashEnergyRequest request)
    {
        if (request.IsResolved)
            return;

        if (currentEnergy < request.Amount)
        {
            ShowInsufficientEnergy();
            request.Resolve(false);
            return;
        }

        currentEnergy -= request.Amount;
        UpdateFillAmount();
        request.Resolve(true);
    }

    private void ShowInsufficientEnergy()
    {
        insufficientColorTimer = insufficientColorDuration;

        if (fillImage != null)
            fillImage.color = insufficientColor;

        if (backgroundImage != null)
            backgroundImage.color = insufficientColor;
    }

    private void RestoreNormalColors()
    {
        if (fillImage != null)
            fillImage.color = normalColor;

        if (backgroundImage != null)
            backgroundImage.color = normalBackgroundColor;
    }

    private void UpdateFillAmount()
    {
        if (fillImage != null)
            fillImage.fillAmount = currentEnergy / maxEnergy;
    }
}
