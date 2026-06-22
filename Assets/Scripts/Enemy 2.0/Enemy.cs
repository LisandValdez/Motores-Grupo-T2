using System.Xml;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Estadísticas Base")]
    [SerializeField] protected int life_enemy;
    [SerializeField] protected int damage;
    protected EnemyFSM fsm;

    [Header("Configuración de Rangos Dinámicos")]
    [Tooltip("El rango de persecución por defecto que usa el enemigo.")]
    [SerializeField] protected float chaseRangeBase = 8f;
    [Tooltip("A qué cantidad aumentará el rango de persecución si el enemigo recibe daño estando lejos.")]
    [SerializeField] protected float chaseRangeAlRecibirDanio = 15f;
    [Tooltip("Cuánto tiempo (en segundos) dura el rango aumentado antes de volver al base.")]
    [SerializeField] private float tiempoRangoAumentado = 10f;

    protected float currentChaseRange;

    // Variables internas para controlar el temporizador
    private float tiempoParaRestablecerRango;
    private bool tieneRangoAumentado = false;

    [Header("Audio")]
    public AudioClip hurtClip;
    [Tooltip("Arrastra aquí el sonido que hará el enemigo al morir.")]
    public AudioClip deathClip;
    [Tooltip("Sonido que se reproducirá una sola vez cuando detecte al jugador.")]
    public AudioClip alertClip;

    protected bool isDead;
    public bool IsDead => isDead;

    [HideInInspector] public bool yaAlertoAlPlayer = false;

    protected AudioSource audioSource;

    protected virtual void Awake()
    {
        if (fsm == null)
            fsm = GetComponent<EnemyFSM>();

        audioSource = GetComponent<AudioSource>();
        currentChaseRange = chaseRangeBase;
    }

    // NUEVO: El Update del script base ahora controla el tiempo de alerta
    protected virtual void Update()
    {
        if (isDead) return;

        // Si el rango está aumentado, vigilamos que no se agote el tiempo
        if (tieneRangoAumentado)
        {
            // Si el enemigo ya detectó al jugador y pasó a ChaseState, congelamos el temporizador
            // para que no se le baje el rango en medio de una persecución activa
            if (fsm != null && fsm.currentState is ChaseState)
            {
                // Actualizamos el tiempo límite para que los 10 segundos empiecen a contar 
                // RECIÉN cuando el jugador logre escapar de su vista
                tiempoParaRestablecerRango = Time.time + tiempoRangoAumentado;
                return;
            }

            // Si pasa el tiempo configurado (10s) y sigue en patrulla, vuelve a la normalidad
            if (Time.time >= tiempoParaRestablecerRango)
            {
                ResetChaseRange();
                Debug.Log($"🍃 [IA] El enemigo {gameObject.name} se cansó de buscar. Rango restablecido a {chaseRangeBase}.");
            }
        }
    }

    public virtual void Atk_enemy()
    {
        Debug.Log("enemigo ataca");
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        // LÓGICA DE ALERTA POR DAÑO DISTANTE
        if (fsm != null && fsm.currentState is PatrolState)
        {
            Debug.Log($"🏹 [IA] Enemigo dañado en sigilo. ¡Rango aumentado a {chaseRangeAlRecibirDanio} por {tiempoRangoAumentado} segundos!");
            currentChaseRange = chaseRangeAlRecibirDanio;

            // Calculamos el frame exacto del futuro en el que debe apagarse la alerta
            tiempoParaRestablecerRango = Time.time + tiempoRangoAumentado;
            tieneRangoAumentado = true;
        }

        life_enemy -= damage;

        if (life_enemy <= 0)
        {
            isDead = true;

            if (audioSource != null && deathClip != null)
                audioSource.PlayOneShot(deathClip);

            Die();
        }
        else
        {
            if (audioSource != null && hurtClip != null)
                audioSource.PlayOneShot(hurtClip);
        }
    }

    public virtual void Spawn_enemy()
    {
        Debug.Log("spawneo enemigo");
    }

    public virtual void Die()
    {
        isDead = true;
        if (fsm != null)
            fsm.ChangeState(new DeadState(GetComponent<NavMeshAgent>(), GetComponent<Animator>()));
    }

    public float GetCurrentChaseRange() => currentChaseRange;

    // Modificamos el reset para apagar la bandera del temporizador
    public void ResetChaseRange()
    {
        currentChaseRange = chaseRangeBase;
        tieneRangoAumentado = false;
    }

    public void PlayAlertSound()
    {
        if (!yaAlertoAlPlayer && audioSource != null && alertClip != null)
        {
            audioSource.PlayOneShot(alertClip);
            yaAlertoAlPlayer = true;
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.15f);
        Gizmos.DrawWireSphere(transform.position, chaseRangeBase);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.15f);
        Gizmos.DrawWireSphere(transform.position, chaseRangeAlRecibirDanio);

        if (Application.isPlaying)
        {
            // El aro magenta cambiará de tamaño en tiempo real en la escena
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, currentChaseRange);
        }
    }
}