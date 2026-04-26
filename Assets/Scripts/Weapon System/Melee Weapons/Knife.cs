using UnityEngine;

public class Knife : MeleeBase
{
    protected override void OnMeleeStrike()
    {
        // El cuchillo no necesita lógica extra, la base hace todo
        Debug.Log("Ataque de cuchillo ejecutado");
    }
}