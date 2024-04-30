using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Tooltip("How many audio sources/tracks we have at once.")]
    [SerializeField]
    private int audioSourceCount = 5;
    private AudioSource[] audioSources;
    private AudioSource oneShotAudioSource;
    private int currentIndex = 0;


    private void Awake()
    {
        // Ensure it's a singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSources = new AudioSource[audioSourceCount];
        for (int i = 0; i < audioSourceCount; i++)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSources[i] = audioSource;
        }
        oneShotAudioSource = gameObject.GetComponent<AudioSource>();
    }

    // For playing longer sound clips that we might want to pause or stop
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        AudioSource audioSource = audioSources[currentIndex];

        if (audioSource.isPlaying)
        {
            // Hope that the next audiosource isn't playing anything
            currentIndex = (currentIndex + 1) % audioSources.Length;
            audioSource = audioSources[currentIndex];
        }

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();

        currentIndex = (currentIndex + 1) % audioSources.Length;
    }

    // For smaller sounds like footsteps etc just use this
    public void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        oneShotAudioSource.PlayOneShot(clip, volume);
    }

    // Overloaded, plays a random sound from an array
    public void PlayOneShot(AudioClip[] clips, float volume = 1f)
    {
        if (clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        oneShotAudioSource.PlayOneShot(clips[index], volume);
    }

    public void StopAllSounds()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].Stop();
        }
    }
}