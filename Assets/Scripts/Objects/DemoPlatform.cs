using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoPlatform : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float riseSpeed = 1.5f;

    [Header("Jugador")]
    [SerializeField] private string playerTag = "Player";

    [Header("Escena")]
    [SerializeField] private string sceneName = "Demo";

    private bool activated = false;

    private void Update()
    {
        if (!activated)
            return;

        Vector3 movement = Vector3.up * riseSpeed * Time.deltaTime;
        transform.position += movement;
        GameEvents.InvokeOnPlayerExternalMovement(movement);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        if (!other.CompareTag(playerTag))
            return;

        activated = true;
        GameEvents.InvokeOnPlayerInputLock(true);
        GameEvents.InvokeGoNextLevel();
        SceneManager.LoadScene(sceneName);
        Debug.Log("Demo Platform activated! Loading scene: " + sceneName);
    }
}
