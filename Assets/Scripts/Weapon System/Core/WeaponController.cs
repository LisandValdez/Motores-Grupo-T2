using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // Necesario para la corrutina de cambio

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Management")]
    public WeaponBase[] arsenal;
    private int currentWeaponIndex = 0;
    private WeaponBase currentWeapon;
    private bool isAttackPressed = false;
    private bool isSwitching = false; // Bloqueo para evitar spam de cambio

    [Header("Camera & Crosshair")]
    public Camera mainCamera;
    public LayerMask ignorePlayerLayer;

    [Header("Movement Integration")]
    public PlayerMove movementScript;
    public PlayerLook lookScript;

    private void Start()
    {
        // Inicializar: desactivar todas y equipar la primera
        foreach (var weapon in arsenal)
        {
            weapon.gameObject.SetActive(false);
        }
        EquipWeaponInstant(0);
    }

    private void Update()
    {
        // Solo permite atacar si hay un arma y no estamos en medio de un cambio
        if (isAttackPressed && currentWeapon != null && !isSwitching)
        {
            currentWeapon.Attack(GetCrosshairTargetPoint());
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed) isAttackPressed = true;
        else if (context.canceled) isAttackPressed = false;

        if (context.started && currentWeapon != null && !isSwitching)
        {
            currentWeapon.Attack(GetCrosshairTargetPoint());
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (currentWeapon != null && !isSwitching)
        {
            bool aiming = context.ReadValueAsButton();

            currentWeapon.Aim(aiming);

            if (movementScript != null) movementScript.SetAiming(aiming);
            if (lookScript != null) lookScript.SetAiming(aiming);
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.started && currentWeapon != null && !isSwitching)
        {
            currentWeapon.Reload();
        }
    }

    public void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        if (context.started && !isSwitching && arsenal.Length > 1)
        {
            int nextIndex = (currentWeaponIndex + 1) % arsenal.Length;
            StartCoroutine(SwitchWeaponRoutine(nextIndex));
        }
    }

    // Corrutina para manejar la transición visual de salida y entrada
    private IEnumerator SwitchWeaponRoutine(int newIndex)
    {
        isSwitching = true;

        // 1. Cancelar estados actuales (dejar de apuntar/correr con el arma)
        if (currentWeapon != null)
        {
            currentWeapon.Aim(false);
            if (movementScript != null) movementScript.SetAiming(false);
            if (lookScript != null) lookScript.SetAiming(false);

            // 2. Ejecutar animación de "Bajar Arma" (GunChangeDOWN)
            // Debes tener el Trigger "SwitchTrigger" configurado en tu Animator
            Animator anim = currentWeapon.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("SwitchTrigger");
            }

            // 3. Esperar a que la animación de guardado termine (ajusta este tiempo al de tu clip)
            yield return new WaitForSeconds(0.5f);
            currentWeapon.gameObject.SetActive(false);
        }

        // 4. Activar la nueva arma
        currentWeaponIndex = newIndex;
        currentWeapon = arsenal[currentWeaponIndex];
        currentWeapon.gameObject.SetActive(true);

        // Al activarse, el Animator de la nueva arma entrará por GunChangeUP automáticamente
        Debug.Log($"Arma equipada: {currentWeapon.weaponName}");

        isSwitching = false;
    }

    // Método para la configuración inicial sin esperas
    private void EquipWeaponInstant(int index)
    {
        currentWeaponIndex = index;
        currentWeapon = arsenal[currentWeaponIndex];
        currentWeapon.gameObject.SetActive(true);
        currentWeapon.Aim(false);

        if (movementScript != null) movementScript.SetAiming(false);
        if (lookScript != null) lookScript.SetAiming(false);
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