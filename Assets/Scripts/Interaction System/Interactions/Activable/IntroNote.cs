using UnityEngine;

public class IntroNote : ExamineObject
{
    // 🔴 VARIABLE GLOBAL: Controla si la nota ya fue leída en toda la partida
    public static bool HasReadIntroNote { get; private set; } = false;

    protected override void Activate()
    {
        // Si el jugador ya leyó la nota, se comporta como un objeto examinable común
        if (HasReadIntroNote)
        {
            base.Activate();
            return;
        }

        // Si es la primera vez que la lee, disparamos el diálogo normal
        base.Activate();

        // Marcamos la nota como leída
        HasReadIntroNote = true;
        Debug.Log("📜 [STORY] El jugador ha leído la nota de introducción. Puertas habilitadas.");
    }
}