using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TransitionSceneSkipper : MonoBehaviour
{
    private const string TransitionPrefix = "Transicion";

    [Header("Entrada")]
    [SerializeField] private bool allowKeyboard = true;
    [SerializeField] private Key keyboardKey = Key.Space;
    [SerializeField] private bool allowGamepad = true;

    private bool isLoadingScene;

    private void Update()
    {
        bool keyboardPressed = allowKeyboard &&
            Keyboard.current != null &&
            Keyboard.current[keyboardKey].wasPressedThisFrame;
        bool gamepadPressed = allowGamepad &&
            Gamepad.current != null &&
            Gamepad.current.buttonSouth.wasPressedThisFrame;

        if (keyboardPressed || gamepadPressed)
            SkipTransition();
    }

    public void SkipTransition()
    {
        if (isLoadingScene)
            return;

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (!currentSceneName.StartsWith(TransitionPrefix, StringComparison.Ordinal))
        {
            Debug.LogWarning(
                $"TransitionSceneSkipper: '{currentSceneName}' no es una escena de transicion.",
                this);
            return;
        }

        string targetSceneName = currentSceneName.Substring(TransitionPrefix.Length);
        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogError(
                "TransitionSceneSkipper: no se pudo obtener la escena de destino.",
                this);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.LogError(
                $"TransitionSceneSkipper: la escena '{targetSceneName}' no existe " +
                "o no esta incluida en Build Settings.",
                this);
            return;
        }

        isLoadingScene = true;
        SceneManager.LoadScene(targetSceneName);
    }
}
