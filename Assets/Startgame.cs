using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void EmpezarJuego()
    {
        SceneManager.LoadScene("MapaHotel");
    }
}