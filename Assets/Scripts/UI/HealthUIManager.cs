using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class HealthUIManager : MonoBehaviour
{
    [Header("UI de Corazones")]
    [SerializeField] private GameObject heartsPanel;
    [SerializeField] private Image[] heartImages;
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    [Header("Sistema de Vida")]
    [SerializeField] private Sist_vida healthSystem;

    private void Start()
    {
        InitializeHeartUI();

        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged += UpdateHeartUI;
            UpdateHeartUI(healthSystem.GetActualLife()); // Inicializar al empezar
        }
    }

    void InitializeHeartUI()
    {
        if (heartsPanel == null)
        {
            heartsPanel = GameObject.Find("HeartsPanel");
            if (heartsPanel == null)
            {
                Debug.LogWarning("No se encontró el panel de corazones");
            }
        }

        if (heartImages == null || heartImages.Length == 0)
        {
            if (heartsPanel != null)
            {
                heartImages = heartsPanel.GetComponentsInChildren<Image>();
                List<Image> hearts = new List<Image>();
                foreach (Image img in heartImages)
                {
                    if (img.gameObject.name.Contains("Heart") || img.gameObject.name.Contains("Corazon"))
                    {
                        hearts.Add(img);
                    }
                }
                if (hearts.Count > 0)
                    heartImages = hearts.ToArray();
            }
        }

        if (healthSystem != null && heartImages.Length != healthSystem.GetMaxLife())
        {
            Debug.LogWarning($" Número de corazones ({heartImages.Length}) no coincide con la vida máxima ({healthSystem.GetMaxLife()})");
        }
    }

    void UpdateHeartUI(int currentHealth)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentHealth)
            {
                if (fullHeartSprite != null)
                    heartImages[i].sprite = fullHeartSprite;
                heartImages[i].color = Color.white;
            }
            else
            {
                if (emptyHeartSprite != null)
                    heartImages[i].sprite = emptyHeartSprite;
                heartImages[i].color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }
    }
}
