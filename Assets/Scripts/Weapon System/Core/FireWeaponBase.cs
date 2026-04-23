using UnityEngine;

using System.Collections; // Necesario para usar Corrutinas

public abstract class FireWeaponBase : WeaponBase
{
    [Header("FireWeapon Settings")]
    public int maxAmmo = 10;
    public float reloadTime = 2f; 
    public Transform firePoint;

    [Header("Animations")]
    protected Animator anim;

    protected int currentAmmo;
    protected bool isAiming;
    protected bool isReloading = false; 

    [Header("Casing Settings")]
    public GameObject casingPrefab;
    public Transform casingExitPoint;
    public float casingEjectForce = 5f;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
    }
    protected virtual void Start()
    {
        currentAmmo = maxAmmo;
        
    }

    public override void Aim(bool isAimingState)
    {
        // Si estamos recargando, podrías querer cancelar el apuntado
        

        isAiming = isAimingState;

        if (anim != null)
        {
            anim.SetBool("isAimingAnim", isAiming);
        }
    }

    
    public override void Reload()
    {
        // Solo recarga si no está recargando ya y le falta munición
        if (!isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log($"Recargando {weaponName}...");

        // Verificamos si el Animator tiene el parámetro antes de llamarlo
        if (anim != null && HasParameter("ReloadTrigger", anim))
        {
            anim.SetTrigger("ReloadTrigger");
        }

        // Espera el tiempo definido en el inspector
        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log($"{weaponName} lista.");
        
    }

    // Función auxiliar para evitar errores de "Parameter does not exist"
    private bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }

    // Actualizamos CanShoot para incluir el bloqueo de recarga
    protected bool CanShoot()
    {
        return Time.time >= nextAttackTime && currentAmmo > 0 && !isReloading;
    }

    protected void PlayFireAnimation()
    {
        if (anim != null)
        {
            anim.SetTrigger("FireTrigger");
        }
    }

    public void PlaySwitchAnimation()
    {
        if (anim != null)
        {
            anim.SetTrigger("SwitchTrigger");
        }
    }

    public void CasingRelease()
    {
        if (casingPrefab != null && casingExitPoint != null)
        {
            GameObject casing = Instantiate(casingPrefab, casingExitPoint.position, casingExitPoint.rotation);
            Rigidbody rb = casing.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 forceDirection = casingExitPoint.right + (casingExitPoint.up * 0.5f);
                rb.AddForce(forceDirection * casingEjectForce, ForceMode.Impulse);
                rb.AddTorque(new Vector3(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500)));
            }
            Destroy(casing, 1f);
        }
    }

    public virtual void Shoot() { }
}