using UnityEngine;
using System.Collections;

public abstract class FireWeaponBase : WeaponBase
{
    [Header("FireWeapon Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int maxAmmo = 10;
    public float reloadTime = 2f;

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip dryFireSound;
    public AudioClip reloadSound;
    public float shootVolume = 1f;
    public float dryFireVolume = 0.8f;

    [Header("Audio Source (Opcional)")]
    public AudioSource audioSource;

    [Header("Animations")]
    protected Animator anim;

    protected int currentAmmo;
    protected bool isAiming;
    protected bool isReloading = false;

    [Header("Casing Settings")]
    public GameObject casingPrefab;
    public Transform casingExitPoint;
    public float casingEjectForce = 5f;

    [Header("Ajustes de Munición del Inventario")]
    public string ammoTypeName = "Pistol Ammo"; // Debe coincidir con el 'itemName' de tu ItemPickup

    // Evento para avisarle a la UI que disparamos o recargamos y que debe actualizarse
    public System.Action OnWeaponAmmoChanged;

    // Métodos públicos para que la UI pueda consultar los datos fácilmente
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }



        // SOLUCIÓN AL 00/00: Establecemos las balas aquí para que estén listas antes de que el Start de la UI pregunte por ellas
        currentAmmo = maxAmmo;
    }

    protected virtual void Start()
    {
        // Dejado vacío por si tus armas hijas (Pistol) lo extienden
    }

    public override void Attack(Vector3 targetPoint)
    {
        if (CanShoot())
        {
            nextAttackTime = Time.time + (1f / fireRate);
            currentAmmo--;

            // Invocar el evento para actualizar la UI del juego al disparar
            OnWeaponAmmoChanged?.Invoke();

            PlayFireAnimation();
            PlayShootSound();

            Vector3 fireDirection = (targetPoint - firePoint.position).normalized;
            GameObject projObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(fireDirection));

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
            PlayDryFireSound();
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
        if (isReloading || currentAmmo >= maxAmmo) return;

        // Verificar si el inventario tiene balas de este tipo
        int ammoInInventory = Inventory.Instance != null ? Inventory.Instance.GetAmmo(ammoTypeName) : 0;

        if (ammoInInventory > 0)
        {
            StartCoroutine(ReloadRoutine());
        }
        else
        {
            Debug.LogWarning($"❌ No hay munición de tipo '{ammoTypeName}' en el inventario.");
            PlayDryFireSound();
        }
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log($"Recargando {weaponName}...");

        PlayReloadSound();

        if (anim != null && HasParameter("ReloadTrigger", anim))
        {
            anim.SetTrigger("ReloadTrigger");
        }

        yield return new WaitForSeconds(reloadTime);

        // Lógica de transferencia desde el Inventario
        if (Inventory.Instance != null)
        {
            int ammoNeeded = maxAmmo - currentAmmo;
            int ammoInInventory = Inventory.Instance.GetAmmo(ammoTypeName);

            int ammoToLoad = Mathf.Min(ammoNeeded, ammoInInventory);

            if (Inventory.Instance.UseAmmo(ammoTypeName, ammoToLoad))
            {
                currentAmmo += ammoToLoad;
                Debug.Log($"🔄 {weaponName} recargada. Cargador: {currentAmmo}/{maxAmmo}");
            }
        }

        isReloading = false;

        // Invocar el evento para actualizar la UI al terminar de recargar
        OnWeaponAmmoChanged?.Invoke();
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

    protected void PlayShootSound()
    {
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound, shootVolume);
        }
    }

    protected void PlayDryFireSound()
    {
        if (dryFireSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(dryFireSound, dryFireVolume);
        }
    }

    protected void PlayReloadSound()
    {
        if (reloadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(reloadSound, 0.8f);
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