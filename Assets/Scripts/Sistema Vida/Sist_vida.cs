using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using System.Collections;

public class Sist_vida : MonoBehaviour, IDamageable, IHealt
{
    [SerializeField] private int maxLife;
    private int actualLife;

    public Action<int> OnHealthChanged;
    public Action OnDeath;

    private void Awake()
    {
        actualLife = maxLife;
        OnHealthChanged?.Invoke(actualLife);
    }

    // Implementación de daño
    public void TakeDamage(int damage)
    {
        actualLife = Mathf.Clamp(actualLife - damage, 0, maxLife);
        OnHealthChanged?.Invoke(actualLife);

        if (actualLife <= 0)
        {
            OnDeath?.Invoke();
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
