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
        sound.playOnAwake = false;
    }

    public void PlayAttack() => PlayOneShot(attackClip);
    public void PlayHurt() => PlayOneShot(hurtClip);
    public void PlayDie() => PlayOneShot(dieClip);

    private void PlayOneShot(AudioClip clip)
    {
        if (clip == null) return;
        sound.PlayOneShot(clip);
    }
}
