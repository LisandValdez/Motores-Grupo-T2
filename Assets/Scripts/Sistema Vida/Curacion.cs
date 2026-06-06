using UnityEngine;

public class Curacion : MonoBehaviour
{
    [SerializeField] private int healtAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Botiquín tocó: " + other.name);

        if (other.TryGetComponent<IHealt>(out var healt))
        {
            healt.Take_health(healtAmount);
            Debug.Log("Curación aplicada: " + healtAmount);

            Destroy(gameObject); 
        }
        else
        {
            Debug.Log("El objeto no tiene IHealt");
        }
    }
}
