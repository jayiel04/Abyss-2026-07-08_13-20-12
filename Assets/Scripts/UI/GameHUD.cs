using System.Collections;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    [Header("Elementos de UI")]
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject dashEnergyBar;

    [Header("Configuración")]
    [SerializeField] private bool showOnStart = true;
    [SerializeField] private float fadeDuration = 0.3f;

    private CanvasGroup healthBarGroup;
    private CanvasGroup dashEnergyBarGroup;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        healthBarGroup = GetCanvasGroup(healthBar);
        dashEnergyBarGroup = GetCanvasGroup(dashEnergyBar);
    }

    private void Start()
    {
        if (showOnStart)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void OnEnable()
    {
        GameEvents.GoNextLevel += OnGoNextLevel;
        GameEvents.ReturnCheckpoint += OnReturnCheckpoint;
        GameEvents.OnHUDShowRequested += FadeIn;
    }

    private void OnDisable()
    {
        GameEvents.GoNextLevel -= OnGoNextLevel;
        GameEvents.ReturnCheckpoint -= OnReturnCheckpoint;
        GameEvents.OnHUDShowRequested -= FadeIn;
    }

    private void OnGoNextLevel()
    {
        Hide();
    }

    private void OnReturnCheckpoint()
    {
        Hide();
        StartCoroutine(ShowDelayed(1f));
    }

    public void Show()
    {
        SetActive(healthBar, true);
        SetActive(dashEnergyBar, true);

        if (healthBarGroup != null) healthBarGroup.alpha = 1f;
        if (dashEnergyBarGroup != null) dashEnergyBarGroup.alpha = 1f;
    }

    public void Hide()
    {
        SetActive(healthBar, false);
        SetActive(dashEnergyBar, false);
    }

    public void FadeIn()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        SetActive(healthBar, true);
        SetActive(dashEnergyBar, true);
        fadeCoroutine = StartCoroutine(Fade(0f, 1f));
    }

    public void FadeOut()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(Fade(1f, 0f, () =>
        {
            SetActive(healthBar, false);
            SetActive(dashEnergyBar, false);
        }));
    }

    private IEnumerator ShowDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        FadeIn();
    }

    private IEnumerator Fade(float from, float to, System.Action onComplete = null)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);

            if (healthBarGroup != null) healthBarGroup.alpha = alpha;
            if (dashEnergyBarGroup != null) dashEnergyBarGroup.alpha = alpha;

            yield return null;
        }

        if (healthBarGroup != null) healthBarGroup.alpha = to;
        if (dashEnergyBarGroup != null) dashEnergyBarGroup.alpha = to;

        onComplete?.Invoke();
    }

    private void SetActive(GameObject obj, bool state)
    {
        if (obj != null)
            obj.SetActive(state);
    }

    private CanvasGroup GetCanvasGroup(GameObject obj)
    {
        if (obj == null) return null;

        CanvasGroup group = obj.GetComponent<CanvasGroup>();
        if (group == null)
            group = obj.AddComponent<CanvasGroup>();

        return group;
    }
}
