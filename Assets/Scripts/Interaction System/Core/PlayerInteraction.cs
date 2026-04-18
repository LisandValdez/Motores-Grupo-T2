using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Parameters")]
    [SerializeField] private float distance = 6f;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float cooldown = 1.5f;

    // Guarda Cooldowns por objeto
    private Dictionary<GameObject, float> cooldowns = new Dictionary<GameObject, float>();

    //Invoke Unity Events
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryInteract();
        }
    }

    private void Update()
    {
        UpdateCooldowns();
    }

    private void TryInteract()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.SphereCast(ray, radius, out RaycastHit hit, distance))
        {
            GameObject target = hit.collider.gameObject;

            // Se fija si el objeto esta en Cooldown
            if (cooldowns.ContainsKey(target) && cooldowns[target] > 0)
                return;

            IInteractable interactable = target.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();

                // Cooldown de objeto
                cooldowns[target] = cooldown;
            }
        }
    }

    private void UpdateCooldowns()
    {
        List<GameObject> keys = new List<GameObject>(cooldowns.Keys);
        foreach (GameObject obj in keys)
        {
            cooldowns[obj] -= Time.deltaTime;
            if (cooldowns[obj] <= 0)
            {
                cooldowns.Remove(obj);
            }
        }
    }

    // Gizmos raycast 
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(transform.position, transform.forward * distance);
        Gizmos.DrawWireSphere(transform.position + transform.forward * distance, radius);
    }
}