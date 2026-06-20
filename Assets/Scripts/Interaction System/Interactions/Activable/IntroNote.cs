using UnityEngine;
using System.Collections;

public class IntroNote : ExamineObject
{
    // Variable estática que controla las puertas
    public static bool HasReadIntroNote { get; private set; } = false;

    [Header("Secuencia del Apagón")]
    [Tooltip("El diálogo que dirá el personaje JUSTO DESPUÉS de quedarse a oscuras.")]
    [SerializeField] private Dialogue reaccionApagonDialogue;

    // 🌟 NUEVO MÉTODO: Reinicia el estado global al recargar la escena
    private void Awake()
    {
        HasReadIntroNote = false;
        Debug.Log("📜 [STORY] Reiniciando estado de la nota inicial (Memoria estática limpia).");
    }

    protected override void Activate()
    {
        // Si ya completó este evento (en este intento actual), examen normal
        if (HasReadIntroNote)
        {
            base.Activate();
            return;
        }

        // Si es la primera vez en esta vida, iniciamos la secuencia
        StartCoroutine(SecuenciaNotaYApagon());
    }

    private IEnumerator SecuenciaNotaYApagon()
    {
        // 1. Muestra el texto de la nota
        base.Activate();

        // 2. Esperamos de forma segura hasta que el jugador cierre el diálogo
        yield return new WaitUntil(() => Time.timeScale > 0f);

        // 3. Esperamos 0.5 segundos con luz
        yield return new WaitForSeconds(0.5f);

        // 4. Provocamos el apagón instantáneo
        if (LightingManager.Instance != null)
        {
            LightingManager.Instance.ApagarLuces();
        }

        // 5. Activamos la bandera global (habilita las puertas de esta sesión)
        HasReadIntroNote = true;

        // 6. Pausa dramática a oscuras
        yield return new WaitForSeconds(0.2f);

        // 7. Lanzamos el texto de reacción a oscuras
        if (DialogueManager.Instance != null && reaccionApagonDialogue != null && reaccionApagonDialogue.lines.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(reaccionApagonDialogue);
        }
    }
}