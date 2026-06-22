using UnityEngine;

public class ExamineObject : ActivableBase
{
    [Header("Configuración del Examen")]
    [Tooltip("Define aquí las líneas de texto que el personaje pensará o dirá al examinar este objeto.")]
    [SerializeField] private Dialogue examineDialogue;

    // Se ejecuta automáticamente gracias a ActivableBase cuando el jugador interactúa con el objeto
    protected override void Activate()
    {
        if (examineDialogue != null && examineDialogue.lines != null && examineDialogue.lines.Length > 0)
        {
            // Disparamos el sistema de diálogos que pausará el juego y mostrará el texto en máquina de escribir
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(examineDialogue);
                Debug.Log($"Examinando objeto: {gameObject.name}");
            }
            else
            {
                Debug.LogWarning("No se encontró DialogueManager en la escena para mostrar el texto del examen.");
            }
        }
        else
        {
            Debug.LogWarning($"El objeto {gameObject.name} no tiene un diálogo de examen configurado.");
        }
    }
}