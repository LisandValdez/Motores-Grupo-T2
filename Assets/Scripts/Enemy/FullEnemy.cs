using UnityEngine;
using UnityEngine.AI;

public class EnemigoCompleto : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform objetivo; // El jugador
    
    [Header("Patrullaje")]
    [SerializeField] private Transform[] puntosPatrulla;
    private int indicePatrulla = 0;
    
    [Header("Persecución")]
    [SerializeField] private float rangoDeteccion = 10f;
    [SerializeField] private float rangoPerdida = 15f;
    [SerializeField] private float velocidadPersecucion = 5f;
    [SerializeField] private float velocidadPatrulla = 2f;
    
    [Header("Ataque")]
    [SerializeField] private float rangoAtaque = 2f;
    [SerializeField] private int daño = 20;
    [SerializeField] private float cooldownAtaque = 1.5f;
    
    [Header("Estados")]
    private EstadoEnemigo estadoActual = EstadoEnemigo.Patrullando;
    private float ultimoTiempoAtaque = 0f;
    
    private enum EstadoEnemigo
    {
        Patrullando,
        Persiguiendo,
        Atacando
    }
    
    // Referencias a los otros scripts
    private EnemyHealth enemyHealth;
    private EnemyAttack enemyAttack;
    
    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponent<Animator>();
        
        // Obtener referencias de los scripts de vida y ataque
        enemyHealth = GetComponent<EnemyHealth>();
        enemyAttack = GetComponent<EnemyAttack>();
        
        if (objetivo == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) objetivo = player.transform;
        }
        
        agent.speed = velocidadPatrulla;
    }
    
    void Update()
    {
        if (objetivo == null) return;
        
        float distanciaAlJugador = Vector3.Distance(transform.position, objetivo.position);
        
        // Verificar si puede atacar
        if (distanciaAlJugador <= rangoAtaque && Time.time >= ultimoTiempoAtaque + cooldownAtaque)
        {
            CambiarEstado(EstadoEnemigo.Atacando);
            Atacar();
        }
        // Cambiar estados según la distancia
        else if (distanciaAlJugador < rangoDeteccion)
        {
            CambiarEstado(EstadoEnemigo.Persiguiendo);
            Perseguir();
        }
        else if (estadoActual == EstadoEnemigo.Persiguiendo && distanciaAlJugador > rangoPerdida)
        {
            CambiarEstado(EstadoEnemigo.Patrullando);
        }
        else if (estadoActual == EstadoEnemigo.Patrullando)
        {
            Patrullar();
        }
        else if (estadoActual == EstadoEnemigo.Persiguiendo)
        {
            Perseguir();
        }
        
        // Actualizar animación
        float velocidad = agent.velocity.magnitude;
        if (anim != null)
            anim.SetFloat("speed", velocidad);
    }
    
    void Patrullar()
    {
        if (puntosPatrulla.Length == 0) return;
        
        agent.SetDestination(puntosPatrulla[indicePatrulla].position);
        
        if (Vector3.Distance(transform.position, puntosPatrulla[indicePatrulla].position) < 1f)
        {
            indicePatrulla = (indicePatrulla + 1) % puntosPatrulla.Length;
        }
    }
    
    void Perseguir()
    {
        agent.speed = velocidadPersecucion;
        agent.SetDestination(objetivo.position);
        
        // Rotar hacia el jugador
        Vector3 direccion = (objetivo.position - transform.position).normalized;
        if (direccion != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 5f);
        }
    }
    
    void Atacar()
    {
        ultimoTiempoAtaque = Time.time;
        
        // Animación de ataque
        if (anim != null)
            anim.SetTrigger("Attack");
        
        // Aplicar daño después de un pequeño delay (para sincronizar con animación)
        Invoke(nameof(ApplyDamage), 0.3f);
        
        Debug.Log($"⚔️ Enemigo atacando a {objetivo.name}");
    }
    
    void ApplyDamage()
    {
        if (objetivo == null) return;
        
        // Verificar que el jugador sigue en rango
        float distancia = Vector3.Distance(transform.position, objetivo.position);
        if (distancia <= rangoAtaque + 0.5f)
        {
            Sist_vida playerHealth = objetivo.GetComponent<Sist_vida>();
            if (playerHealth != null)
            {
                playerHealth.Take_damage(daño);
                Debug.Log($"⚔️ Enemigo causó {daño} de daño al jugador");
            }
        }
        
        // Volver a perseguir después del ataque
        CambiarEstado(EstadoEnemigo.Persiguiendo);
    }
    
    void CambiarEstado(EstadoEnemigo nuevoEstado)
    {
        if (estadoActual == nuevoEstado) return;
        
        estadoActual = nuevoEstado;
        
        switch (nuevoEstado)
        {
            case EstadoEnemigo.Patrullando:
                agent.speed = velocidadPatrulla;
                Debug.Log("🟢 Enemigo: Modo PATRULLA");
                break;
            case EstadoEnemigo.Persiguiendo:
                agent.speed = velocidadPersecucion;
                Debug.Log("🔴 Enemigo: Modo PERSECUCIÓN");
                break;
            case EstadoEnemigo.Atacando:
                agent.speed = 0f;
                Debug.Log("⚔️ Enemigo: Modo ATACANDO");
                break;
        }
    }
    
    // Método para recibir daño (puede ser llamado desde otro script)
    public void RecibirDaño(int cantidad)
    {
        if (enemyHealth != null)
            enemyHealth.TakeDamage(cantidad);
        else
            Debug.LogWarning($"⚠️ {gameObject.name} no tiene componente EnemyHealth");
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoPerdida);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}