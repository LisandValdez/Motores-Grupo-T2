using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 80f;
    public float lifeTime = 5f;
    public float maxDistance = 500f;
    public bool destroyOnImpact = true;
    public int damage = 20;

    [Header("Effects")]
    [SerializeField] protected ParticleSystem bloodParticlePrefab; // Asigna aquí tu prefab de cubos

    private Vector3 lastPosition;
    private Vector3 startPosition;
    private bool hasHit = false;
    private Collider projectileCollider;

    public virtual void Initialize(float weaponDamage)
    {
        damage = Mathf.RoundToInt(weaponDamage);
        startPosition = transform.position;
        lastPosition = transform.position;
        Destroy(gameObject, lifeTime);

        projectileCollider = GetComponent<Collider>();

        // Ignorar colisión con el jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && projectileCollider != null)
        {
            Collider playerCollider = player.GetComponent<Collider>();
            if (playerCollider != null)
            {
                Physics.IgnoreCollision(projectileCollider, playerCollider);
                Debug.Log("🔧 Bala ignora colisión con jugador");
            }
        }
    }

    protected virtual void Update()
    {
        if (hasHit) return;

        Vector3 currentPosition = transform.position;
        Vector3 newPosition = currentPosition + transform.forward * speed * Time.deltaTime;

        // Raycast para detectar colisiones entre frames
        Vector3 direction = (newPosition - currentPosition).normalized;
        float distance = Vector3.Distance(currentPosition, newPosition);

        RaycastHit hit;
        // Ignorar la capa del jugador (Layer 3)
        int layerMask = ~LayerMask.GetMask("Player");

        if (Physics.Raycast(currentPosition, direction, out hit, distance, layerMask))
        {
            // Verificar que no es el propio proyectil
            if (hit.collider == projectileCollider)
            {
                transform.position = newPosition;
                lastPosition = newPosition;
                return;
            }

            Debug.Log($"💥 Raycast impactó: {hit.collider.gameObject.name} (Layer: {hit.collider.gameObject.layer})");

            transform.position = hit.point;

            // Enviamos el RaycastHit completo para tener la posición y la normal de la superficie
            HandleHit(hit);
            return;
        }

        // Movimiento normal
        transform.position = newPosition;
        lastPosition = newPosition;

        // Distancia máxima
        if (Vector3.Distance(startPosition, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void HandleHit(RaycastHit hit)
    {
        if (hasHit) return;
        hasHit = true;

        Collider hitCollider = hit.collider;
        Debug.Log($"💥 Proyectil impactó en: {hitCollider.gameObject.name}");

        // Buscar IDamageable
        IDamageable damageable = hitCollider.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Debug.Log($"  ✅ {damage} de daño aplicado");

            // Si el objetivo tiene vida, instanciamos la sangre en el punto de impacto
            SpawnBloodEffect(hit.point, hit.normal);
        }
        else
        {
            Debug.Log($"  ❌ {hitCollider.name} no tiene IDamageable");
        }

        // Efectos de impacto adicionales
        OnImpact(hit);

        if (destroyOnImpact)
        {
            Destroy(gameObject);
        }
    }

    private void SpawnBloodEffect(Vector3 position, Vector3 normal)
    {
        if (bloodParticlePrefab == null) return;

        // Instancia las partículas alineadas con la normal de la superficie (hacia afuera)
        ParticleSystem fx = Instantiate(bloodParticlePrefab, position, Quaternion.LookRotation(normal));

        // Destruye el clon automáticamente en base a su duración para no saturar la jerarquía
        Destroy(fx.gameObject, fx.main.duration + fx.main.startLifetime.constantMax);
    }

    protected virtual void OnImpact(RaycastHit hit)
    {
        // Sobrescribir en scripts hijos si necesitas efectos extra (ej: chispas en metal, agujeros de bala)
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
    }
}