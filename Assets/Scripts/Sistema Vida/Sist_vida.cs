using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using System.Collections;

// Esto obliga al GameObject a tener un AudioSource, evitando olvidos en el editor
[RequireComponent(typeof(AudioSource))]
public class Sist_vida : MonoBehaviour, IDamageable, IHealt
{
    [Header("Vida")]
    [SerializeField] private int maxLife = 3;
    private int actualLife;

    public Action<int> OnHealthChanged;
    public Action OnDeath;

    [Header("Audio")]
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deathSound;

    // Referencia interna al componente
    private AudioSource audioSource;

    private void Awake()
    {
        actualLife = maxLife;
        OnHealthChanged?.Invoke(actualLife);

        // Obtenemos el componente AudioSource del propio objeto
        audioSource = GetComponent<AudioSource>();
    }

    // Implementación de daño
    public void TakeDamage(int damage)
    {
        actualLife = Mathf.Clamp(actualLife - damage, 0, maxLife);
        OnHealthChanged?.Invoke(actualLife);

        if (actualLife <= 0)
        {
            OnDeath?.Invoke();

            // 💀 REPRODUCIR SONIDO DE MUERTE
            // Usamos PlayOneShot para que el audio se reproduzca encima de cualquier otro sin cortarlo
            if (audioSource != null && deathSound != null)
                audioSource.PlayOneShot(deathSound);
        }
        else
        {
            // 🩸 REPRODUCIR SONIDO DE DAÑO
            // Solo suena si no ha muerto para que no se pisen drásticamente
            if (audioSource != null && damageSound != null)
                audioSource.PlayOneShot(damageSound);
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