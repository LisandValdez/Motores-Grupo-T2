using System.Xml;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] protected int life_enemy;  
    [SerializeField] protected int damage;
    protected EnemyFSM fsm;
    public AudioClip hurtClip;

    public float hurtVolume = 1f;
    protected bool isDead;
    public bool IsDead => isDead;

    protected virtual void Awake()
    {
        if (fsm == null)
            fsm = GetComponent<EnemyFSM>();
    }

    public virtual void Atk_enemy()
    {
        Debug.Log("enemigo ataca");
    }

   public virtual void TakeDamage(int damage)
    {

        if (isDead) return;

        life_enemy -= damage;

        if (hurtClip != null)
            AudioSource.PlayClipAtPoint(hurtClip, transform.position, Mathf.Clamp01(hurtVolume));

        if (life_enemy <= 0)
        {
            isDead = true;
            Die();
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
