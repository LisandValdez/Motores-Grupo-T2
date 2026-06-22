using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeBase : WeaponBase
{
    [Header("Melee Settings")]
    public Collider hitCollider;
    protected Animator anim;
    protected bool isAttacking = false;

    
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

            hitTargets.Clear();

            if (anim != null) anim.SetTrigger("AttackTrigger");

            OnMeleeStrike();
        }
    }

    protected abstract void OnMeleeStrike();

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        if (other.CompareTag("Enemy") && !hitTargets.Contains(other.gameObject))
        {
            hitTargets.Add(other.gameObject);


            Debug.Log($"{weaponName} golpe� a {other.name} causando {damage} de da�o.");
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