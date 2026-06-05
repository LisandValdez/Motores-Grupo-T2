using UnityEngine;

public class Tomar_Daño : MonoBehaviour
{
    [SerializeField] private int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Sist_vida life_Player))
        {
            life_Player.Take_damage(damage);
        }
    }
}
