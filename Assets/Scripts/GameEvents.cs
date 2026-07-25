using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action GoNextLevel;
    public static event Action RestartLevel;
    public static event Action<int> OnHealthChanged;
    public static event Action<DashEnergyRequest> DashEnergyRequested;
    public static event Action<bool> OnCinematic;
    public static event Action<int> OnPlayerDamaged;
    public static event Action<bool> OnPlayerInputLock;
    public static event Action<Vector3> OnPlayerExternalMovement;
    public static event Action OnHUDShowRequested;
    public static event Action<float, bool> OnPlayerMovementState;
    public static event Action<Vector3> OnPlayerTeleport;
    public static int MaxHealth = 3;
    public static int CurrentHealth { get; private set; }

    static GameEvents()
    {
        CurrentHealth = MaxHealth;
    }

    public static void InvokeGoNextLevel()
    {
        Debug.Log("GoNextLevel invocado");

        GoNextLevel?.Invoke();
    }

    public static void InvokeRestartLevel()
    {
        Debug.Log("RestartLevel invocado");

        RestartLevel?.Invoke();
    }

    public static void InvokeOnPlayerDamaged(int amount)
    {
        OnPlayerDamaged?.Invoke(amount);
    }

    public static void InvokeOnPlayerInputLock(bool locked)
    {
        OnPlayerInputLock?.Invoke(locked);
    }

    public static void InvokeOnPlayerExternalMovement(Vector3 movement)
    {
        OnPlayerExternalMovement?.Invoke(movement);
    }

    public static void InvokeOnHUDShowRequested()
    {
        OnHUDShowRequested?.Invoke();
    }

    public static void InvokeOnPlayerMovementState(float horizontalInput, bool isGrounded)
    {
        OnPlayerMovementState?.Invoke(horizontalInput, isGrounded);
    }

    public static void InvokeOnPlayerTeleport(Vector3 position)
    {
        OnPlayerTeleport?.Invoke(position);
    }

    public static bool TryConsumeDashEnergy(float amount)
    {
        if (amount <= 0f)
            return true;

        DashEnergyRequest request = new DashEnergyRequest(amount);
        DashEnergyRequested?.Invoke(request);

        if (!request.IsResolved)
            Debug.LogWarning("No hay una barra de energia de dash activa.");

        return request.WasConsumed;
    }

    public static void InvokeOnCinematic(bool isStarting)
    {
        Debug.Log(isStarting ? "Cinematic iniciado" : "Cinematic finalizado");
        OnCinematic?.Invoke(isStarting);
    }

    public static void AddHealth(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        Debug.Log($"Vida actual: {CurrentHealth}");

        OnHealthChanged?.Invoke(CurrentHealth);
    }

    public static void RemoveHealth(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);
        Debug.Log($"Vida actual: {CurrentHealth}");

        OnHealthChanged?.Invoke(CurrentHealth);
    }

    public static void ResetHealth()
    {
        CurrentHealth = MaxHealth;
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    public sealed class DashEnergyRequest
    {
        public float Amount { get; }
        public bool IsResolved { get; private set; }
        public bool WasConsumed { get; private set; }

        public DashEnergyRequest(float amount)
        {
            Amount = amount;
        }

        public void Resolve(bool wasConsumed)
        {
            if (IsResolved)
                return;

            IsResolved = true;
            WasConsumed = wasConsumed;
        }
    }
}
