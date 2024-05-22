using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundInteractable : MonoBehaviour, IInteractable, IInteractableSound, IInteractableHand
{
    [Tooltip("Events that happen when this interactable is interacted with.")]
    public UnityEvent onInteracted;
    [Tooltip("If we should show the hand when hovering over this interactable.")]
    [SerializeField] private bool handVisible = false;
    [Tooltip("If we should use the AudioManager instead of the local AudioSource.")]
    [SerializeField] private bool useAudioManager = false;
    [Tooltip("Audio files to be played when the computer is interacted with.")]
    [SerializeField] private AudioClip[] interactSound;
    [SerializeField] private float interactVolume = 0.5f;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        onInteracted.Invoke();
        PlaySound();
    }

    public void PlaySound()
    {
        if (interactSound.Length == 0) return;

        int index = Random.Range(0, interactSound.Length);
        if (useAudioManager && AudioManager.Instance != null) AudioManager.Instance.PlayOneShot(interactSound[index], interactVolume);
        else
        {
            audioSource.Stop();
            audioSource.PlayOneShot(interactSound[index], interactVolume);
        }
    }

    public bool ShowInteractableHand()
    {
        return handVisible;
    }
}