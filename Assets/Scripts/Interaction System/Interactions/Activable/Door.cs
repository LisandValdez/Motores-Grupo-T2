using UnityEngine;

public class Door : ActivableBase
{
    [SerializeField] private float openAngle = 90f;   // Ángulo de apertura
    [SerializeField] private float speed = 2f;        // Velocidad de rotación
    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Start()
    {
        // Guardamos la rotación inicial como "cerrada"
        closedRotation = transform.rotation;
        // Calculamos la rotación abierta sumando el ángulo en Y
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    protected override void Activate()
    {
        isOpen = !isOpen;
        Debug.Log("Puerta " + (isOpen ? "abierta" : "cerrada"));
    }

    private void Update()
    {
        // Interpolamos suavemente entre abierta y cerrada
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
    }
}