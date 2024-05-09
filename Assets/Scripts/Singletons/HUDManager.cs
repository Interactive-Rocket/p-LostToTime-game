using UnityEngine;
using UnityEngine.Events;

public class HUDManager : MonoBehaviour
{
    [Header("HUD Manager Events")]
    public UnityEvent onInteractPrompt;
    public UnityEvent onPlayerDeathScreen;
    public UnityEvent onHoveringGrabbable;
    public UnityEvent onNotHoveringGrabbable;
    public static HUDManager Instance { get; private set; }

    private bool interactionPromptActive = false;
    private bool playerDeathScreenActive = false;
    private bool hoveringGrabbableObject = false;

    public bool InteractionPrompt
    {
        get => interactionPromptActive;
        set
        {
            interactionPromptActive = value;
            onInteractPrompt.Invoke();
        }
    }

    public bool PlayerDeathScreen
    {
        get => playerDeathScreenActive;
        set
        {
            if (playerDeathScreenActive != value)
            {
                playerDeathScreenActive = value;
                onPlayerDeathScreen.Invoke();
            }
        }
    }

    public bool HoveringGrabbable
    {
        get => hoveringGrabbableObject;
        set
        {
            if (hoveringGrabbableObject != value)
            {
                hoveringGrabbableObject = value;

                if (value) onHoveringGrabbable.Invoke();
                else onNotHoveringGrabbable.Invoke();
            }
        }
    }

    void Awake()
    {
        // Ensure it's a singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}