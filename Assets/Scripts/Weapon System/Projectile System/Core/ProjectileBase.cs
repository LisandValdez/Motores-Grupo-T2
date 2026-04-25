using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 50f;
    public float lifeTime = 3f;
    public bool destroyOnImpact = true;

    protected float damage;

    public virtual void Initialize(float weaponDamage)
    {
        damage = weaponDamage;
        Destroy(gameObject, lifeTime);
    }

    protected virtual void Update()
    {
        // Movimiento estándar hacia adelante
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        // 1. Intentamos obtener la interfaz (Solución más limpia)
        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Debug.Log($"Impacto en {other.name}: {damage} de daño.");
        }

        // 2. Si el objeto debe destruirse al chocar, lo hacemos después de aplicar daño
        if (destroyOnImpact)
        {
            Destroy(gameObject);
        }
    }
}