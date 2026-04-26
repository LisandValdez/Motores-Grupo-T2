using UnityEngine;
using UnityEngine.AI;

public class Perseguir : MonoBehaviour
{

    [SerializeField] private NavMeshAgent enemy;
    [SerializeField] private Transform objetivo_a_perseguir;
    [SerializeField] float velocity;
    [SerializeField] float rango_deteccion;
    [SerializeField] float distancia_del_obj;
    [SerializeField] float distanci_no_seguir;
    private bool persiguiendo;
  

    [Header("Animaciones")]
    [SerializeField] private Animator anim;
    public float RangoDeteccion => rango_deteccion;


    private void Update()
    {
        Met_perseguir();

        if (!persiguiendo)
        {
            enemy.speed = 0f;
            if (anim != null)
                anim.SetFloat("speed", 0f);

        }
        else if (persiguiendo == true)
        {
            enemy.speed = velocity;
            enemy.SetDestination(objetivo_a_perseguir.position);
            float velocidadActual = enemy.velocity.magnitude;
            anim.SetFloat("speed", velocidadActual);

        }
    }

    private void Met_perseguir()
    {
        distancia_del_obj = Vector3.Distance(enemy.transform.position, objetivo_a_perseguir.position);

        if (distancia_del_obj < rango_deteccion)
        {
            persiguiendo = true;

        }
        else if (distancia_del_obj > rango_deteccion + distanci_no_seguir)
        {

            persiguiendo = false;

        }
    }

    private void OnDrawGizmos()

    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(enemy.transform.position, rango_deteccion);

    }
}
