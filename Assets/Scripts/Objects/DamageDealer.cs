using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Daño")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private string playerTag = "Player";

    [Header("Comportamiento")]
    [SerializeField] private bool destroyOnHit = false;
    [SerializeField] private bool useCollision = false;

    /// <summary>
    /// Se llama cuando otro Collider entra en el trigger.
    /// Solo se ejecuta si useCollision es false (modo trigger por defecto).
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Si está configurado para usar collision, ignoramos el trigger
        if (useCollision) return;

        TryDamage(other.gameObject);
    }

    /// <summary>
    /// Se llama cuando ocurre una colisión física.
    /// Solo se ejecuta si useCollision es true.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        // Si está configurado para usar trigger, ignoramos la collision
        if (!useCollision) return;

        TryDamage(collision.gameObject);
    }

    /// <summary>
    /// Intenta aplicar daño al objeto que colisionó, si es el jugador.
    /// </summary>
    /// <param name="target">El GameObject que colisionó con este DamageDealer.</param>
    private void TryDamage(GameObject target)
    {
        // Verifica que el objeto tenga el tag del jugador
        if (!target.CompareTag(playerTag)) return;

        // Notifica al sistema de eventos global
        GameEvents.InvokeOnPlayerDamaged(damageAmount);
        Debug.Log($"[DamageDealer] Aplicado {damageAmount} de daño al jugador");

        // Si está configurado, destruye este objeto al impactar
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
}
