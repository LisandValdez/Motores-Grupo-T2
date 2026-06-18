using System.Collections;
using System.Collections.Generic;
using TMPro; // <- CAMBIO CRÍTICO: Importamos la librería de TextMeshPro
using UnityEngine.InputSystem;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Elementos de UI del Diálogo (TMP)")]
    public GameObject dialoguePanel;
    public TMP_Text nameText;       // <- CAMBIO: De 'Text' a 'TMP_Text'
    public TMP_Text dialogueText;   // <- CAMBIO: De 'Text' a 'TMP_Text'

    [Header("UI del Juego Externa")]
    [Tooltip("Arrastra aquí el Canvas principal de tu juego (vida, barcos, etc.) para ocultarlo durante el diálogo.")]
    public GameObject gameplayCanvas;

    [Header("Configuración")]
    public float typingSpeed = 0.02f; // Tiempo en segundos reales por letra

    private Queue<DialogueLine> linesQueue;
    private bool isTyping = false;
    private string currentLineText = "";
    private bool isDialogueActive = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        linesQueue = new Queue<DialogueLine>();
    }

    public void RegisterUIElements(GameObject panel, TMP_Text nameTxt, TMP_Text dialogueTxt, GameObject gameplayCanv)
    {
        dialoguePanel = panel;
        nameText = nameTxt;
        dialogueText = dialogueTxt;
        gameplayCanvas = gameplayCanv;

        // Ocultar el panel recién registrado por seguridad
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    void Update()
    {
        // Solo detectamos clics si hay un diálogo activo en pantalla
        // Usamos el nuevo Input System para leer el clic izquierdo del mouse de forma directa
        if (isDialogueActive && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue == null || dialogue.lines.Length == 0) return;

        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        linesQueue.Clear();

        if (gameplayCanvas != null)
        {
            gameplayCanvas.SetActive(false);
        }

        Time.timeScale = 0f;

        foreach (DialogueLine line in dialogue.lines)
        {
            linesQueue.Enqueue(line);
        }

        DisplayNextLine();
    }

    private void HandleClick()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = currentLineText;
            isTyping = false;
        }
        else
        {
            DisplayNextLine();
        }
    }

    public void DisplayNextLine()
    {
        if (linesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = linesQueue.Dequeue();
        nameText.text = currentLine.characterName;
        currentLineText = currentLine.text;

        StartCoroutine(TypeSentence(currentLineText));
    }

    IEnumerator TypeSentence(string sentence)
    {
        // 1. Le asignamos todo el texto de golpe al componente. 
        // Al hacer esto, TMP procesa internamente las etiquetas (<color>, <b>, etc.) y las oculta automáticamente.
        dialogueText.text = sentence;

        // 2. Forzamos a TextMeshPro a actualizar el entramado de texto para que calcule cuántas letras reales tiene
        dialogueText.ForceMeshUpdate();

        // 3. Escondemos todas las letras visibles poniendo el contador en cero
        dialogueText.maxVisibleCharacters = 0;
        isTyping = true;

        // 4. Obtenemos el total de caracteres reales de la frase (ignorando las etiquetas ocultas)
        int totalVisibleCharacters = dialogueText.textInfo.characterCount;

        // 5. Vamos revelando las letras reales una a una
        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            dialogueText.maxVisibleCharacters = i;

            // Espera en segundos reales (para que no le afecte la pausa)
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;

        if (gameplayCanvas != null)
        {
            gameplayCanvas.SetActive(true);
        }

        Time.timeScale = 1f;
    }
}