using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ProyectilPlanta : MonoBehaviour
{
    [Header("Daño")]
    [SerializeField] private int dano = 1;

    [Header("Duración")]
    [SerializeField] private float tiempoDeVida = 6f;

    [Header("Movimiento")]
    [SerializeField] private bool orientarAlMovimiento = true;

    [Tooltip("Velocidad mínima necesaria para rotar el proyectil.")]
    [SerializeField] private float velocidadMinimaParaRotar = 0.1f;

    [Header("Colisiones")]
    [SerializeField] private bool destruirAlTocarPared = true;

    private Rigidbody rb;
    private Collider colliderProyectil;
    private GameObject propietario;

    private bool lanzado;
    private bool destruido;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colliderProyectil = GetComponent<Collider>();

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        colliderProyectil.isTrigger = true;
    }

    public void Inicializar(
        Vector3 posicionObjetivo,
        float tiempoDeVuelo,
        float inclinacionVertical,
        float multiplicadorFuerza,
        GameObject nuevoPropietario)
    {
        propietario = nuevoPropietario;

        tiempoDeVuelo = Mathf.Max(0.1f, tiempoDeVuelo);
        multiplicadorFuerza = Mathf.Max(0.01f, multiplicadorFuerza);

        IgnorarColisionesConPropietario();

        Vector3 desplazamiento =
            posicionObjetivo - transform.position;

        Vector3 gravedad = Physics.gravity;

        Vector3 velocidadCalculada =
            (
                desplazamiento -
                0.5f *
                gravedad *
                tiempoDeVuelo *
                tiempoDeVuelo
            ) / tiempoDeVuelo;

        velocidadCalculada *= multiplicadorFuerza;

        velocidadCalculada +=
            Vector3.up * inclinacionVertical;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(
            velocidadCalculada,
            ForceMode.VelocityChange
        );

        lanzado = true;

        Destroy(gameObject, tiempoDeVida);
    }

    private void FixedUpdate()
    {
        if (!lanzado || !orientarAlMovimiento)
        {
            return;
        }

        Vector3 velocidadActual = rb.linearVelocity;

        if (velocidadActual.sqrMagnitude <
            velocidadMinimaParaRotar *
            velocidadMinimaParaRotar)
        {
            return;
        }

        transform.rotation = Quaternion.LookRotation(
            velocidadActual.normalized
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (destruido)
        {
            return;
        }

        if (EsParteDelPropietario(other.transform))
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            GameEvents.InvokeOnPlayerDamaged(dano);
            DestruirProyectil();
            return;
        }

        if (destruirAlTocarPared && !other.isTrigger)
        {
            DestruirProyectil();
        }
    }

    private bool EsParteDelPropietario(Transform objeto)
    {
        if (propietario == null)
        {
            return false;
        }

        return objeto == propietario.transform ||
               objeto.IsChildOf(propietario.transform);
    }

    private void IgnorarColisionesConPropietario()
    {
        if (propietario == null ||
            colliderProyectil == null)
        {
            return;
        }

        Collider[] collidersPropietario =
            propietario.GetComponentsInChildren<Collider>();

        foreach (Collider colliderPropietario
                 in collidersPropietario)
        {
            Physics.IgnoreCollision(
                colliderProyectil,
                colliderPropietario,
                true
            );
        }
    }

    private void DestruirProyectil()
    {
        if (destruido)
        {
            return;
        }

        destruido = true;
        Destroy(gameObject);
    }
}
