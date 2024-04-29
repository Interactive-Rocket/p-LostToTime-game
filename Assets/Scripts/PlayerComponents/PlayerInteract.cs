using UnityEngine;

public interface IInteractable
{
    public void Interact();
}

public class PlayerInteract : MonoBehaviour
{
    public static PlayerInteract Instance { get; private set; }
    public float InteractionRange = 10f;
    private IInteractable focusedInteractable = null;
    public bool InteractionPromptActive { get; private set; } = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Only one interact manager may exist at once!");
            return;
        }
    }

    private void Update()
    {
        CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        Vector3 screenCenterPoint = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        bool interactableInRange = Physics.Raycast(ray, out RaycastHit hitInfo, InteractionRange);

        if (interactableInRange && hitInfo.collider.gameObject.TryGetComponent(out IInteractable tempInteractable))
        {
            focusedInteractable = tempInteractable;
            InteractionPromptActive = true;
        }
        else
        {
            focusedInteractable = null;
            InteractionPromptActive = false;
        }
    }

    public void SendInteract()
    {
        if (focusedInteractable != null)
        {
            focusedInteractable.Interact();
            // additional things could be added here, such as interact audio
        }
    }
}
