using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    [ContextMenu("Disparar Diálogo")] // Te permite probarlo desde el inspector haciendo clic derecho en el componente
    public void TriggerDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogue);
        }
        else
        {
            Debug.LogError("DialogueManager no ha sido encontrado en la escena. Asegúrate de tener el prefab/objeto creado.");
        }
    }
}