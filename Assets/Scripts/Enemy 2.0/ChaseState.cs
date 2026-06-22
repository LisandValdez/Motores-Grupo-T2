using UnityEngine;
using UnityEngine.AI;

public class ChaseState : EnemyState
{
    private EnemyFSM fsm;
    private NavMeshAgent agent;
    private Transform player;
    private Animator anim;
    private Transform[] puntos;

    private float chaseRange;
    private float attackRange;
    private float cooldown;
    private float chaseSpeed;

    public ChaseState(
        EnemyFSM fsm,
        NavMeshAgent agent,
        Transform player,
        Animator anim,
        Transform[] puntos,
        float chaseRange,
        float attackRange,
        float cooldown,
        float chaseSpeed
    )
    {
        this.fsm = fsm;
        this.agent = agent;
        this.player = player;
        this.anim = anim;
        this.puntos = puntos;
        this.chaseRange = chaseRange;
        this.attackRange = attackRange;
        this.cooldown = cooldown;
        this.chaseSpeed = chaseSpeed;
    }

    public override void Enter()
    {
        if (agent == null) return;
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.autoBraking = true;
        Debug.Log("Entró a ChaseState");
    }

    public override void Update()
    {
        if (agent == null || player == null) return;

        // ?? ACTUALIZAMOS EL CHASE RANGE SEGÚN EL ESTADO ACTUAL DEL ENEMIGO
        Enemy enemyComponent = agent.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            chaseRange = enemyComponent.GetCurrentChaseRange();
        }

        float distToPlayer = Vector3.Distance(agent.transform.position, player.position);

        // Si el jugador logra escapar de la distancia de persecución activa
        if (distToPlayer > chaseRange)
        {
            // ?? El enemigo pierde al jugador: restablece el sonido de alerta y su rango al valor base
            if (enemyComponent != null)
            {
                enemyComponent.yaAlertoAlPlayer = false;
                enemyComponent.ResetChaseRange();
            }

            fsm.ChangeState(new PatrolState(fsm, agent, player, anim, puntos, chaseRange, attackRange, cooldown, chaseSpeed));
            return;
        }

        // Actualizar destino solo si cambió significativamente
        float destUpdateThreshold = 0.5f;
        if (!agent.hasPath || Vector3.Distance(agent.destination, player.position) > destUpdateThreshold)
        {
            agent.SetDestination(player.position);
        }

        // Evitar overshoot: usar remainingDistance
        if (agent.hasPath && !agent.pathPending)
        {
            float remaining = agent.remainingDistance;
            if (remaining <= agent.stoppingDistance + 0.2f)
            {
                agent.isStopped = true;
                agent.ResetPath();

                // si está dentro de attackRange, cambiar a AttackState
                if (distToPlayer <= attackRange)
                {
                    fsm.ChangeState(new AttackState(fsm, agent, player, anim, puntos, chaseRange, attackRange, cooldown, chaseSpeed));
                    return;
                }
            }
        }

        // animación de movimiento
        if (anim != null)
            anim.SetFloat("speed", agent.velocity.magnitude);
    }

    public override void Exit()
    {
        if (agent == null) return;
        agent.ResetPath();
        Debug.Log("ChaseState.Exit()");
    }
}