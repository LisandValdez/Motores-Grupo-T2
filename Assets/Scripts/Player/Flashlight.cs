using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
    public Light flashlight; 
    public AudioSource Sound;
    private bool isOn = false;

    void Start()
    {
        // Para que empice apagada
        if (flashlight != null)
        {
            flashlight.enabled = false;
        }
    }
    public void OnFlash(InputAction.CallbackContext context)
    {
        
        if (context.started)
        {
            isOn = !isOn;
            flashlight.enabled = isOn;
            Sound.Play();
        }
    }
}