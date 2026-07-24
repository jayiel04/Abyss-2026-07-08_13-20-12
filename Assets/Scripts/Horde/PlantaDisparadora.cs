using UnityEngine;

public class PlantaDisparadora : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform jugador;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private ProyectilPlanta prefabProyectil;

    [Tooltip("Parte de la planta que girará hacia el jugador.")]
    [SerializeField] private Transform parteQueGira;

    [Header("Detección")]
    [SerializeField] private float alcance = 12f;

    [Tooltip("Altura del cuerpo del jugador a la que apuntará.")]
    [SerializeField] private float alturaObjetivo = 1f;

    [Header("Frecuencia de disparo")]
    [SerializeField] private float tiempoEntreDisparos = 2f;

    [Header("Trayectoria del proyectil")]
    [Tooltip("Tiempo mínimo que tardará el proyectil en llegar.")]
    [SerializeField] private float tiempoVueloMinimo = 1f;

    [Tooltip("Tiempo máximo que tardará el proyectil en llegar.")]
    [SerializeField] private float tiempoVueloMaximo = 1.4f;

    [Tooltip("Impulso vertical adicional mínimo.")]
    [SerializeField] private float inclinacionVerticalMinima = 0.3f;

    [Tooltip("Impulso vertical adicional máximo.")]
    [SerializeField] private float inclinacionVerticalMaxima = 1f;

    [Tooltip("Multiplicador mínimo de la fuerza calculada.")]
    [SerializeField] private float multiplicadorFuerzaMinimo = 0.95f;

    [Tooltip("Multiplicador máximo de la fuerza calculada.")]
    [SerializeField] private float multiplicadorFuerzaMaximo = 1.05f;

    [Header("Rotación")]
    [SerializeField] private bool girarHaciaJugador = true;
    [SerializeField] private bool girarSoloEnY = true;
    [SerializeField] private float velocidadGiro = 5f;

    [Tooltip("Úsalo si el modelo mira hacia una dirección incorrecta.")]
    [SerializeField] private float correccionRotacionY = 0f;

    private float siguienteDisparo;

    private void Start()
    {
        if (jugador == null)
        {
            GameObject objetoJugador =
                GameObject.FindGameObjectWithTag("Player");

            if (objetoJugador != null)
            {
                jugador = objetoJugador.transform;
            }
        }

        if (parteQueGira == null)
        {
            parteQueGira = transform;
        }

        OrdenarValores();
    }

    private void OnValidate()
    {
        OrdenarValores();
    }

    private void Update()
    {
        if (jugador == null ||
            puntoDisparo == null ||
            prefabProyectil == null)
        {
            return;
        }

        float distanciaAlJugador = Vector3.Distance(
            transform.position,
            jugador.position
        );

        if (distanciaAlJugador > alcance)
        {
            return;
        }

        Vector3 posicionObjetivo =
            jugador.position + Vector3.up * alturaObjetivo;

        if (girarHaciaJugador)
        {
            GirarHaciaJugador(posicionObjetivo);
        }

        if (Time.time >= siguienteDisparo)
        {
            Disparar(posicionObjetivo);

            siguienteDisparo =
                Time.time + tiempoEntreDisparos;
        }
    }

    private void GirarHaciaJugador(Vector3 posicionObjetivo)
    {
        Vector3 direccion =
            posicionObjetivo - parteQueGira.position;

        if (girarSoloEnY)
        {
            direccion.y = 0f;
        }

        if (direccion.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion rotacionObjetivo =
            Quaternion.LookRotation(direccion.normalized);

        rotacionObjetivo *= Quaternion.Euler(
            0f,
            correccionRotacionY,
            0f
        );

        parteQueGira.rotation = Quaternion.Slerp(
            parteQueGira.rotation,
            rotacionObjetivo,
            velocidadGiro * Time.deltaTime
        );
    }

    private void Disparar(Vector3 posicionObjetivo)
    {
        float tiempoVuelo = Random.Range(
            tiempoVueloMinimo,
            tiempoVueloMaximo
        );

        float inclinacionVertical = Random.Range(
            inclinacionVerticalMinima,
            inclinacionVerticalMaxima
        );

        float multiplicadorFuerza = Random.Range(
            multiplicadorFuerzaMinimo,
            multiplicadorFuerzaMaximo
        );

        ProyectilPlanta proyectil = Instantiate(
            prefabProyectil,
            puntoDisparo.position,
            puntoDisparo.rotation
        );

        proyectil.Inicializar(
            posicionObjetivo,
            tiempoVuelo,
            inclinacionVertical,
            multiplicadorFuerza,
            gameObject
        );
    }

    private void OrdenarValores()
    {
        alcance = Mathf.Max(0f, alcance);
        tiempoEntreDisparos = Mathf.Max(0.05f, tiempoEntreDisparos);

        tiempoVueloMinimo = Mathf.Max(0.1f, tiempoVueloMinimo);
        tiempoVueloMaximo = Mathf.Max(
            tiempoVueloMinimo,
            tiempoVueloMaximo
        );

        inclinacionVerticalMaxima = Mathf.Max(
            inclinacionVerticalMinima,
            inclinacionVerticalMaxima
        );

        multiplicadorFuerzaMinimo = Mathf.Max(
            0.01f,
            multiplicadorFuerzaMinimo
        );

        multiplicadorFuerzaMaximo = Mathf.Max(
            multiplicadorFuerzaMinimo,
            multiplicadorFuerzaMaximo
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, alcance);
    }
}
