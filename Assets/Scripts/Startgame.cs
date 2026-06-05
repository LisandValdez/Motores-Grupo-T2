using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public AudioSource glitchSound;

    public void EmpezarJuego()
    {
        glitchSound.Play();
        Invoke("CargarJuego", 1f);
    }

    void CargarJuego()
    {
        SceneManager.LoadScene("MapaHotel");
    }
}