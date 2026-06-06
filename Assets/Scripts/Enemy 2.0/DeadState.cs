using UnityEngine;
using UnityEngine.AI;

public class DeadState : EnemyState
{
    private NavMeshAgent agent;
    private Animator anim;

    public DeadState(NavMeshAgent agent, Animator anim)
    {
        this.agent = agent;
        this.anim = anim;
    }

    public override void Enter()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            
        }
        if (anim != null)
        {
            anim.SetTrigger("dead");
        }
    }

    public override void Update() { }

    public override void Exit() { }
}
