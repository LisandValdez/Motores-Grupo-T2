using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 60;
    [SerializeField] private int currentHealth;
    
    [Header("Efectos")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;
    
    [Header("Componentes")]
    [SerializeField] private EnemigoCompleto enemyAI;
    [SerializeField] private Collider enemyCollider;
    [SerializeField] private Rigidbody rb;
    
    public System.Action<int, int> OnHealthChanged;
    public System.Action OnEnemyDied;
    
    private bool isDead = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        
        if (enemyAI == null) enemyAI = GetComponent<EnemigoCompleto>();
        if (enemyCollider == null) enemyCollider = GetComponent<Collider>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        
        Debug.Log($"Enemigo iniciado con {currentHealth}/{maxHealth} de vida");
    }
    
    public void TakeDamage(float amount)
    {
        TakeDamage(Mathf.RoundToInt(amount));
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) 
        {
            Debug.Log("Enemigo ya está muerto, no recibe más daño");
            return;
        }
        
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        Debug.Log($"💥 Enemigo recibió {damage} de daño. Vida: {currentHealth}/{maxHealth}");
        
        if (hurtSound != null)
            AudioSource.PlayClipAtPoint(hurtSound, transform.position, 1f);
        
        StartCoroutine(FlashRed());
        
        // 🔥 VERIFICAR MUERTE CORRECTAMENTE
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    
    void Die()
    {
        isDead = true;
        Debug.Log("💀 Enemigo murió!");
        
        // Desactivar componentes
        if (enemyAI != null)
        {
            enemyAI.enabled = false;
            Debug.Log("EnemyAI desactivado");
        }
        
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
            Debug.Log("Collider desactivado");
        }
        
        if (rb != null)
        {
            rb.isKinematic = true;
            Debug.Log("Rigidbody desactivado");
        }
        
        // Efectos visuales
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Debug.Log("Efecto de muerte instanciado");
        }
        
        // Sonido
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position, 1f);
            Debug.Log("Sonido de muerte reproducido");
        }
        
        OnEnemyDied?.Invoke();
        
        // Destruir el objeto después de un tiempo
        Debug.Log($"Destruyendo {gameObject.name} en 2 segundos...");
        Destroy(gameObject);
    }
    
    IEnumerator FlashRed()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            renderer.material.color = originalColor;
        }
    }
    
    public void Heal(int amount)
    {
        if (isDead) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"💚 Enemigo curó {amount}. Vida: {currentHealth}/{maxHealth}");
    }
    
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsDead() => isDead;
}