using UnityEngine;
using System.Collections; // Necesario para usar Corrutinas

public abstract class FireWeaponBase : WeaponBase
{
    [Header("FireWeapon Settings")]
    public GameObject projectilePrefab; // Prefab que hereda de ProjectileBase
    public Transform firePoint;
    public int maxAmmo = 10;
    public float reloadTime = 2f;

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

    public override void Attack(Vector3 targetPoint)
    {
        // Lógica de disparo centralizada para todas las armas de fuego
        if (CanShoot())
        {
            nextAttackTime = Time.time + (1f / fireRate);
            currentAmmo--;

            PlayFireAnimation();

            // Cálculo de dirección e instanciación del proyectil
            Vector3 fireDirection = (targetPoint - firePoint.position).normalized;
            GameObject projObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(fireDirection));

            // Inicialización del proyectil con el daño del arma
            ProjectileBase proj = projObj.GetComponent<ProjectileBase>();
            if (proj != null)
            {
                proj.Initialize(damage);
            }

            Debug.Log($"{weaponName} disparó. Balas restantes: {currentAmmo}");
        }
        else if (currentAmmo <= 0 && !isReloading)
        {
            Debug.Log("¡Click click! (Sin balas)");
        }
    }

    public override void Aim(bool isAimingState)
    {
        isAiming = isAimingState;

        if (anim != null)
        {
            anim.SetBool("isAimingAnim", isAiming);
        }
    }

    public override void Reload()
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log($"Recargando {weaponName}...");

        if (anim != null && HasParameter("ReloadTrigger", anim))
        {
            anim.SetTrigger("ReloadTrigger");
        }

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log($"{weaponName} lista.");
    }

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

    private bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }
}