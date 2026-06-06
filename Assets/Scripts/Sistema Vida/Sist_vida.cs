using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using System.Collections;

public class Sist_vida : MonoBehaviour, IDamageable, IHealt
{
    [Header("Vida")]
    [SerializeField] private int maxLife = 3;
    private int actualLife;

    public Action<int> OnHealthChanged;
    public Action OnDeath;

    [Header("Audio (simple, sin componentes extra)")]
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField][Range(0f, 1f)] private float damageVolume = 1f;
    [SerializeField][Range(0f, 1f)] private float deathVolume = 1f;

    private void Awake()
    {
        actualLife = maxLife;
        OnHealthChanged?.Invoke(actualLife);
    }

    // Implementación de daño
    public void TakeDamage(int damage)
    {
        // Reproducir sonido de daño (fallback posicional)
        if (damageSound != null)
            AudioSource.PlayClipAtPoint(damageSound, transform.position, damageVolume);

        actualLife = Mathf.Clamp(actualLife - damage, 0, maxLife);
        OnHealthChanged?.Invoke(actualLife);

        if (actualLife <= 0)
        {
            OnDeath?.Invoke();

            if (deathSound != null)
                AudioSource.PlayClipAtPoint(deathSound, transform.position, deathVolume);
        }
    }

    // Implementación de curación
    public void Take_health(int healthAmount)
    {
        actualLife = Mathf.Clamp(actualLife + healthAmount, 0, maxLife);
        OnHealthChanged?.Invoke(actualLife);
    }

    // Getters
    public int GetMaxLife() => maxLife;
    public int GetActualLife() => actualLife;
    public bool IsDead() => actualLife <= 0;
}
