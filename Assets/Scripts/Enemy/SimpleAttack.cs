using UnityEngine;

public class SimpleEnemyAttack : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerHealth playerHealth;  // Arrastrar manualmente
    
    [Header("Configuración")]
    public int damage = 1;
    public float attackRange = 3f;
    public float attackCooldown = 1f;
    
    private float lastAttackTime = 0f;
    
    void Start()
    {
        // Si no se asignó manualmente, buscar automáticamente
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
            
            if (playerHealth != null)
                Debug.Log($"✅ PlayerHealth encontrado automáticamente en: {playerHealth.gameObject.name}");
            else
                Debug.LogError("❌ No se encontró PlayerHealth! Arrástralo manualmente al inspector.");
        }
    }
    
    void Update()
    {
        if (playerHealth == null) return;
        
        float distance = Vector3.Distance(transform.position, playerHealth.transform.position);
        
        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }
    
    void Attack()
    {
        lastAttackTime = Time.time;
        playerHealth.TakeDamage(damage);
        Debug.Log($"⚔️ Ataque! Vida: {playerHealth.GetCurrentHealth()}/{playerHealth.GetMaxHealth()}");
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}