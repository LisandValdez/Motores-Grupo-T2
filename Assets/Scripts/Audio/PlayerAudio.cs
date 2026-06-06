using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Source Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float defaultVolume = 1f;
    [SerializeField] private float pitchVariance = 0.05f;

    void Reset()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0.5f; 
    }

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayOneShot(AudioClip clip, float volume = 1f, bool randomizePitch = true)
    {
        if (clip == null || audioSource == null) return;

        float originalPitch = audioSource.pitch;
        if (randomizePitch)
            audioSource.pitch = originalPitch + Random.Range(-pitchVariance, pitchVariance);

        audioSource.PlayOneShot(clip, Mathf.Clamp01(volume * defaultVolume));

        if (randomizePitch)
            audioSource.pitch = originalPitch;
    }

    public void PlayLoop(AudioClip clip, float volume = 1f)
    {
        if (clip == null || audioSource == null) return;
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = Mathf.Clamp01(volume * defaultVolume);
        audioSource.Play();
    }

    public void StopLoop()
    {
        if (audioSource == null) return;
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
    }

    public void SetVolume(float v)
    {
        defaultVolume = Mathf.Clamp01(v);
        if (audioSource != null)
            audioSource.volume = defaultVolume;
    }
}
