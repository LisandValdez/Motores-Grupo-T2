using UnityEngine;

public class Pistol : FireWeaponBase
{
    public GameObject bulletPrefab;

    public override void Attack(Vector3 targetPoint)
    {
        if (CanShoot())
        {
            nextAttackTime = Time.time + (1f / fireRate);
            currentAmmo--;

            PlayFireAnimation();

            Vector3 fireDirection = (targetPoint - firePoint.position).normalized;
            Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(fireDirection));

            Debug.Log($"Pistola disparó. Balas restantes: {currentAmmo}");
        }
        else if (currentAmmo <= 0)
        {
            Debug.Log("¡Click click! (Sin balas)");
        }
    }
    public override void Shoot()
    {
        //Necesario por una animación no implementada todavia
    }
}


