using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTimer : MonoBehaviour
{
    [Header("Tiempo")]
    [SerializeField] private float waitTime = 5f;

    [Header("Escena")]
    [SerializeField] private string sceneName = "Demo";

    [Header("Opciones")]
    [SerializeField] private bool startOnAwake = true;

    private void Awake()
    {
        if (startOnAwake)
        {
            StartTransition();
        }
    }

    public void StartTransition()
    {
        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneName);
    }
}
