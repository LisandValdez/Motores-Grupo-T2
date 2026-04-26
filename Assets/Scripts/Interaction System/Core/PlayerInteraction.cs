using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Parameters")]
    [SerializeField] private float distance = 3f;  // Reducido de 6 a 3
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float interactionCooldown = 0.5f;
    
    [Header("Debug")]
    [SerializeField] private bool showDebug = true;
    
    private float lastInteractionTime = 0f;
    private GameObject lastHitObject;
    
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Time.time < lastInteractionTime + interactionCooldown)
            {
                if (showDebug) Debug.Log("⏰ Interacción en cooldown");
                return;
            }
            
            TryInteract();
        }
    }
    
    private void TryInteract()
    {
        lastInteractionTime = Time.time;
        
        // MÉTODO 1: Raycast (para mirar directamente)
        Ray ray = new Ray(transform.position, transform.forward);
        QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;
        
        if (Physics.Raycast(ray, out RaycastHit hit, distance, ~0, triggerInteraction))
        {
            GameObject target = hit.collider.gameObject;
            if (showDebug) Debug.Log($"🎯 Raycast hit: {target.name}");
            
            IInteractable interactable = GetInteractableInHierarchy(target);
            if (interactable != null)
            {
                if (showDebug) Debug.Log($"✅ Interactuando con: {target.name}");
                interactable.Interact();
                return;
            }
        }
        
        // MÉTODO 2: SphereCast (más ancho)
        if (Physics.SphereCast(ray, radius, out hit, distance, ~0, triggerInteraction))
        {
            GameObject target = hit.collider.gameObject;
            if (showDebug) Debug.Log($"🎯 SphereCast hit: {target.name}");
            
            IInteractable interactable = GetInteractableInHierarchy(target);
            if (interactable != null)
            {
                if (showDebug) Debug.Log($"✅ Interactuando con: {target.name}");
                interactable.Interact();
                return;
            }
        }
        
        // MÉTODO 3: OverlapSphere (para buscar alrededor)
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 2f, 2f, ~0, QueryTriggerInteraction.Collide);
        foreach (Collider col in hitColliders)
        {
            IInteractable interactable = GetInteractableInHierarchy(col.gameObject);
            if (interactable != null)
            {
                float distToObject = Vector3.Distance(transform.position, col.transform.position);
                if (distToObject <= distance * 0.8f)
                {
                    if (showDebug) Debug.Log($"✅ OverlapSphere encontrado: {col.name}");
                    interactable.Interact();
                    return;
                }
            }
        }
        
        if (showDebug) Debug.Log("❌ No se encontró nada interactuable");
    }
    
    private IInteractable GetInteractableInHierarchy(GameObject obj)
    {
        // Buscar en el objeto
        IInteractable interactable = obj.GetComponent<IInteractable>();
        if (interactable != null) return interactable;
        
        // Buscar en el padre
        if (obj.transform.parent != null)
            interactable = obj.transform.parent.GetComponent<IInteractable>();
        if (interactable != null) return interactable;
        
        // Buscar en el hijo
        interactable = obj.GetComponentInChildren<IInteractable>();
        return interactable;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * distance);
        Gizmos.DrawWireSphere(transform.position + transform.forward * distance, radius);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 2f, 2f);
    }
}