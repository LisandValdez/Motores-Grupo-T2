using UnityEngine;

public class DetectarVision 

{
    private Transform eye;
    private float viewAngle;// angulo de vision
    private float viewDistance; //distancia a la que puede ver
    private LayerMask obstacleMask;
    private LayerMask playerMask;

    public DetectarVision(Transform eye, float viewAngle, float viewDistance, LayerMask obstacleMask, LayerMask playerMask)
    {
        this.eye = eye;
        this.viewAngle = viewAngle;
        this.viewDistance = viewDistance;
        this.obstacleMask = obstacleMask;
        this.playerMask = playerMask;
    }

    // Comprueba si el target es visible desde 'eye'
    public bool CanSee(Transform target)
    {
        if (eye == null || target == null) return false;

        Vector3 dirToTarget = (target.position - eye.position);
        float dist = dirToTarget.magnitude;
        if (dist > viewDistance) return false;

        // Ángulo de visión
        Vector3 forward = eye.forward;
        float angle = Vector3.Angle(forward, dirToTarget);
        if (angle > viewAngle * 0.5f) return false;

        // Raycast para comprobar oclusión
        Ray ray = new Ray(eye.position, dirToTarget.normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, viewDistance, obstacleMask | playerMask))
        {
            // Si el primer collider alcanzado pertenece al playerMask -> visible
            if (((1 << hit.collider.gameObject.layer) & playerMask) != 0)
                return true;
            return false; // golpeó un obstáculo primero
        }

        return false;
    }

    // Opcional: versión que devuelve distancia y ángulo para debug
    public (bool seen, float distance, float angle) CanSeeDetailed(Transform target)
    {
        if (eye == null || target == null) return (false, Mathf.Infinity, Mathf.Infinity);
        Vector3 dirToTarget = (target.position - eye.position);
        float dist = dirToTarget.magnitude;
        float angle = Vector3.Angle(eye.forward, dirToTarget);
        bool seen = CanSee(target);
        return (seen, dist, angle);
    }
}