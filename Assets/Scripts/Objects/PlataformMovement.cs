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

    private PlayerMovement player;

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

        // Arrastrar al jugador
        if (player != null && player.IsGrounded)
        {
            player.AddExternalMovement(deltaMovement);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement p))
        {
            player = p;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement p) && p == player)
        {
            player = null;
        }
    }
}