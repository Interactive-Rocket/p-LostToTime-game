using UnityEngine;

public interface IInteractable
{
    public void Interact();
}

[RequireComponent(typeof(InputManager))]
public class PlayerInteract : MonoBehaviour
{
    public float InteractionRange = 10f;
    private IInteractable focusedInteractable = null;
    private InputManager _input;

    void Awake()
    {
        _input = GetComponent<InputManager>();
    }

    private void Update()
    {
        CheckForInteractable();

        if (_input.IsInteracting())
        {
            SendInteract();

            /* This might not work if we have the interaction and grabbing be the same button
               Keep it for now, but if this presents any issues we scrap it.
               Ideally, if we have a component on grabbable objects we can check for that, and
               if no such component exists we can make _input.InteractInput false.
            */
            _input.InteractInput(false);
        }
    }

    private void CheckForInteractable()
    {
        Vector3 screenCenterPoint = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        bool interactableInRange = Physics.Raycast(ray, out RaycastHit hitInfo, InteractionRange);

        if (interactableInRange && hitInfo.collider.gameObject.TryGetComponent(out IInteractable tempInteractable))
        {  
            focusedInteractable = tempInteractable;
            if (HUDManager.Instance != null) HUDManager.Instance.InteractionPrompt = true;
        }
        else
        {
            focusedInteractable = null;
            if (HUDManager.Instance != null) HUDManager.Instance.InteractionPrompt = false;
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
