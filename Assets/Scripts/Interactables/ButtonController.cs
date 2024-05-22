using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SoundController))]
[RequireComponent(typeof(TimeEntity))]
public class ButtonController : MonoBehaviour, IInteractable, IInteractableSound
{
    [Tooltip("Events that happen when a button is pressed")]
    public UnityEvent onButtonPressed;
    [Tooltip("Events that happen when a button is unpressed")]
    public UnityEvent onButtonUnpressed;
    public bool isButtonPushed = false;
    [Tooltip("How long between button presses")]
    public float cooldown = 5f;
    public float cooldownTime = 0f;
    private TimeEntity timeEntity;
    //private MeshRenderer meshRenderer;
    public bool wasRewinding = false;
    private bool wasPushed = false;

    public Color defaultColour;
    public Color activatedColour;
    public AudioClip pressSound;
    public AudioClip failedSound;
    [SerializeField] private float failedInteractVolume = 0.5f;
    public AudioClip releaseSound;
    private AudioClip currentAudioClip;
    private SoundController soundController;

    void Start()
    {
        timeEntity = GetComponent<TimeEntity>();
        //meshRenderer = GetComponent<MeshRenderer>();
        //meshRenderer.material.color = defaultColour;
        soundController = GetComponent<SoundController>();
    }

    void Update()
    {
        if (isButtonPushed && !(timeEntity.IsRewinding || timeEntity.IsRewinding))
        {
            cooldownTime += Time.deltaTime;

            // Needs to be handled in a manner like this because we need to store the cooldown time
            if (cooldownTime >= cooldown)
            {
                UnpushButton();
                cooldownTime = 0f;
            }

            /* Depending on behaviour, we might want to have the button signal be "pressed" when
             * coming out of reversing time, maybe have the signal last for (cooldown - cooldownTime)
             * seconds in this case instead of the full cooldown time? Bypass with separate
             * onButtonUnpressed events? Potentially the signal can be pressed at every time step,
             * maybe have a flag for this behaviour?
            */
            else if (wasRewinding)
            {
                wasRewinding = false;
                PushButton();
            }

        }
        else if (isButtonPushed)
        {
            PushButton();
        }
        else if (!isButtonPushed && wasRewinding && wasPushed)
        {
            UnpushButton();
            cooldownTime = 0f;
        }

        wasPushed = isButtonPushed;

        wasRewinding = timeEntity.IsRewinding || timeEntity._isStopped;

        //this should be where we animate
        //meshRenderer.material.color = isButtonPushed ? activatedColour : defaultColour;
    }

    public void Interact()
    {
        if (!isButtonPushed) PushButton();
        else if (AudioManager.Instance != null) AudioManager.Instance.PlayOneShot(failedSound, failedInteractVolume);
    }

    private void PushButton()
    {
        // Check that the button gets pushed when we push it and exit rewind (local or global)
        Debug.Log("Button pushed");
        // This is here in case we wish to play a different sound when the button is pushed
        // and we have some logic to prevent it from being pushed when already pressed.

        currentAudioClip = pressSound;
        PlaySound();
        isButtonPushed = true;
        onButtonPressed.Invoke();
    }

    private void UnpushButton()
    {
        // Check that the button gets pushed when we push it and exit rewind (local or global)
        Debug.Log("Button unpushed");
        currentAudioClip = releaseSound;
        PlaySound();
        isButtonPushed = false;
        onButtonUnpressed.Invoke();
    }

    public void PlaySound()
    {
        soundController.Play(currentAudioClip);
    }
}