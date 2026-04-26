using UnityEngine;
using UnityEngine.UI;

public class CorduraManager : MonoBehaviour
{
    public Slider corduraBar;

    public float maxCordura = 100f;
    public float currentCordura;

    void Start()
    {
        currentCordura = maxCordura;
        corduraBar.maxValue = maxCordura;
        corduraBar.value = currentCordura;
    }

    void Update()
    {
        // Tecla J para probar pérdida de cordura
        if (Input.GetKeyDown(KeyCode.J))
        {
            PerderCordura(10f);
        }
    }

    public void PerderCordura(float cantidad)
    {
        currentCordura -= cantidad;
        currentCordura = Mathf.Clamp(currentCordura, 0, maxCordura);

        corduraBar.value = currentCordura;

        if (currentCordura <= 0)
        {
            FindFirstObjectByType<GameOverManager>().GameOver();
        }
    }
}