using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class AttackState : EnemyState
{
    private EnemyFSM fsm;
    private NavMeshAgent agent;
    private Transform player;
    private Animator anim;
    private Transform[] puntos;

    private float chaseRange;
    private float attackRange;
    private float cooldown;
    private float lastAttack;
    private IDamageable targetDamageable;
    private float chaseSpeed;

    public AttackState(
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
        if (agent == null || player == null) return;
        Debug.Log("Entró a AttackState");

        agent.isStopped = true;
        agent.ResetPath();
        agent.updateRotation = false;

        // permitir ataque inmediato si corresponde
        lastAttack = Time.time - cooldown;

        targetDamageable = player.GetComponent<IDamageable>();

        // girar hacia el jugador
        Vector3 dir = player.position - agent.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            agent.transform.rotation = Quaternion.LookRotation(dir.normalized);

        // disparar animación si existe el trigger
        if (anim != null) anim.SetTrigger("atk");
    }

    public override void Update()
    {
        if (agent == null || player == null) return;

        float distance = Vector3.Distance(agent.transform.position, player.position);

        // si el jugador está fuera del chaseRange, volver a patrulla
        if (distance > chaseRange)
        {
            agent.updateRotation = true;
            fsm.ChangeState(new PatrolState(fsm, agent, player, anim, puntos, chaseRange, attackRange, cooldown, chaseSpeed));
            return;
        }

        // si se aleja más que el rango de ataque, volver a chase
        if (distance > attackRange)
        {
            agent.updateRotation = true;
            fsm.ChangeState(new ChaseState(fsm, agent, player, anim, puntos, chaseRange, attackRange, cooldown, chaseSpeed));
            return;
        }

        // atacar por proximidad cuando pase el cooldown
        if (distance <= attackRange && Time.time - lastAttack >= cooldown)
        {
            lastAttack = Time.time;
            Debug.Log("AttackState: aplicando ataque por proximidad");
            if (targetDamageable != null)
                targetDamageable.TakeDamage(1);
            else
                Debug.LogWarning("AttackState: jugador no implementa IDamageable");

            if (anim != null)
            {
                anim.ResetTrigger("atk");
                anim.SetTrigger("atk");
            }
        }
    }

    public override void Exit()
    {
        if (agent == null) return;
        agent.isStopped = false;
        agent.ResetPath();
        agent.updateRotation = true;
        agent.autoBraking = true;
        Debug.Log("AttackState.Exit()");
    }
}