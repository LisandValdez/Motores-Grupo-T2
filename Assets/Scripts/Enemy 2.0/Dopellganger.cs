using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Dopellganger : Enemy
{
    [SerializeField] private Transform player;
    [SerializeField] private Animator anim;

    [SerializeField] private float distanciaAtaque; // stopping distance / rango de ataque
    [SerializeField] private float cooldownAtaque;
    [SerializeField] private float chaseRange;
    [SerializeField] private Transform[] puntosPatrulla;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private EnemyAudio enemyAudio;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            // Ajustá esto en el Inspector si preferís otro valor
            agent.stoppingDistance = distanciaAtaque;
            agent.autoBraking = true;
        }

        if (fsm == null)
        {
            fsm = GetComponent<EnemyFSM>();
        }

        // Validar referencias antes de iniciar
        if (fsm == null || agent == null || player == null || anim == null)
        {
            Debug.LogError("Faltan referencias en Dopellganger. Revisa inspector.");
            return;
        }

        // Iniciar FSM en PatrolState (sin visión)
        fsm.ChangeState(new PatrolState(
            fsm,
            agent,
            player,
            anim,
            puntosPatrulla,
            chaseRange,
            distanciaAtaque,
            cooldownAtaque,
            chaseSpeed
        ));
    }

    private void OnDrawGizmos()
    {
        // Dibuja radios de chase y ataque en Scene para debug
        if (player == null) return;

        Gizmos.color = new Color(1f, 1f, 0f, 0.12f);
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = new Color(1f, 0f, 0f, 0.12f);
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);

        // línea al jugador
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, player.position);
    }

    public override void Die()
    {
        isDead = true;
        enemyAudio?.PlayDie();
        fsm.ChangeState(new DeadState(agent, anim));
        Destroy(gameObject, 1f);
    }
}
