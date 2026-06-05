using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [Header("Ataque")]
    [SerializeField] private int damage = 1;  // Cambiado a 1 para que quite un corazón
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackDelay = 0.3f;
    
    [Header("Componentes")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Animator anim;
    
    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    private GameObject currentTarget;
    
    void Start()
    {
        if (attackPoint == null)
            attackPoint = transform;
        
        if (playerLayer == 0)
            playerLayer = LayerMask.GetMask("Player");
        
        if (anim == null)
            anim = GetComponent<Animator>();
    }
    
    void Update()
    {
        if (currentTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) currentTarget = player;
        }
        
        if (currentTarget == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, currentTarget.transform.position);
        
        if (distanceToPlayer <= attackRange && !isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(PerformAttack());
        }
    }
    
    IEnumerator PerformAttack()
    {
        isAttacking = true;
        
        if (anim != null)
            anim.SetTrigger("Attack");
        
        yield return new WaitForSeconds(attackDelay);
        
        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (distance <= attackRange)
        {
            ApplyDamage();
        }
        
        lastAttackTime = Time.time;
        isAttacking = false;
    }
    
    void ApplyDamage()
    {
        if (currentTarget == null) return;
        
        // 🔥 USAR EL NUEVO SISTEMA PlayerHealth
        PlayerHealth playerHealth = currentTarget.GetComponent<PlayerHealth>();
        
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log($"⚔️ Enemigo atacó al jugador causando {damage} de daño");
        }
        else
        {
            // Fallback: si no tiene PlayerHealth, buscar Sist_vida original
            Sist_vida oldHealth = currentTarget.GetComponent<Sist_vida>();
            if (oldHealth != null)
                oldHealth.Take_damage(damage);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}