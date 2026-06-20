using UnityEngine;

[System.Serializable]
public struct DialogueLine
{
    public string characterName; // Quién habla
    [TextArea(3, 5)]
    public string text;          // Qué dice
}

[System.Serializable]
public class Dialogue
{
    public DialogueLine[] lines; // Array de líneas de la conversación
}