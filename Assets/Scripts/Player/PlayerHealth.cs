using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;
    
    [Header("UI de Corazones")]
    [SerializeField] private GameObject heartsPanel;
    [SerializeField] private Image[] heartImages;
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;
    
    [Header("Efectos")]
    [SerializeField] private GameObject damageEffect;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deathSound;
    
    [Header("Invulnerabilidad")]
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private float blinkInterval = 0.1f;
    
    [Header("Game Over")]
    [SerializeField] private GameOverManager gameOverManager;  // ← AGREGAR ESTO
    
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Animator animator;
    
    public System.Action OnPlayerDamaged;
    public System.Action OnPlayerDied;
    public System.Action<int> OnHealthChanged;
    
    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
        
        //Buscar GameOverManager si no está asignado
        if (gameOverManager == null)
        {
           gameOverManager = FindFirstObjectByType<GameOverManager>();
           if (gameOverManager != null)
               Debug.Log("✅ GameOverManager encontrado automáticamente");
           else
               Debug.LogWarning("⚠️ No se encontró GameOverManager en la escena");
        }
        
        InitializeHeartUI();
        UpdateHeartUI();
    }
    
    void InitializeHeartUI()
    {
        if (heartsPanel == null)
        {
            heartsPanel = GameObject.Find("HeartsPanel");
            if (heartsPanel == null)
            {
                Debug.LogWarning("⚠️ No se encontró el panel de corazones");
            }
        }
        
        if (heartImages == null || heartImages.Length == 0)
        {
            if (heartsPanel != null)
            {
                heartImages = heartsPanel.GetComponentsInChildren<Image>();
                System.Collections.Generic.List<Image> hearts = new System.Collections.Generic.List<Image>();
                foreach (Image img in heartImages)
                {
                    if (img.gameObject.name.Contains("Heart") || img.gameObject.name.Contains("Corazon"))
                    {
                        hearts.Add(img);
                    }
                }
                if (hearts.Count > 0)
                    heartImages = hearts.ToArray();
            }
        }
        
        if (heartImages.Length != maxHealth)
        {
            Debug.LogWarning($"⚠️ Número de corazones ({heartImages.Length}) no coincide con la vida máxima ({maxHealth})");
        }
    }
    
    void UpdateHeartUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentHealth)
            {
                if (fullHeartSprite != null)
                    heartImages[i].sprite = fullHeartSprite;
                heartImages[i].color = Color.white;
            }
            else
            {
                if (emptyHeartSprite != null)
                    heartImages[i].sprite = emptyHeartSprite;
                heartImages[i].color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }
        
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        Debug.Log($"💔 Jugador recibió {damage} de daño. Vida: {currentHealth}/{maxHealth}");
        
        UpdateHeartUI();
        
        if (damageEffect != null)
            Instantiate(damageEffect, transform.position, Quaternion.identity);
        
        if (damageSound != null)
            AudioSource.PlayClipAtPoint(damageSound, transform.position, 1f);
        
        if (animator != null)
            animator.SetTrigger("Hit");
        
        OnPlayerDamaged?.Invoke();
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }
    
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        Debug.Log($"💚 Jugador curó {amount}. Vida: {currentHealth}/{maxHealth}");
        
        UpdateHeartUI();
        
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        
        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }
        
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
        
        isInvincible = false;
    }
    
    void Die()
    {
        Debug.Log("💀 El jugador ha muerto");
        
        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position, 1f);
        
        OnPlayerDied?.Invoke();
        
        // 🔥 ACTIVAR GAME OVER 🔥
        ActivarGameOver();
        
        // Desactivar el control del jugador
        PlayerMove playerMove = GetComponent<PlayerMove>();
        if (playerMove != null)
            playerMove.enabled = false;
        
        // Animation de muerte
        if (animator != null)
            animator.SetTrigger("Death");
    }
    
    void ActivarGameOver()
    {
       if (gameOverManager != null)
       {
           gameOverManager.GameOver();
           Debug.Log("✅ GameOver activado desde PlayerHealth");
       }
       else
       {
           Debug.LogError("❌ GameOverManager es NULL! No se puede mostrar Game Over");
            
           // Intentar encontrarlo como fallback
           gameOverManager = FindFirstObjectByType<GameOverManager>();
           if (gameOverManager != null)
           {
               //gameOverManager.GameOver();
               Debug.Log("✅ GameOver encontrado y activado como fallback");
           }
           else
           {
               Debug.LogError("❌ No hay GameOverManager en la escena!");
           }
       }
    }
    
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHeartUI();
        isInvincible = false;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }
    }
    
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsDead() => currentHealth <= 0;
}