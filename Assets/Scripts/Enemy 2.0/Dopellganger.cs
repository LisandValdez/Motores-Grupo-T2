using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Dopellganger : Enemy
{
    [SerializeField] private Transform player;
    [SerializeField] private Animator anim;

    [SerializeField] private float distanciaAtaque; // stopping distance / rango de ataque
    [SerializeField] private float cooldownAtaque;
    [SerializeField] private Transform[] puntosPatrulla;
    [SerializeField] private float chaseSpeed;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.stoppingDistance = distanciaAtaque;
            agent.autoBraking = true;
        }

        if (fsm == null)
        {
            fsm = GetComponent<EnemyFSM>();
        }

        if (fsm == null || agent == null || player == null || anim == null)
        {
            Debug.LogError("Faltan referencias en Dopellganger. Revisa inspector.");
            return;
        }

        // ?? USAMOS EL GetCurrentChaseRange() dinámico heredado de Enemy
        fsm.ChangeState(new PatrolState(
            fsm,
            agent,
            player,
            anim,
            puntosPatrulla,
            GetCurrentChaseRange(),
            distanciaAtaque,
            cooldownAtaque,
            chaseSpeed
        ));
    }



    public override void Die()
    {
        isDead = true;
        fsm.ChangeState(new DeadState(agent, anim));
        Destroy(gameObject, 1f);
    }
}