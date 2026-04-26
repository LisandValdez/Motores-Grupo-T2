using UnityEngine;
using UnityEngine.AI;

public class Patrullaje : MonoBehaviour
{

    [Header("NavMesh")]
    [SerializeField] private NavMeshAgent enemy;

    [Header("Patrulla")]
    [SerializeField] private Transform[] puntos;
    private Animator anim;
    private int index = 0;
    //private int indexActual = 0; remplazaria al de arriba

    void Awake()
    {
        if (enemy == null)
            enemy = GetComponent<NavMeshAgent>();
        if (anim == null)
            anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (puntos.Length == 0) return;

        enemy.SetDestination(puntos[index].position);

        float distancia = Vector3.Distance(transform.position, puntos[index].position);

        if (distancia < 1f)
        {
            index = (index + 1) % puntos.Length;
        }

        float speed = enemy.velocity.magnitude / enemy.speed;
        anim.SetFloat("speed", speed);

        //indexActual = MoverEntrePuntos(puntos, indexActual, enemy);  al tener la funcion solo iria esto en update y lo otro se borra

    }

    //private int MoverEntrePuntos(Transform[] puntos, int index, Transform enemy, float distanciaMinima = 1f)
    //{
    //    if (puntos.Length == 0) return index; // Retorna el mismo índice si no hay puntos

    //    // Mover al enemigo hacia el punto actual
    //    enemy.SetDestination(puntos[index].position);

    //    // Calcular la distancia al punto
    //    float distancia = Vector3.Distance(enemy.position, puntos[index].position);

    //    // Si está lo suficientemente cerca, pasar al siguiente punto
    //    if (distancia < distanciaMinima)
    //    {
    //        index = (index + 1) % puntos.Length;
    //    }

    //    return index; // Retornamos el índice actualizado
    //}
}
