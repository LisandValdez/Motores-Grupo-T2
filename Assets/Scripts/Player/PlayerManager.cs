using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Sist_vida vida;
    [SerializeField] private GameOverManager gameOverManager;

    private void Start()
    {
        if (vida != null && gameOverManager != null)
        {
            vida.OnDeath += gameOverManager.GameOver;
        }
    }
}
