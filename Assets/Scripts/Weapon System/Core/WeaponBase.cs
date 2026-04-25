using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    [Header("Base Stats")]
    public string weaponName;
    public float damage;
    public float fireRate;

    protected float nextAttackTime = 0f;

    // Attack sigue siendo abstracto porque TODA arma debe definir CÓMO ataca.
    public abstract void Attack(Vector3 targetPoint);

    // Aim y Reload son virtuales. Si un arma no los sobrescribe, no harán nada.
    public virtual void Aim(bool isAiming) { }
    public virtual void Reload() { }
}