using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Dopelllganger : Enemy
{

    [SerializeField] private Patrullaje patrol;
    [SerializeField] private Perseguir chase;
    [SerializeField] private Transform player;
    [SerializeField] private Animator anim;

    [SerializeField] private float distanciaAtaque; // es el valor de stopping distance en navmeshagent
    [SerializeField] private float cooldownAtaque;
    private float ultimoAtaque;

    void Start()
    {
        ActivarPatrullaje();
        ultimoAtaque = -cooldownAtaque;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) < chase.RangoDeteccion)
        {
            GetComponent<Dopelllganger>().ActivarPersecucion();
        }
        else
        {
            GetComponent<Dopelllganger>().ActivarPatrullaje();
        }

        // Intento de ataque
        if (Vector3.Distance(transform.position, player.position) <= distanciaAtaque)
        {
            if (Time.time - ultimoAtaque >= cooldownAtaque)
            {
                GetComponent<Dopelllganger>().Atk_enemy();
                ultimoAtaque = Time.time;
            }
        }


    }
    public override void Atk_enemy()
    {
        if (player == null) return;

        if (anim != null)
            anim.SetTrigger("atk");

        Sist_vida sv = player.GetComponent<Sist_vida>();
        if (sv != null)
        {
            sv.TakeDamage(damage);
            Debug.Log("Dopelllganger atacó al jugador por " + damage + " puntos");
        }

    }

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override void Spawn_enemy()
    {
        base.Spawn_enemy();
    }

    public override void Take_damage_enemy()
    {
        life_enemy -= damage; 
        Debug.Log("Enemigo recibió " + damage + " puntos de daño. Vida actual: " + life_enemy);

        // Revisar muerte
        if (life_enemy <= 0)
        {
            die();
        }
    }

    public void die()
    {
        Debug.Log("Enemigo muerto");

        
        if (patrol != null) patrol.enabled = false;
        if (chase != null) chase.enabled = false;

        
        if (anim != null) anim.SetTrigger("death");

        
        Destroy(gameObject, 1f);
    }

    public override string ToString()
    {
        return base.ToString();
    }


    public void ActivarPatrullaje()
    {
        patrol.enabled = true;
        chase.enabled = false;
    }

    public void ActivarPersecucion()
    {
        patrol.enabled = false;
        chase.enabled = true;
    }

}
