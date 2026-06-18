using UnityEngine;
using TMPro;

public class DialogueCanvasBinder : MonoBehaviour
{
    [Header("Elementos locales de esta escena")]
    public GameObject dialoguePanel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    [Header("UI externa de esta escena")]
    [Tooltip("Arrastra aquí el Canvas principal (vida, munición) de ESTA escena.")]
    public GameObject gameplayCanvas;

    void Start()
    {
        // Buscamos al DialogueManager persistente y le inyectamos los elementos de este nivel
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.RegisterUIElements(dialoguePanel, nameText, dialogueText, gameplayCanvas);
            Debug.Log("✨ DialogueManager enlazado correctamente con la UI de esta escena.");
        }
        else
        {
            Debug.LogError("⚠️ No se encontró un DialogueManager en la escena para enlazar la UI.");
        }
    }
}