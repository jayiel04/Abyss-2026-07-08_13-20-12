using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action GoNextLevel;
    public static event Action RestartLevel;

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
}
