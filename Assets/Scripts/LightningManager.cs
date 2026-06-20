using UnityEngine;
using System.Collections;

// Esto obliga al GameObject a tener un AudioSource, evitando errores de configuración
[RequireComponent(typeof(AudioSource))]
public class LightingManager : MonoBehaviour
{
    public static LightingManager Instance { get; private set; }

    [Header("Configuración de Luces")]
    [Tooltip("Arrastra aquí la Directional Light o las luces principales del techo.")]
    public Light[] lucesPrincipales;

    [Header("Configuración de Iluminación Global")]
    public bool cambiarAmbientLight = true;
    public Color colorLuzEncendida = new Color(0.2f, 0.2f, 0.2f);
    public Color colorLuzApagada = Color.black;

    [Header("Configuración de la Niebla (Survival Horror)")]
    [Tooltip("¿Quieres que el sistema controle la niebla automáticamente?")]
    public bool usarNieblaEnApagon = true;
    [Tooltip("El color de la niebla cuando todo se apague (ej. Gris oscuro o verde rancio).")]
    public Color colorNieblaApagada = new Color(0.05f, 0.05f, 0.05f);
    [Tooltip("Qué tan densa será la niebla a oscuras. Valores entre 0.03 y 0.1 suelen ser muy ciegos.")]
    [Range(0f, 0.5f)] public float densidadNieblaApagada = 0.08f;
    [Tooltip("El modo de cálculo de la niebla. El más común y suave es Exponential Squared.")]
    public FogMode modoNiebla = FogMode.ExponentialSquared;

    [Header("Efectos de Sonido (NUEVO)")]
    [Tooltip("Sonido de cortocircuito o explosión de fusibles cuando se corta la luz.")]
    [SerializeField] private AudioClip sonidoApagon;
    [Tooltip("Sonido de generador arrancando o luces parpadeando al volver la energía.")]
    [SerializeField] private AudioClip sonidoRestauracion;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Obtenemos la referencia al componente AudioSource
        audioSource = GetComponent<AudioSource>();
        // Lo configuramos por código para que no sea afectado por el espacio 3D (sonido global)
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        // El juego siempre inicia iluminado y sin la niebla del apagón
        SetLightingState(true);

        // Nos suscribimos automáticamente al evento de la caja de fusibles
        FuseBox fuseBox = FindFirstObjectByType<FuseBox>();
        if (fuseBox != null)
        {
            fuseBox.OnFusePlaced += IniciarRestauracionConRetraso;
        }
    }

    private void OnDestroy()
    {
        FuseBox fuseBox = FindFirstObjectByType<FuseBox>();
        if (fuseBox != null)
        {
            fuseBox.OnFusePlaced -= IniciarRestauracionConRetraso;
        }
    }

    // Apaga las luces de inmediato e inyecta la niebla
    public void ApagarLuces()
    {
        SetLightingState(false);
        Debug.Log("🔌 [LIGHTING] Apagón general provocado por la historia. Niebla activada.");

        // 🔊 REPRODUCIR SONIDO DEL APAGÓN
        if (audioSource != null && sonidoApagon != null)
        {
            audioSource.PlayOneShot(sonidoApagon);
        }
    }

    // Escucha el evento del fusible e inicia la espera
    private void IniciarRestauracionConRetraso()
    {
        
        StartCoroutine(RoutineRestaurarEnergia(1.0f));
    }

    // ⏳ CORRUTINA: Espera antes de encender todo
    private IEnumerator RoutineRestaurarEnergia(float tiempoEspera)
    {
        Debug.Log($"⚡ [LIGHTING] Fusible colocado. Arrancando generadores, esperando {tiempoEspera}s...");

        // Esperamos el tiempo real de los generadores
        yield return new WaitForSecondsRealtime(tiempoEspera);

        SetLightingState(true);
        Debug.Log("💡 [LIGHTING] ¡Energía restaurada y niebla disipada con éxito!");

        // 🔊 REPRODUCIR SONIDO DE RESTAURACIÓN
        if (audioSource != null && sonidoRestauracion != null)
        {
            audioSource.PlayOneShot(sonidoRestauracion);
        }
    }

    private void SetLightingState(bool encendido)
    {
        // 1. Luces físicas
        foreach (Light luz in lucesPrincipales)
        {
            if (luz != null) luz.enabled = encendido;
        }

        // 2. Luz ambiental (sombras y entorno)
        if (cambiarAmbientLight)
        {
            RenderSettings.ambientLight = encendido ? colorLuzEncendida : colorLuzApagada;
        }

        // 3. Intensidad del Skybox
        RenderSettings.ambientIntensity = encendido ? 1f : 0f;
        RenderSettings.reflectionIntensity = encendido ? 1f : 0f;

        // 🌫️ 4. CONTROL DE NIEBLA DINÁMICA
        if (usarNieblaEnApagon)
        {
            if (!encendido)
            {
                RenderSettings.fog = true;
                RenderSettings.fogMode = modoNiebla;
                RenderSettings.fogColor = colorNieblaApagada;
                RenderSettings.fogDensity = densidadNieblaApagada;
            }
            else
            {
                RenderSettings.fog = false;
            }
        }
    }
}