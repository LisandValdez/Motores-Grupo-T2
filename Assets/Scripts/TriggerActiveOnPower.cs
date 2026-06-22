using UnityEngine;
using System.Collections;

public class TriggerActiveOnPower : MonoBehaviour
{
    [Header("Configuración del Evento")]
    [Tooltip("El objeto de la jerarquía que quieres activar (ej: un enemigo, una luz, un ítem).")]
    [SerializeField] private GameObject objetoAActivar;

    [Tooltip("¿Quieres que este trigger deje de funcionar después de activarse una vez?")]
    [SerializeField] private bool destruirAlActivar = true;

    [Header("Sistema de Diálogos")]
    [Tooltip("¿Quieres que salte un diálogo al activarse el objeto?")]
    [SerializeField] private bool activarDialogo = true;

    [Tooltip("Define aquí las líneas de texto y qué personaje las dice al pisar el trigger.")]
    [SerializeField] private Dialogue dialogoAlActivar;

    [Header("Audio Post-Diálogo (NUEVO)")]
    [Tooltip("Arrastra aquí el AudioSource del objeto que quieres que empiece a sonar AL TERMINAR el diálogo (ej: un grito, una alarma, música de persecución).")]
    [SerializeField] private AudioSource audioSourceExterno;

    private void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"❌ {gameObject.name} necesita un Collider para funcionar.");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning($"El Collider de {gameObject.name} no está en modo 'Is Trigger'.");
            col.isTrigger = true;
        }

        if (objetoAActivar != null && objetoAActivar.activeSelf)
        {
            objetoAActivar.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FuseBox fuseBox = FindFirstObjectByType<FuseBox>();

            if (fuseBox != null)
            {
                if (fuseBox.isCompleted)
                {
                    EjecutarEvento();
                }
                else
                {
                    Debug.Log($" El jugador pisó el trigger {gameObject.name}, pero la energía aún está apagada.");
                }
            }
            else
            {
                Debug.LogWarning("No se encontró ninguna FuseBox en la escena para comprobar la energía.");
            }
        }
    }

    private void EjecutarEvento()
    {
        if (objetoAActivar != null)
        {
            objetoAActivar.SetActive(true);
            Debug.Log($" [TRIGGER] Activando el objeto: {objetoAActivar.name}");
        }

        if (activarDialogo && dialogoAlActivar != null && dialogoAlActivar.lines != null && dialogoAlActivar.lines.Length > 0)
        {
            if (DialogueManager.Instance != null)
            {
    
                StartCoroutine(SecuenciaDialogoYAudio());
            }
            else
            {
                Debug.LogWarning("No se encontró el DialogueManager. Reproduciendo audio de inmediato.");
                ReproducirAudioExterno();
                ManejarDestruccion();
            }
        }
        else
        {

            ReproducirAudioExterno();
            ManejarDestruccion();
        }
    }

    private IEnumerator SecuenciaDialogoYAudio()
    {

        DialogueManager.Instance.StartDialogue(dialogoAlActivar);

        yield return new WaitUntil(() => Time.timeScale > 0f);

        ReproducirAudioExterno();

        ManejarDestruccion();
    }

    private void ReproducirAudioExterno()
    {
        if (audioSourceExterno != null)
        {
            audioSourceExterno.Play();
            Debug.Log($"🔊 [AUDIO] Reproduciendo sonido post-diálogo desde: {audioSourceExterno.gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"El trigger {gameObject.name} terminó su diálogo pero no tiene un 'Audio Source Externo' asignado.");
        }
    }

    private void ManejarDestruccion()
    {
        if (destruirAlActivar)
        {
            GetComponent<Collider>().enabled = false;
            this.enabled = false;
            Destroy(gameObject, 0.1f);
        }
    }
}