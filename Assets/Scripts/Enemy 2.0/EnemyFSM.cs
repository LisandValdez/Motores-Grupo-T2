using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    public EnemyState currentState;
    private bool changingState;
    private EnemyState pendingState;

    private void Update()
    {
        currentState?.Update();
    }

    public void ChangeState(EnemyState newState)
    {
        if (newState == null) return;

        // Si ya estamos cambiando, encolamos la petición
        if (changingState)
        {
            pendingState = newState;
            return;
        }

        changingState = true;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();

        changingState = false;

        // Si durante Enter/Exit se solicitó otra transición, procesarla ahora
        if (pendingState != null)
        {
            EnemyState next = pendingState;
            pendingState = null;
            ChangeState(next);
        }
    }
}
