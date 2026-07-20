using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            GameEvents.InvokeRestartLevel();
        }
    }
}
