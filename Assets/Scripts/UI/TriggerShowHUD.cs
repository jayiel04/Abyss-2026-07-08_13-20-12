using UnityEngine;

public class TriggerShowHUD : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string playerTag = "Player";

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (!other.CompareTag(playerTag)) return;

        triggered = true;
        GameEvents.InvokeOnHUDShowRequested();
    }
}
