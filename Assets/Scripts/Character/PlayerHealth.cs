using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 3;

    [Header("Invulnerabilidad")]
    [SerializeField] private float invulnerabilityDuration = 2f;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("Muerte")]
    [SerializeField] private bool restartOnDeath = true;

    // Vida actual del jugador
    private int currentHealth;

    // Indica si el jugador está en período de invulnerabilidad
    private bool isInvulnerable;

    // Referencia al Renderer para el efecto de parpadeo
    private Renderer objectRenderer;

    // Referencia a la rutina de parpadeo para poder detenerla si es necesario
    private Coroutine blinkCoroutine;

    private void Awake()
    {
        // Busca un Renderer en este GameObject o en sus hijos
        // Si el jugador no tiene Renderer, el parpadeo no funcionará
        objectRenderer = GetComponentInChildren<Renderer>();

        // Inicializa la vida actual al máximo
        currentHealth = maxHealth;
    }

    private void Start()
    {
        // Aseguramos que el renderer esté activo al iniciar
        if (objectRenderer != null)
            objectRenderer.enabled = true;

        // Sincroniza el sistema de eventos global con la vida local
        GameEvents.MaxHealth = maxHealth;
        Debug.Log($"[PlayerHealth] Start: maxHealth={maxHealth}, objectRenderer={objectRenderer != null}");
        GameEvents.ResetHealth();
    }

    private void OnEnable()
    {
        GameEvents.RestartLevel += HandleRestartLevel;
    }

    private void OnDisable()
    {
        GameEvents.RestartLevel -= HandleRestartLevel;
    }

    private void HandleRestartLevel()
    {
        currentHealth = maxHealth;
        isInvulnerable = false;
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
        if (objectRenderer != null)
            objectRenderer.enabled = true;
        GameEvents.MaxHealth = maxHealth;
        GameEvents.ResetHealth();
        Debug.Log("[PlayerHealth] Vida reiniciada tras restart level");
    }

    /// <summary>
    /// Método público llamado por DamageDealer para aplicar daño al jugador.
    /// </summary>
    /// <param name="amount">Cantidad de daño a recibir.</param>
    public void TakeDamage(int amount)
    {
        // Si ya está invulnerable o ya está muerto, no hace nada
        if (isInvulnerable || currentHealth <= 0)
        {
            Debug.Log($"[PlayerHealth] Daño ignorado. isInvulnerable={isInvulnerable}, currentHealth={currentHealth}");
            return;
        }

        // Reduce la vida y la clampeamos entre 0 y maxHealth
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        Debug.Log($"[PlayerHealth] Jugador recibió {amount} de daño. Vida restante: {currentHealth}");

        // Notifica al sistema de eventos global (actualiza la UI de vida)
        GameEvents.RemoveHealth(amount);

        // Si la vida llegó a 0, el jugador muere
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Si aún tiene vida, activa el período de invulnerabilidad con parpadeo
        StartInvulnerability();
    }

    /// <summary>
    /// Activa el período de invulnerabilidad y el efecto de parpadeo.
    /// </summary>
    private void StartInvulnerability()
    {
        isInvulnerable = true;
        Debug.Log($"[PlayerHealth] Invulnerabilidad activada por {invulnerabilityDuration}s");

        // Si ya hay una rutina de parpadeo corriendo, la detenemos primero
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        // Inicia la rutina de parpadeo
        blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    /// <summary>
    /// Rutina que hace parpadear al jugador durante el período de invulnerabilidad.
    /// </summary>
    private IEnumerator BlinkRoutine()
    {
        float timer = 0f;

        // Mientras no se termine el tiempo de invulnerabilidad, alterna la visibilidad
        while (timer < invulnerabilityDuration)
        {
            if (objectRenderer != null)
                objectRenderer.enabled = !objectRenderer.enabled;

            yield return new WaitForSeconds(blinkInterval);

            timer += blinkInterval;
        }

        // Al final, aseguramos que el Renderer esté visible
        if (objectRenderer != null)
            objectRenderer.enabled = true;

        // Desactiva la invulnerabilidad
        isInvulnerable = false;
        blinkCoroutine = null;
        Debug.Log("[PlayerHealth] Invulnerabilidad terminada");
    }

    /// <summary>
    /// Maneja la muerte del jugador.
    /// </summary>
    private void Die()
    {
        Debug.Log("[PlayerHealth] Jugador ha muerto");

        // Opcional: ocultar el renderer al morir
        if (objectRenderer != null)
            objectRenderer.enabled = false;

        // Si está activado, reinicia el nivel
        if (restartOnDeath)
        {
            GameEvents.InvokeRestartLevel();
        }
    }
}
