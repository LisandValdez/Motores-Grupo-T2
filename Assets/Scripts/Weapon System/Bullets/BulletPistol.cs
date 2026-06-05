using UnityEngine;

public class BulletPistol : MonoBehaviour
{
    public float speed = 50f;
    public float lifeTime = 3f;

    void Start()
    {
        // Destruir la bala después de un tiempo para no saturar la memoria
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Como la rotación de la bala ya mira hacia el targetPoint al ser instanciada,
        // simplemente la movemos hacia adelante.
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Lógica de daño aquí...
        Debug.Log($"Bala impactó contra: {other.name}");
        Destroy(gameObject);
    }
}