using UnityEngine;

public class TriggerShowHUD : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string playerTag = "Player";

    [Header("Estado inicial")]
    [SerializeField] private bool hideOnStart = true;

    private GameHUD hud;
    private bool triggered = false;

    private void Start()
    {
        hud = FindObjectOfType<GameHUD>();

        if (hideOnStart && hud != null)
        {
            hud.Hide();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (!other.CompareTag(playerTag)) return;

        if (hud != null)
        {
            triggered = true;
            hud.FadeIn();
        }
    }
}
