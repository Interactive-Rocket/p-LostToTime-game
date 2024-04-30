using UnityEngine;

[RequireComponent(typeof(HUDManager))]
public class HUDInteractionPrompt : MonoBehaviour
{
    [SerializeField] private GameObject interactionPrompt;

    void Awake()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    public void OnInteractPrompt()
    {
        if (interactionPrompt != null && HUDManager.Instance != null)
        {
            interactionPrompt.SetActive(HUDManager.Instance.InteractionPrompt);
        }
    }
}
