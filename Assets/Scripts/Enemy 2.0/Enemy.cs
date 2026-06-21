using System.Xml;
using UnityEngine;
using UnityEngine.AI;

// Asegura que todo enemigo tenga su propio AudioSource en la escena
[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Estadísticas")]
    [SerializeField] protected int life_enemy;
    [SerializeField] protected int damage;
    protected EnemyFSM fsm;

    [Header("Audio (NUEVO CONTROL)")]
    public AudioClip hurtClip;
    [Tooltip("Arrastra aquí el sonido que hará el enemigo al morir.")]
    public AudioClip deathClip; // ¡Añadido para que también manejes la muerte!

    protected bool isDead;
    public bool IsDead => isDead;

    // Referencia interna al componente de audio
    protected AudioSource audioSource;

    protected virtual void Awake()
    {
        if (fsm == null)
            fsm = GetComponent<EnemyFSM>();

        // Conseguimos el componente de audio del propio enemigo
        audioSource = GetComponent<AudioSource>();
    }

    public virtual void Atk_enemy()
    {
        Debug.Log("enemigo ataca");
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        life_enemy -= damage;

        if (life_enemy <= 0)
        {
            isDead = true;

            // 💀 REPRODUCIR SONIDO DE MUERTE
            if (audioSource != null && deathClip != null)
                audioSource.PlayOneShot(deathClip);

            Die();
        }
        else
        {
            // 🩸 REPRODUCIR SONIDO DE DAÑO
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
}