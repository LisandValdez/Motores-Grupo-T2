using UnityEngine;

public class Tomar_daño_enemigo : MonoBehaviour
{
    [SerializeField] private int damage;

    private void OnTriggerEnter(Collider other)
    {
        
        Dopelllganger enemy = other.GetComponent<Dopelllganger>();
        if (enemy != null)
        {
            enemy.Take_damage_enemy(); 
        }
    }
}
