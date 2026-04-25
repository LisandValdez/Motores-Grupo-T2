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
    [SerializeField] private string Z_Idle;
    [SerializeField] private string Z_Walk_InPlace;

    private void Update()
    {
        distancia_del_obj = Vector3.Distance(enemy.transform.position, objetivo_a_perseguir.position);

        if (distancia_del_obj < rango_deteccion)
        {
            persiguiendo = true;
            anim.Play("Z_Idle");
        }
        else if (distancia_del_obj > rango_deteccion + distanci_no_seguir)
        {

            persiguiendo = false;
        }


        if (persiguiendo == false)
        {
            enemy.speed = 0f;
        }
        else if (persiguiendo == true)
        {
            enemy.speed = velocity;
            anim.Play("Z_Walk_InPlace");
            enemy.SetDestination(objetivo_a_perseguir.position);
        }
    }

    private void OnDrawGizmos()

    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(enemy.transform.position, rango_deteccion);

    }
}