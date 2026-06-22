using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    [Header("Base Stats")]
    public string weaponName;
    public float damage;
    public float fireRate;

    protected float nextAttackTime = 0f;


    public abstract void Attack(Vector3 targetPoint);


    public virtual void Aim(bool isAiming) { }
    public virtual void Reload() { }
}