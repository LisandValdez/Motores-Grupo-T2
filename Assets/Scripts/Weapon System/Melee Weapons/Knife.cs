using UnityEngine;

public class Knife : MeleeBase
{
    public override void Attack(Vector3 targetPoint)
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + (1f / fireRate);

            // Usamos el attackRange heredado de MeleeBase
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, attackRange))
            {
                Debug.Log($"Cortaste a: {hit.collider.name} haciendo {damage} de daño.");
            }
        }
    }
    // No necesitamos Aim() ni Reload() aquí.
}