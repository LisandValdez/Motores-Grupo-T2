using UnityEngine;
using System.Collections;

public class IntroNote : ExamineObject
{
    // Variable estática que controla las puertas
    public static bool HasReadIntroNote { get; private set; } = false;

    [Header("Secuencia del Apagón")]
    [Tooltip("El diálogo que dirá el personaje JUSTO DESPUÉS de quedarse a oscuras.")]
    [SerializeField] private Dialogue reaccionApagonDialogue;

    private void Awake()
    {
        HasReadIntroNote = false;
        Debug.Log("[STORY] Reiniciando estado de la nota inicial (Memoria estática limpia).");
    }

    protected override void Activate()
    {

        if (HasReadIntroNote)
        {
            base.Activate();
            return;
        }

        StartCoroutine(SecuenciaNotaYApagon());
    }

    private IEnumerator SecuenciaNotaYApagon()
    {
        base.Activate();

        yield return new WaitUntil(() => Time.timeScale > 0f);

        yield return new WaitForSeconds(0.5f);

        if (LightingManager.Instance != null)
        {
            LightingManager.Instance.ApagarLuces();
        }

        HasReadIntroNote = true;

        yield return new WaitForSeconds(0.2f);

        if (DialogueManager.Instance != null && reaccionApagonDialogue != null && reaccionApagonDialogue.lines.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(reaccionApagonDialogue);
        }
    }
}