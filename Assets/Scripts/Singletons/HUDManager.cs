using UnityEngine;
using UnityEngine.Events;

public class HUDManager : MonoBehaviour
{
    [Header("HUD Manager Events")]
    public UnityEvent onInteractPrompt;
    public UnityEvent onPlayerDeathScreen;
    public UnityEvent onHoveringGrabbable;
    public UnityEvent onNotHoveringGrabbable;
    public UnityEvent onDisplayTooltip;
    public UnityEvent onNotDisplayTooltip;
    public static HUDManager Instance { get; private set; }

    private bool interactionPromptActive = false;
    private bool playerDeathScreenActive = false;
    private bool hoveringGrabbableObject = false;
    private bool displayingTooltip = false;
    public int displayedTooltipIndex = 0;

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

    public bool DisplayingTooltip
    {
        get => displayingTooltip;
        set
        {
            displayingTooltip = value;

            if (value) onDisplayTooltip.Invoke();
            else onNotDisplayTooltip.Invoke();
        }
    }
    //TODO ------- create an accessor for calling the Objective Prompt
    private bool objectivePromptActive = false; //? I propose use objectivePromptState instead
    public bool ObjectivePrompt
    {
        get => objectivePromptActive;
        set
        {
            if (value && ObjectiveManager.Instance != null) Debug.Log("HUD -> NEW Objective:" + ObjectiveManager.Instance.GetObjective());
            objectivePromptActive = value;
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