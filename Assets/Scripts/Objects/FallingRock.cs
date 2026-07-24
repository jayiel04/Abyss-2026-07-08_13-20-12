using UnityEngine;

public class FallingRock : MonoBehaviour
{
    [Header("Detección")]
    [SerializeField] private float detectionRange = 2f;
    [SerializeField] private string playerTag = "Player";

    [Header("Movimiento")]
    [SerializeField] private float fallSpeed = 4f;

    [Header("Destrucción")]
    [SerializeField] private float destroyAfterFall = 2f;

    [Header("Daño")]
    [SerializeField] private int damageAmount = 1;

    private bool hasFallen = false;
    private bool hasHitPlayer = false;
    private Rigidbody rb;
    private Vector3 startPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;

        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        if (hasFallen)
            return;

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null)
            return;

        float distanceX = Mathf.Abs(transform.position.x - player.transform.position.x);

        if (distanceX <= detectionRange)
        {
            StartFalling();
        }
    }

    private void StartFalling()
    {
        hasFallen = true;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.down * fallSpeed;
        }
        else
        {
            StartCoroutine(FallWithoutRigidbody());
        }
    }

    private System.Collections.IEnumerator FallWithoutRigidbody()
    {
        while (!hasHitPlayer)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            if (!hasHitPlayer)
            {
                hasHitPlayer = true;

                PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(damageAmount);
                }

                Destroy(gameObject, destroyAfterFall);
            }
        }
        else
        {
            Destroy(gameObject, destroyAfterFall);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 left = transform.position + Vector3.left * detectionRange;
        Vector3 right = transform.position + Vector3.right * detectionRange;
        Gizmos.DrawLine(left, right);
    }
}
