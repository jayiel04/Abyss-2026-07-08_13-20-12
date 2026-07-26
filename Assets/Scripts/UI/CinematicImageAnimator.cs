using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class CinematicImageAnimator : MonoBehaviour
{
    public enum MovementType
    {
        Static,
        ZoomIn,
        ZoomOut,
        LeftToRight,
        RightToLeft,
        BottomToTop,
        TopToBottom,
        BottomLeftToTopRight,
        TopRightToBottomLeft,
        Custom
    }

    [Serializable]
    public class CinematicShot
    {
        [SerializeField] private Sprite image;
        [Min(0.01f)] [SerializeField] private float duration = 5f;
        [SerializeField] private MovementType movement = MovementType.ZoomIn;
        [Min(0f)] [SerializeField] private float travelDistance = 100f;
        [SerializeField] private Vector2 customStartPosition;
        [SerializeField] private Vector2 customEndPosition;
        [Min(0.01f)] [SerializeField] private float startZoom = 1f;
        [Min(0.01f)] [SerializeField] private float endZoom = 1.1f;
        [Min(0f)] [SerializeField] private float fadeInDuration = 0.5f;
        [Min(0f)] [SerializeField] private float fadeOutDuration = 0.5f;
        [SerializeField] private AnimationCurve movementCurve =
            AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        public Sprite Image => image;
        public float Duration => duration;
        public MovementType Movement => movement;
        public float TravelDistance => travelDistance;
        public Vector2 CustomStartPosition => customStartPosition;
        public Vector2 CustomEndPosition => customEndPosition;
        public float StartZoom => startZoom;
        public float EndZoom => endZoom;
        public float FadeInDuration => fadeInDuration;
        public float FadeOutDuration => fadeOutDuration;
        public AnimationCurve MovementCurve => movementCurve;
    }

    [Header("Referencias")]
    [SerializeField] private Image targetImage;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Secuencia")]
    [SerializeField] private List<CinematicShot> shots = new List<CinematicShot>();
    [SerializeField] private bool playOnStart;
    [SerializeField] private bool loop;
    [SerializeField] private bool useUnscaledTime = true;
    [SerializeField] private bool hideWhenFinished = true;
    [SerializeField] private bool preserveAspect = true;

    private RectTransform imageRectTransform;
    private Vector2 baseAnchoredPosition;
    private Vector3 baseScale;
    private Coroutine playbackCoroutine;

    public bool IsPlaying => playbackCoroutine != null;
    public int ShotCount => shots.Count;

    public event Action<int> ShotStarted;
    public event Action PlaybackFinished;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (targetImage == null)
            targetImage = GetComponentInChildren<Image>(true);

        if (targetImage == null)
        {
            Debug.LogError("CinematicImageAnimator necesita una Image asignada.", this);
            enabled = false;
            return;
        }

        imageRectTransform = targetImage.rectTransform;
        baseAnchoredPosition = imageRectTransform.anchoredPosition;
        baseScale = imageRectTransform.localScale;
        targetImage.preserveAspect = preserveAspect;

        if (!playOnStart)
            SetVisible(false);
    }

    private void Start()
    {
        if (playOnStart)
            Play();
    }

    private void OnDisable()
    {
        playbackCoroutine = null;
    }

    public void Play()
    {
        if (!CanPlay())
            return;

        StopActiveCoroutine();
        playbackCoroutine = StartCoroutine(PlaySequence());
    }

    public void PlayShot(int shotIndex)
    {
        if (!IsValidShotIndex(shotIndex))
            return;

        PlayShot(shotIndex, shots[shotIndex].Duration);
    }

    public void PlayShot(int shotIndex, float duration)
    {
        if (!IsValidShotIndex(shotIndex))
            return;

        StopActiveCoroutine();
        playbackCoroutine = StartCoroutine(PlaySingleShot(shotIndex, duration));
    }

    public void StopPlayback()
    {
        StopActiveCoroutine();
        RestoreImageTransform();
        SetVisible(false);
    }

    private IEnumerator PlaySequence()
    {
        do
        {
            for (int i = 0; i < shots.Count; i++)
                yield return AnimateShot(i, shots[i].Duration);
        }
        while (loop);

        FinishPlayback();
    }

    private IEnumerator PlaySingleShot(int shotIndex, float duration)
    {
        yield return AnimateShot(shotIndex, duration);
        FinishPlayback();
    }

    private IEnumerator AnimateShot(int shotIndex, float requestedDuration)
    {
        CinematicShot shot = shots[shotIndex];
        if (shot.Image == null)
        {
            Debug.LogWarning($"El plano {shotIndex} no tiene una imagen asignada.", this);
            yield break;
        }

        float duration = Mathf.Max(0.01f, requestedDuration);
        GetMovementPositions(shot, out Vector2 startOffset, out Vector2 endOffset);
        GetZoomValues(shot, out float startZoom, out float endZoom);

        targetImage.sprite = shot.Image;
        targetImage.preserveAspect = preserveAspect;
        SetVisible(true);
        ShotStarted?.Invoke(shotIndex);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float progress = Mathf.Clamp01(elapsed / duration);
            float curvedProgress = shot.MovementCurve != null
                ? shot.MovementCurve.Evaluate(progress)
                : progress;

            imageRectTransform.anchoredPosition = baseAnchoredPosition +
                Vector2.LerpUnclamped(startOffset, endOffset, curvedProgress);
            imageRectTransform.localScale = baseScale *
                Mathf.LerpUnclamped(startZoom, endZoom, curvedProgress);
            canvasGroup.alpha = CalculateAlpha(elapsed, duration, shot);

            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            yield return null;
        }

        imageRectTransform.anchoredPosition = baseAnchoredPosition + endOffset;
        imageRectTransform.localScale = baseScale * endZoom;
        canvasGroup.alpha = shot.FadeOutDuration > 0f ? 0f : 1f;
    }

    private void GetMovementPositions(
        CinematicShot shot,
        out Vector2 startOffset,
        out Vector2 endOffset)
    {
        float distance = shot.TravelDistance;

        switch (shot.Movement)
        {
            case MovementType.LeftToRight:
                startOffset = Vector2.left * distance;
                endOffset = Vector2.right * distance;
                break;
            case MovementType.RightToLeft:
                startOffset = Vector2.right * distance;
                endOffset = Vector2.left * distance;
                break;
            case MovementType.BottomToTop:
                startOffset = Vector2.down * distance;
                endOffset = Vector2.up * distance;
                break;
            case MovementType.TopToBottom:
                startOffset = Vector2.up * distance;
                endOffset = Vector2.down * distance;
                break;
            case MovementType.BottomLeftToTopRight:
                startOffset = new Vector2(-distance, -distance);
                endOffset = new Vector2(distance, distance);
                break;
            case MovementType.TopRightToBottomLeft:
                startOffset = new Vector2(distance, distance);
                endOffset = new Vector2(-distance, -distance);
                break;
            case MovementType.Custom:
                startOffset = shot.CustomStartPosition;
                endOffset = shot.CustomEndPosition;
                break;
            default:
                startOffset = Vector2.zero;
                endOffset = Vector2.zero;
                break;
        }
    }

    private static void GetZoomValues(
        CinematicShot shot,
        out float startZoom,
        out float endZoom)
    {
        startZoom = shot.StartZoom;
        endZoom = shot.EndZoom;

        if (shot.Movement == MovementType.ZoomIn && endZoom < startZoom)
            (startZoom, endZoom) = (endZoom, startZoom);
        else if (shot.Movement == MovementType.ZoomOut && startZoom < endZoom)
            (startZoom, endZoom) = (endZoom, startZoom);
    }

    private static float CalculateAlpha(
        float elapsed,
        float duration,
        CinematicShot shot)
    {
        float fadeInAlpha = shot.FadeInDuration > 0f
            ? elapsed / Mathf.Min(shot.FadeInDuration, duration)
            : 1f;
        float remaining = duration - elapsed;
        float fadeOutAlpha = shot.FadeOutDuration > 0f
            ? remaining / Mathf.Min(shot.FadeOutDuration, duration)
            : 1f;

        return Mathf.Clamp01(Mathf.Min(fadeInAlpha, fadeOutAlpha));
    }

    private bool CanPlay()
    {
        if (targetImage == null || imageRectTransform == null)
            return false;

        if (shots.Count > 0)
            return true;

        Debug.LogWarning("CinematicImageAnimator no tiene planos configurados.", this);
        return false;
    }

    private bool IsValidShotIndex(int shotIndex)
    {
        if (shotIndex >= 0 && shotIndex < shots.Count)
            return targetImage != null && imageRectTransform != null;

        Debug.LogWarning($"No existe el plano con indice {shotIndex}.", this);
        return false;
    }

    private void FinishPlayback()
    {
        playbackCoroutine = null;

        if (hideWhenFinished)
            SetVisible(false);

        PlaybackFinished?.Invoke();
    }

    private void StopActiveCoroutine()
    {
        if (playbackCoroutine == null)
            return;

        StopCoroutine(playbackCoroutine);
        playbackCoroutine = null;
    }

    private void RestoreImageTransform()
    {
        if (imageRectTransform == null)
            return;

        imageRectTransform.anchoredPosition = baseAnchoredPosition;
        imageRectTransform.localScale = baseScale;
    }

    private void SetVisible(bool visible)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = visible ? 1f : 0f;
    }
}
