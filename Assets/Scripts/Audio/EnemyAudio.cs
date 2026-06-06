using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip dieClip;
    private AudioSource sound;

    void Awake()
    {
        sound = GetComponent<AudioSource>();
        if (sound != null)
        {
            sound.playOnAwake = false;
            
        }
        else
        {
            Debug.LogError("EnemyAudio necesita un AudioSource en el mismo GameObject.");
        }
    }

    public void PlayAttack() => PlayOneShot(attackClip);
    public void PlayHurt() => PlayOneShot(hurtClip);
    public void PlayDie() => PlayOneShot(dieClip);

    // Opción con volumen por llamada
    private void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sound == null) return;
        sound.PlayOneShot(clip, Mathf.Clamp01(volume));
    }
}
