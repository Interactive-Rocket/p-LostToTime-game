using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Update()
    {
        if (interactionPrompt != null && PlayerInteract.Instance != null)
        {
            if (PlayerInteract.Instance.InteractionPromptActive)
            {
                interactionPrompt.SetActive(true);
            }
            else
            {
                interactionPrompt.SetActive(false);
            }
        }
    }
}
