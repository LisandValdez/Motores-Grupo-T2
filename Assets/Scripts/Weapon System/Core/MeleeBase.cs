using UnityEngine;

public abstract class MeleeBase : WeaponBase
{
    [Header("Melee Settings")]
    public float attackRange = 1.5f;
    protected bool isBlocking;

    // En melee, "apuntar" suele traducirse en bloquear o preparar un golpe fuerte
    public override void Aim(bool isAimingState)
    {
        isBlocking = isAimingState;
        Debug.Log(isBlocking ? $"{weaponName} en posición de bloqueo" : "Guardia baja");
    }

    // NO sobrescribimos Reload(). Un arma cuerpo a cuerpo simplemente usará 
    // el método vacío de WeaponBase, ahorrándonos código basura.
}