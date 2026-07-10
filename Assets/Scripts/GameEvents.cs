using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action GoNextLevel;

    public static void InvokeGoNextLevel()
    {
        Debug.Log("GoNextLevel invocado");

        GoNextLevel?.Invoke();
    }
}
