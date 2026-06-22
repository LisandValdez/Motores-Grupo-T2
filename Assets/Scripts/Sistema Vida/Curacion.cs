using UnityEngine;

public class Curacion : MonoBehaviour
{
    [SerializeField] private int healtAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Botiquin tocado: " + other.name);

        if (other.TryGetComponent<IHealt>(out var healt))
        {
            healt.Take_health(healtAmount);
            Debug.Log("Curacion aplicada: " + healtAmount);

            Destroy(gameObject); 
        }
        else
        {
            Debug.Log("El objeto no tiene IHealt");
        }
    }
}
