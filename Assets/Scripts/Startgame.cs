using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    public void EmpezarJuego()
    {
        Invoke("CargarJuego", 1f);
    }

    void CargarJuego()
    {
        SceneManager.LoadScene("GameScene");
    }
}