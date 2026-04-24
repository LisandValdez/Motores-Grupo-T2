using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeBase : WeaponBase
{
    [Header("Melee Settings")]
    public Collider hitCollider;
    protected Animator anim;
    protected bool isAttacking = false;

    // Movido a la base para que todas las armas melee lo compartan
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        if (hitCollider != null) hitCollider.enabled = false;
    }

    public override void Attack(Vector3 targetPoint)
    {
        if (Time.time >= nextAttackTime && !isAttacking)
        {
            nextAttackTime = Time.time + (1f / fireRate);
            isAttacking = true;

            hitTargets.Clear(); // Limpiamos la lista al iniciar cada ataque

            if (anim != null) anim.SetTrigger("AttackTrigger");

            OnMeleeStrike();
        }
    }

    protected abstract void OnMeleeStrike();

    // Detectamos el daño de forma genérica para todas las armas melee
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        // Buscamos la interfaz IDamageable (recomendado) o usamos el Tag
        if (other.CompareTag("Enemy") && !hitTargets.Contains(other.gameObject))
        {
            hitTargets.Add(other.gameObject);

            // Si usas la interfaz: 
            // other.GetComponent<IDamageable>()?.TakeDamage(damage);
            Debug.Log($"{weaponName} golpeó a {other.name} causando {damage} de daño.");
        }
    }

    public void EnableHitbox() => hitCollider.enabled = true;
    public void DisableHitbox() => hitCollider.enabled = false;

    public void FinishAttack()
    {
        isAttacking = false;
        if (hitCollider != null) hitCollider.enabled = false;
    }
}