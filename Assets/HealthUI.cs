using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Image[] hearts;

    public Sprite fullHeart;
    public Sprite emptyHeart;

    public int currentHealth = 3;
    public int maxHealth = 3;

    void Start()
    {
        UpdateHearts();
    }

    void Update()
    {
        // tecla H para probar daño
        if (Input.GetKeyDown(KeyCode.H))
        {
            currentHealth--;

            if (currentHealth < 0)
                currentHealth = 0;

            UpdateHearts();
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }

        if (currentHealth <= 0)
        {
            FindFirstObjectByType<GameOverManager>().GameOver();
        }
    }
}