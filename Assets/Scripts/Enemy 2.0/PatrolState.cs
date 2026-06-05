using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PatrolState : EnemyState
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

    private int currentIndex = 0;

    public PatrolState(
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
        agent.speed = chaseSpeed * 0.6f;
        GoToNextPoint();
        Debug.Log("Entró a PatrolState");
    }

    private void GoToNextPoint()
    {
        if (puntos == null || puntos.Length == 0) return;
        agent.SetDestination(puntos[currentIndex].position);
        currentIndex = (currentIndex + 1) % puntos.Length;
    }

    public override void Update()
    {
        if (agent == null || player == null) return;

        float distToPlayer = Vector3.Distance(agent.transform.position, player.position);
        if (distToPlayer <= chaseRange)
        {
            fsm.ChangeState(new ChaseState(fsm, agent, player, anim, puntos, chaseRange, attackRange, cooldown, chaseSpeed));
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            GoToNextPoint();
        }

        if (anim != null)
            anim.SetFloat("speed", agent.velocity.magnitude);
    }

    public override void Exit()
    {
        if (agent == null) return;
        agent.ResetPath();
        Debug.Log("PatrolState.Exit()");
    }
}
