using UnityEngine;

public class Knife : MeleeBase
{
    protected override void OnMeleeStrike()
    {
        Debug.Log("Ataque de cuchillo ejecutado");
    }
}