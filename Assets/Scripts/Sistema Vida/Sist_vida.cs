using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using System.Collections;


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

   
    private AudioSource audioSource;

    private void Awake()
    {
        actualLife = maxLife;
        OnHealthChanged?.Invoke(actualLife);

        
        audioSource = GetComponent<AudioSource>();
    }

    
    public void TakeDamage(int damage)
    {
        actualLife = Mathf.Clamp(actualLife - damage, 0, maxLife);
        OnHealthChanged?.Invoke(actualLife);

        if (actualLife <= 0)
        {
            OnDeath?.Invoke();

            
            if (audioSource != null && deathSound != null)
                audioSource.PlayOneShot(deathSound);
        }
        else
        {
            
            if (audioSource != null && damageSound != null)
                audioSource.PlayOneShot(damageSound);
        }
    }

    public void Take_health(int healthAmount)
    {
        actualLife = Mathf.Clamp(actualLife + healthAmount, 0, maxLife);
        OnHealthChanged?.Invoke(actualLife);
    }

    public int GetMaxLife() => maxLife;
    public int GetActualLife() => actualLife;
    public bool IsDead() => actualLife <= 0;
}