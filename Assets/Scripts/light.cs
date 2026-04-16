using UnityEngine;
using UnityEngine.InputSystem;

public class light : MonoBehaviour
{
    public Light flashlight;

    bool isOn = true;

    void Start()
    {
        flashlight.enabled = isOn;
    }

    public void OnFlashlight(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("Flashlight pressed");

            isOn = !isOn;
            flashlight.enabled = isOn;
        }
    }
}