using UnityEngine;
using System.Collections;

public class ElevatorLightController : MonoBehaviour
{
    [Header("Configuración de Luz")]
    public Light elevatorLight;
    public float lightIntensity = 3f;
    public float lightRange = 8f;
    
    [Header("Efectos")]
    public float fadeInDuration = 0.5f;  // Duración del encendido
    public float flickerDuration = 0.3f; // Duración del parpadeo
    public bool playFlickerOnStart = true;
    
    [Header("Color")]
    public Color greenColor = Color.green;
    public Color offColor = Color.white;
    
    private bool isOn = false;
    private float originalIntensity;
    
    void Start()
    {
        if (elevatorLight == null)
            elevatorLight = GetComponent<Light>();
        
        if (elevatorLight == null)
        {
            Debug.LogError("No se encontró componente Light en " + gameObject.name);
            return;
        }
        
        // Guardar configuración original
        originalIntensity = elevatorLight.intensity;
        
        // Apagar luz inicialmente
        elevatorLight.enabled = false;
        elevatorLight.intensity = 0;
    }
    
    // Método principal para encender la luz verde
    public void TurnOnGreen()
    {
        if (!isOn)
        {
            StartCoroutine(ActivateGreenLight());
        }
    }
    
    IEnumerator ActivateGreenLight()
    {
        isOn = true;
        
        // Activar la luz
        elevatorLight.enabled = true;
        
        // Cambiar color a verde
        elevatorLight.color = greenColor;
        
        // Parpadeo inicial (como si estuviera encendiéndose)
        if (playFlickerOnStart)
        {
            yield return StartCoroutine(Flicker());
        }
        
        // Encendido suave (fade in)
        float elapsedTime = 0;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeInDuration;
            elevatorLight.intensity = Mathf.Lerp(0, lightIntensity, t);
            yield return null;
        }
        
        // Asegurar intensidad final
        elevatorLight.intensity = lightIntensity;
        elevatorLight.range = lightRange;
        
        Debug.Log("💡 Luz verde del ascensor encendida");
    }
    
    IEnumerator Flicker()
    {
        float flickerTime = 0;
        float flickerSpeed = 0.05f;
        
        while (flickerTime < flickerDuration)
        {
            // Parpadeo rápido
            elevatorLight.intensity = Random.Range(0.5f, 2f);
            yield return new WaitForSeconds(flickerSpeed);
            flickerTime += flickerSpeed;
        }
        
        elevatorLight.intensity = 0;
        yield return new WaitForSeconds(0.1f);
    }
    
    // Método para apagar la luz
    public void TurnOff()
    {
        StartCoroutine(FadeOut());
    }
    
    IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        float startIntensity = elevatorLight.intensity;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeInDuration;
            elevatorLight.intensity = Mathf.Lerp(startIntensity, 0, t);
            yield return null;
        }
        
        elevatorLight.enabled = false;
        isOn = false;
    }
}