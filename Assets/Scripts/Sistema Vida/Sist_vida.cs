using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using System.Collections;

public class Sist_vida : MonoBehaviour
{
    //este codigo el recibe daño y actualiza la vida, animaciones y muerte.
    [SerializeField] private GameObject player;
    public Action<int> player_take_damage;
    public Action<int> player_take_health;

    [SerializeField] private int maxlife;

    [SerializeField] private int actual_life;
    private Animator anim;
    private Rigidbody rb;


    private void Awake()
    {
        actual_life = maxlife;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        PlayerMove ps = player.GetComponent<PlayerMove>();
        if (ps != null)
            ps.SetMaxLife(maxlife);

    }

    public void TakeDamage(int damage)
    {
        actual_life = math.clamp(actual_life - damage, 0, maxlife);
        player_take_damage?.Invoke(actual_life);

        if (actual_life > 0)
        {
            Debug.Log("Recibiste daño te queda: " + actual_life);
            if (anim != null)
            {
                //anim.SetTrigger("t_damage");
                Debug.Log("Recibiste daño pero con animacion" + actual_life);
            }
        }
        else
        { 
             StartCoroutine(DeathSequence());
        }

    }
    private IEnumerator DeathSequence()
    {
        PlayerMove ps = player.GetComponent<PlayerMove>();

        Debug.Log("El jugador ha muerto");

        if (ps != null)
            ps.die();

        if (anim != null)
        {

            ps.die();
            //anim.SetTrigger("isdead"); 
            Debug.Log("El jugador ha muerto con animacion");

            yield return new WaitForSeconds(1f);

        }

        Destroy_player();

        //SceneManager.LoadScene("defeat");
    }

    public void Take_health(int health)
    {
        actual_life = math.clamp(actual_life + health, 0, maxlife);
        player_take_health?.Invoke(actual_life);
    }

    public void Destroy_player()
    {
        Destroy(gameObject, 1f);
    }

    public int get_maxlife() => maxlife;


    public int get_actual_life() => actual_life;
}
