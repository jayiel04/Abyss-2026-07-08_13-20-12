using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NextLevelPlatform : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float riseSpeed = 1.5f;

    [Header("Jugador")]
    [SerializeField] private string playerTag = "Player";

    private Rigidbody rb;
    private bool activated = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Para moverlo por código
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        if (!other.CompareTag(playerTag))
            return;

        activated = true;

        // Dispara el evento
        GameEvents.InvokeGoNextLevel();
        Debug.Log("Next Level Platform activated!");
    }

    private void FixedUpdate()
    {
        if (!activated)
            return;

        Vector3 newPosition = rb.position + Vector3.up * riseSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }
}