using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Management")]
    public WeaponBase[] arsenal;
    private int currentWeaponIndex = 0;
    private WeaponBase currentWeapon;
    private bool isAttackPressed = false;
    private bool isSwitching = false;

    [Header("Camera & Crosshair")]
    public Camera mainCamera;
    public LayerMask ignorePlayerLayer;

    [Header("Movement Integration")]
    public PlayerMove movementScript;
    public PlayerLook lookScript;

    private void Start()
    {
        foreach (var weapon in arsenal)
        {
            weapon.gameObject.SetActive(false);
        }
        EquipWeaponInstant(0);
    }

    private void Update()
    {
        // Bloqueamos el bucle de ráfaga automática si el juego está pausado
        if (Time.timeScale == 0f) return;

        if (isAttackPressed && currentWeapon != null && !isSwitching)
        {
            currentWeapon.Attack(GetCrosshairTargetPoint());
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        // SI EL JUEGO ESTÁ PAUSADO, IGNORAMOS POR COMPLETO EL INPUT
        if (Time.timeScale == 0f)
        {
            isAttackPressed = false; // Nos aseguramos de limpiar el estado
            return;
        }

        if (context.performed) isAttackPressed = true;
        else if (context.canceled) isAttackPressed = false;

        if (context.started && currentWeapon != null && !isSwitching)
        {
            currentWeapon.Attack(GetCrosshairTargetPoint());
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        // SI EL JUEGO ESTÁ PAUSADO, IGNORAMOS EL APUNTADO
        if (Time.timeScale == 0f) return;

        if (currentWeapon != null && !isSwitching && !(currentWeapon is MeleeBase))
        {
            bool aiming = context.ReadValueAsButton();
            currentWeapon.Aim(aiming);

            if (movementScript != null) movementScript.SetAiming(aiming);
            if (lookScript != null) lookScript.SetAiming(aiming);
        }
        else if (currentWeapon is MeleeBase)
        {
            if (movementScript != null) movementScript.SetAiming(false);
            if (lookScript != null) lookScript.SetAiming(false);
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        // SI EL JUEGO ESTÁ PAUSADO, NO RECARGAMOS
        if (Time.timeScale == 0f) return;

        if (context.started && currentWeapon != null && !isSwitching)
        {
            currentWeapon.Reload();
        }
    }

    public void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        // SI EL JUEGO ESTÁ PAUSADO, NO CAMBIAMOS DE ARMA
        if (Time.timeScale == 0f) return;

        if (context.started && !isSwitching && arsenal.Length > 1)
        {
            int nextIndex = (currentWeaponIndex + 1) % arsenal.Length;
            StartCoroutine(SwitchWeaponRoutine(nextIndex));
        }
    }

    private IEnumerator SwitchWeaponRoutine(int newIndex)
    {
        isSwitching = true;

        if (currentWeapon != null)
        {
            currentWeapon.Aim(false);
            if (movementScript != null) movementScript.SetAiming(false);
            if (lookScript != null) lookScript.SetAiming(false);

            Animator anim = currentWeapon.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("SwitchTrigger");
            }

            yield return new WaitForSeconds(0.5f);
            currentWeapon.gameObject.SetActive(false);
        }

        currentWeaponIndex = newIndex;
        currentWeapon = arsenal[currentWeaponIndex];
        currentWeapon.gameObject.SetActive(true);

        Debug.Log($"Arma equipada: {currentWeapon.weaponName}");

        FindFirstObjectByType<WeaponAmmoUI>()?.SetupWeaponUI(currentWeapon as FireWeaponBase);

        isSwitching = false;
    }

    private void EquipWeaponInstant(int index)
    {
        currentWeaponIndex = index;
        currentWeapon = arsenal[currentWeaponIndex];
        currentWeapon.gameObject.SetActive(true);
        currentWeapon.Aim(false);

        if (movementScript != null) movementScript.SetAiming(false);
        if (lookScript != null) lookScript.SetAiming(false);

        FindFirstObjectByType<WeaponAmmoUI>()?.SetupWeaponUI(currentWeapon as FireWeaponBase);
    }

    private Vector3 GetCrosshairTargetPoint()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~ignorePlayerLayer))
        {
            return hit.point;
        }
        return ray.GetPoint(100f);
    }
}