using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int life_enemy;
    
    [SerializeField] protected int damage;

    public virtual void Atk_enemy()
    {
        Debug.Log("enemigo ataca");
    }

    public virtual void Take_damage_enemy()
    {
        Debug.Log("dañaste al enemigo");
    }

    public virtual void Spawn_enemy()
    {
        Debug.Log("spawneo enemigo");
    }
}
