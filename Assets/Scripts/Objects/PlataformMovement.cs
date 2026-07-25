using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Movimiento")]
    public float distance = 40f;
    public float speed = 2f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 target;

    private Vector3 lastPosition;
    private Vector3 deltaMovement;

    private bool playerOnPlatform = false;

    private void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + Vector3.right * distance;

        target = endPosition;
        lastPosition = transform.position;
    }

    private void Update()
    {
        // Mover plataforma
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime);

        // Cambiar de dirección
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            target = (target == endPosition)
                ? startPosition
                : endPosition;
        }

        // Calcular cuánto se movió este frame
        deltaMovement = transform.position - lastPosition;
        lastPosition = transform.position;

        // Arrastrar al jugador si está en la plataforma
        if (playerOnPlatform)
        {
            GameEvents.InvokeOnPlayerExternalMovement(deltaMovement);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement _))
        {
            playerOnPlatform = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement _))
        {
            playerOnPlatform = false;
        }
    }
}