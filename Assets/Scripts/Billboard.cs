using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Header("Configuración")]
    public bool faceCamera = true;
    public float yOffset = 0f;
    
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
            
        if (mainCamera != null && faceCamera)
        {
            // Hacer que el texto mire a la cámara pero manteniendo su eje Y
            Vector3 targetPosition = new Vector3(mainCamera.transform.position.x, 
                                                  transform.position.y, 
                                                  mainCamera.transform.position.z);
            transform.LookAt(targetPosition);
            
            // Girar 180 grados si el texto se ve al revés
            transform.Rotate(0, 180, 0);
        }
    }
}