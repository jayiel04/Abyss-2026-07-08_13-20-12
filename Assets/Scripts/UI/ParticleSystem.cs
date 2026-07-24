using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class LuciernagasAzules : MonoBehaviour
{
    [Header("Material")]
    [SerializeField]
    private Material materialLuciernaga;

    [Header("Cantidad de partículas")]
    [SerializeField, Min(0f)]
    private float particulasPorSegundo = 4f;

    [SerializeField, Min(1)]
    private int maximoParticulas = 30;

    [Header("Zona de aparición")]
    [SerializeField]
    private Vector3 area = new Vector3(15f, 8f, 1f);

    [Header("Duración")]
    [SerializeField]
    private Vector2 tiempoDeVida = new Vector2(5f, 10f);

    [Header("Tamaño")]
    [SerializeField]
    private Vector2 tamaño = new Vector2(0.04f, 0.12f);

    [Header("Movimiento")]
    [SerializeField]
    private float velocidadMinima = 0.02f;

    [SerializeField]
    private float velocidadMaxima = 0.12f;

    [SerializeField, Min(0f)]
    private float intensidadRuido = 0.25f;

    [Header("Colores")]
    [SerializeField]
    private Color azulOscuro = new Color(0.05f, 0.35f, 1f, 1f);

    [SerializeField]
    private Color azulClaro = new Color(0.2f, 0.9f, 1f, 1f);

    private ParticleSystem sistemaParticulas;
    private ParticleSystemRenderer renderizadorParticulas;

    private void Awake()
    {
        ObtenerComponentes();
        ConfigurarSistemaCompleto();

        sistemaParticulas.Play();
    }

    private void ObtenerComponentes()
    {
        sistemaParticulas = GetComponent<ParticleSystem>();
        renderizadorParticulas = GetComponent<ParticleSystemRenderer>();

        if (renderizadorParticulas == null)
        {
            Debug.LogError(
                "No se encontró el ParticleSystemRenderer.",
                this
            );
        }
    }

    private void ConfigurarSistemaCompleto()
    {
        ConfigurarModuloPrincipal();
        ConfigurarEmision();
        ConfigurarForma();
        ConfigurarRuido();
        ConfigurarVelocidad();
        ConfigurarParpadeo();
        ConfigurarTamañoDuranteVida();
        ConfigurarRenderizador();
    }

    private void ConfigurarModuloPrincipal()
    {
        // Debemos guardar el módulo en una variable antes de modificarlo.
        ParticleSystem.MainModule main = sistemaParticulas.main;

        main.loop = true;
        main.prewarm = true;
        main.playOnAwake = true;
        main.gravityModifier = 0f;
        main.maxParticles = maximoParticulas;

        main.simulationSpace =
            ParticleSystemSimulationSpace.Local;

        main.startLifetime = new ParticleSystem.MinMaxCurve(
            tiempoDeVida.x,
            tiempoDeVida.y
        );

        main.startSpeed = new ParticleSystem.MinMaxCurve(
            velocidadMinima,
            velocidadMaxima
        );

        main.startSize = new ParticleSystem.MinMaxCurve(
            tamaño.x,
            tamaño.y
        );

        main.startColor = new ParticleSystem.MinMaxGradient(
            azulOscuro,
            azulClaro
        );
    }

    private void ConfigurarEmision()
    {
        ParticleSystem.EmissionModule emission =
            sistemaParticulas.emission;

        emission.enabled = true;

        emission.rateOverTime =
            new ParticleSystem.MinMaxCurve(particulasPorSegundo);
    }

    private void ConfigurarForma()
    {
        ParticleSystem.ShapeModule shape =
            sistemaParticulas.shape;

        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = area;

        // Hace que no todas se muevan en la misma dirección.
        shape.randomDirectionAmount = 1f;
    }

    private void ConfigurarRuido()
    {
        ParticleSystem.NoiseModule noise =
            sistemaParticulas.noise;

        noise.enabled = true;
        noise.separateAxes = true;

        noise.strengthX = new ParticleSystem.MinMaxCurve(
            intensidadRuido
        );

        noise.strengthY = new ParticleSystem.MinMaxCurve(
            intensidadRuido * 1.5f
        );

        noise.strengthZ = new ParticleSystem.MinMaxCurve(
            intensidadRuido * 0.3f
        );

        noise.frequency = 0.25f;

        noise.scrollSpeed = new ParticleSystem.MinMaxCurve(
            0.1f
        );

        noise.damping = true;
        noise.quality = ParticleSystemNoiseQuality.Medium;
    }

    private void ConfigurarVelocidad()
    {
        ParticleSystem.VelocityOverLifetimeModule velocity =
            sistemaParticulas.velocityOverLifetime;

        velocity.enabled = true;

        velocity.space =
            ParticleSystemSimulationSpace.Local;

        velocity.x = new ParticleSystem.MinMaxCurve(
            -0.04f,
            0.04f
        );

        // Las luciérnagas subirán lentamente.
        velocity.y = new ParticleSystem.MinMaxCurve(
            0.01f,
            0.07f
        );

        velocity.z = new ParticleSystem.MinMaxCurve(
            -0.01f,
            0.01f
        );
    }

    private void ConfigurarParpadeo()
    {
        ParticleSystem.ColorOverLifetimeModule colorLifetime =
            sistemaParticulas.colorOverLifetime;

        colorLifetime.enabled = true;

        Gradient gradiente = new Gradient();

        GradientColorKey[] colores =
        {
            new GradientColorKey(Color.white, 0f),
            new GradientColorKey(Color.white, 1f)
        };

        GradientAlphaKey[] transparencias =
        {
            new GradientAlphaKey(0f, 0f),
            new GradientAlphaKey(1f, 0.12f),
            new GradientAlphaKey(0.20f, 0.35f),
            new GradientAlphaKey(1f, 0.55f),
            new GradientAlphaKey(0.15f, 0.78f),
            new GradientAlphaKey(0f, 1f)
        };

        gradiente.SetKeys(colores, transparencias);

        colorLifetime.color =
            new ParticleSystem.MinMaxGradient(gradiente);
    }

    private void ConfigurarTamañoDuranteVida()
    {
        ParticleSystem.SizeOverLifetimeModule sizeLifetime =
            sistemaParticulas.sizeOverLifetime;

        sizeLifetime.enabled = true;

        AnimationCurve curvaTamaño = new AnimationCurve(
            new Keyframe(0f, 0.2f),
            new Keyframe(0.15f, 1f),
            new Keyframe(0.40f, 0.45f),
            new Keyframe(0.60f, 1f),
            new Keyframe(0.80f, 0.35f),
            new Keyframe(1f, 0f)
        );

        sizeLifetime.size =
            new ParticleSystem.MinMaxCurve(1f, curvaTamaño);
    }

    private void ConfigurarRenderizador()
    {
        if (renderizadorParticulas == null)
            return;

        renderizadorParticulas.renderMode =
            ParticleSystemRenderMode.Billboard;

        renderizadorParticulas.alignment =
            ParticleSystemRenderSpace.View;

        if (materialLuciernaga != null)
        {
            renderizadorParticulas.sharedMaterial =
                materialLuciernaga;
        }
        else
        {
            Debug.LogWarning(
                "No has asignado el Material Luciernaga en el Inspector.",
                this
            );
        }
    }

    private void OnValidate()
    {
        // Evita ejecutar la configuración antes de iniciar el juego.
        if (!Application.isPlaying)
            return;

        sistemaParticulas = GetComponent<ParticleSystem>();
        renderizadorParticulas =
            GetComponent<ParticleSystemRenderer>();

        if (sistemaParticulas == null)
            return;

        // Aquí también guardamos cada módulo en una variable.
        ParticleSystem.ShapeModule shape =
            sistemaParticulas.shape;

        shape.scale = area;

        ParticleSystem.EmissionModule emission =
            sistemaParticulas.emission;

        emission.rateOverTime =
            new ParticleSystem.MinMaxCurve(particulasPorSegundo);

        ParticleSystem.MainModule main =
            sistemaParticulas.main;

        main.maxParticles = maximoParticulas;

        main.startLifetime = new ParticleSystem.MinMaxCurve(
            tiempoDeVida.x,
            tiempoDeVida.y
        );

        main.startSize = new ParticleSystem.MinMaxCurve(
            tamaño.x,
            tamaño.y
        );

        ParticleSystem.NoiseModule noise =
            sistemaParticulas.noise;

        noise.strengthX =
            new ParticleSystem.MinMaxCurve(intensidadRuido);

        noise.strengthY =
            new ParticleSystem.MinMaxCurve(intensidadRuido * 1.5f);

        noise.strengthZ =
            new ParticleSystem.MinMaxCurve(intensidadRuido * 0.3f);
    }
}