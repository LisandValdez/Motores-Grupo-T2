using UnityEngine;

// La interfaz define los "contratos" que toda arma debe cumplir
public interface IWeapon
{
    void Attack(Vector3 targetPoint); // Recibe hacia dónde apuntar (crosshair)
    void Aim(bool isAiming);
    void Reload();
}