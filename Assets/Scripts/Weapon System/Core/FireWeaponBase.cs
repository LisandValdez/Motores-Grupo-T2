using UnityEngine;
using System.Collections; // Necesario para usar Corrutinas

public abstract class FireWeaponBase : WeaponBase
{
    [Header("FireWeapon Settings")]
    public GameObject projectilePrefab; // Prefab que hereda de ProjectileBase
    public Transform firePoint;
    public int maxAmmo = 10;
    public float reloadTime = 2f;

    [Header("Audio")]
    public AudioClip shootSound;        // Sonido de disparo
    public AudioClip dryFireSound;      // Sonido cuando no hay balas (click)
    public AudioClip reloadSound;       // Sonido de recarga
    public float shootVolume = 1f;      // Volumen del disparo
    public float dryFireVolume = 0.8f;  // Volumen del click
    
    [Header("Audio Source (Opcional)")]
    public AudioSource audioSource;     // Si no se asigna, se crea automáticamente

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
        
        // Configurar AudioSource si no está asignado
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // Configurar AudioSource
        audioSource.spatialBlend = 1f; // Sonido 3D
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = 50f;
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
            PlayShootSound(); // 🔫 Reproducir sonido de disparo

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
            PlayDryFireSound(); // 🔇 Sonido de dry fire (sin balas)
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

        PlayReloadSound(); // 🔄 Sonido de inicio de recarga

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
    
    // 🔫 MÉTODOS DE SONIDO
    
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