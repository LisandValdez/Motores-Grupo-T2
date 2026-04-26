using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 80f;
    public float lifeTime = 8f;
    public float maxDistance = 500f;
    public bool destroyOnImpact = true;
    public bool ignorePlayerCollision = true;
    
    [Header("Detección Avanzada")]
    public bool useRaycastDetection = true;
    public float detectionRadius = 0.2f;
    
    protected float damage;
    private Vector3 lastPosition;
    private Vector3 startPosition;
    private Collider projectileCollider;
    private bool hasHit = false;

    public virtual void Initialize(float weaponDamage, GameObject owner = null)
    {
        damage = weaponDamage;
        startPosition = transform.position;
        lastPosition = transform.position;
        Destroy(gameObject, lifeTime);
        
        projectileCollider = GetComponent<Collider>();
        
        // Ignorar colisión con el jugador
        if (ignorePlayerCollision && owner != null)
        {
            Collider ownerCollider = owner.GetComponent<Collider>();
            if (ownerCollider != null && projectileCollider != null)
            {
                Physics.IgnoreCollision(projectileCollider, ownerCollider);
            }
        }
    }

    protected virtual void Update()
    {
        if (hasHit) return;
        
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = currentPosition + transform.forward * speed * Time.deltaTime;
        
        if (useRaycastDetection)
        {
            Vector3 direction = (newPosition - currentPosition).normalized;
            float distance = Vector3.Distance(currentPosition, newPosition);
            
            // Raycast con capas filtradas
            RaycastHit hit;
            int layerMask = ~LayerMask.GetMask("Projectile", "Player"); // Ignorar balas y jugador
            
            if (Physics.Raycast(currentPosition, direction, out hit, distance, layerMask))
            {
                // Ignorar si es la misma bala
                if (hit.collider == projectileCollider) return;
                
                // Ignorar si es otra bala
                if (hit.collider.CompareTag("Bullet")) return;
                if (hit.collider.GetComponent<ProjectileBase>() != null) return;
                
                transform.position = hit.point;
                HandleHit(hit.collider);
                return;
            }
        }
        
        transform.position = newPosition;
        lastPosition = transform.position;
        
        if (Vector3.Distance(startPosition, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        
        // 🔥 IGNORAR LA PROPIA BALA
        if (other == projectileCollider) return;
        
        // 🔥 IGNORAR OTRAS BALAS
        if (other.CompareTag("Bullet")) return;
        if (other.GetComponent<ProjectileBase>() != null) return;
        
        // Ignorar al jugador
        if (other.CompareTag("Player")) return;
        
        HandleHit(other);
    }
    
    protected virtual void HandleHit(Collider hitCollider)
    {
        if (hasHit) return;
        
        hasHit = true;
        
        Debug.Log($"💥 Proyectil impactó en: {hitCollider.gameObject.name}");
        
        IDamageable damageable = hitCollider.GetComponentInParent<IDamageable>();
        
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Debug.Log($"  ✅ {damage} de daño aplicado");
        }
        
        OnImpact(hitCollider);
        
        if (destroyOnImpact)
        {
            Destroy(gameObject);
        }
    }
    
    protected virtual void OnImpact(Collider hitCollider)
    {
        // Sobrescribir para efectos especiales
    }
}