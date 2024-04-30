using UnityEngine;
using UnityEngine.Events;

public class HUDManager : MonoBehaviour
{
    [Header("HUD Manager Events")]
    public UnityEvent onInteractPrompt;
    public UnityEvent onPlayerDeathScreen;
    public static HUDManager Instance { get; private set; }

    private bool interactionPromptActive = false;
    private bool playerDeathScreenActive = false;

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