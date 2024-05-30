using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class ComputerInteractable : MonoBehaviour, IInteractable, IInteractableSound
{
    [Tooltip("Events that happen when the computer is interacted with.")]
    public UnityEvent onComputerInteracted;
    [Tooltip("Audio files to be played when the computer is interacted with.")]
    [SerializeField] private AudioClip[] pressSound;
    [SerializeField] private float pressVolume = 0.5f;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        PushButton();
    }

    private void PushButton()
    {
        Debug.Log("Button pushed");
        onComputerInteracted.Invoke();
        PlaySound();
    }

    public void PlaySound()
    {
        if (pressSound.Length == 0) return;

        int index = Random.Range(0, pressSound.Length);
        audioSource.PlayOneShot(pressSound[index], pressVolume);
    }
}