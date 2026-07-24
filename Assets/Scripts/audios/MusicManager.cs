using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource musicSource;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInitialize()
    {
        if (FindObjectOfType<MusicManager>() == null)
        {
            GameObject go = new GameObject("MusicManager");
            go.AddComponent<MusicManager>();
        }
    }

    private void Awake()
    {
        GameObject bg = GameObject.Find("BackgroundMusic");
        if (bg != null)
            musicSource = bg.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        GameEvents.OnCinematic += HandleCinematic;
    }

    private void OnDisable()
    {
        GameEvents.OnCinematic -= HandleCinematic;
    }

    private void HandleCinematic(bool isStarting)
    {
        if (musicSource == null) return;

        if (isStarting)
            musicSource.Pause();
        else
            musicSource.UnPause();
    }
}
