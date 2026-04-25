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
    }
}
