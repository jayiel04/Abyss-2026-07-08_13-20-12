using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelPlatform : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float riseSpeed = 1.5f;

    [Header("Jugador")]
    [SerializeField] private string playerTag = "Player";

    [Header("Cambio de escena")]
    [SerializeField] private float waitTime = 5f;


    private bool activated = false;

    private PlayerMovement currentPlayer;


    private void Update()
    {
        if (!activated)
            return;


        Vector3 movement = Vector3.up * riseSpeed * Time.deltaTime;


        transform.position += movement;


        if (currentPlayer != null)
        {
            currentPlayer.AddExternalMovement(movement);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;


        if (!other.CompareTag(playerTag))
            return;


        activated = true;


        currentPlayer = other.GetComponent<PlayerMovement>();


        // Bloquear controles del jugador
        if (currentPlayer != null)
        {
            currentPlayer.LockInput(true);
        }


        GameEvents.InvokeGoNextLevel();


        StartCoroutine(ChangeLevel());


        Debug.Log("Next Level Platform activated!");
    }


    private IEnumerator ChangeLevel()
    {
        yield return new WaitForSeconds(waitTime);


        string currentScene = SceneManager.GetActiveScene().name;


        string nextScene = GetNextScene(currentScene);


        if (!string.IsNullOrEmpty(nextScene))
        {
            SceneManager.LoadScene(nextScene);
        }
    }


    private string GetNextScene(string currentScene)
    {
        switch (currentScene)
        {
            case "Capa7":
                return "Capa6";

            case "Capa6":
                return "Capa5";

            case "Capa5":
                return "Capa4";

            case "Capa4":
                return "Capa3";

            case "Capa3":
                return "Capa2";

            case "Capa2":
                return "Capa1";

            case "Capa1":
                return "Victoria";


            default:
                Debug.LogWarning("No existe transición para: " + currentScene);
                return null;
        }
    }
}