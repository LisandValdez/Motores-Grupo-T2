using UnityEngine;

public class Curacion : MonoBehaviour
{
    [SerializeField] private int healthAmount;

    private void OnTriggerEnter(Collider other)
    {
  
        if (other.TryGetComponent<Sist_vida>(out Sist_vida vida))
        {
            vida.Take_health(healthAmount);
            Destroy(gameObject); 
        }
    }
}
