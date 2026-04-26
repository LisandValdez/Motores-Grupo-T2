using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTriggerInteraction : MonoBehaviour
{
    [Header("Configuración")]
    public float interactionCooldown = 0.5f;
    
    private GameObject currentInteractable;
    private float lastInteractionTime = 0f;
    private GameObject currentPrompt;

    void Start()
    {
        CreateCrosshairPrompt();
    }

    void Update()
    {
        // Verificar interacción con tecla E
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (currentInteractable != null && Time.time > lastInteractionTime + interactionCooldown)
            {
                IInteractable interactable = currentInteractable.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                    lastInteractionTime = Time.time;
                }
            }
        }
        
        // Mostrar/ocultar prompt
        if (currentPrompt != null)
            currentPrompt.SetActive(currentInteractable != null);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IInteractable>() != null)
        {
            currentInteractable = other.gameObject;
            Debug.Log($"✅ Entró en rango: {other.name}");
            UpdatePromptText(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (currentInteractable == other.gameObject)
        {
            currentInteractable = null;
            Debug.Log($"❌ Salió del rango: {other.name}");
        }
    }
    
    void CreateCrosshairPrompt()
    {
        // Crear un pequeño texto en la pantalla
        GameObject canvasObj = new GameObject("InteractionPromptUI");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        GameObject textObj = new GameObject("PromptText");
        textObj.transform.SetParent(canvasObj.transform);
        
        UnityEngine.UI.Text text = textObj.AddComponent<UnityEngine.UI.Text>();
        text.text = "🔘 Presiona E";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 24;
        text.color = Color.yellow;
        text.alignment = UnityEngine.TextAnchor.MiddleCenter;
        
        RectTransform rect = text.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0, -100);
        rect.sizeDelta = new Vector2(200, 50);
        
        currentPrompt = canvasObj;
        canvasObj.SetActive(false);
    }
    
    void UpdatePromptText(GameObject target)
    {
        // Opcional: cambiar texto según el tipo de item
        ItemPickup item = target.GetComponent<ItemPickup>();
        if (item != null && currentPrompt != null)
        {
            var text = currentPrompt.GetComponentInChildren<UnityEngine.UI.Text>();
            if (text != null)
                text.text = $"🔘 Presiona E para agarrar {item.itemName}";
        }
    }
}