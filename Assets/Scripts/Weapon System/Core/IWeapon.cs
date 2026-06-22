using UnityEngine;


public interface IWeapon
{
    void Attack(Vector3 targetPoint);
    void Aim(bool isAiming);
    void Reload();
}