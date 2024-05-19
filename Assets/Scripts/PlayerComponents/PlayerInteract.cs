using UnityEngine;

public interface IInteractable
{
    public void Interact();
}

public interface IHoverable
{
    public void Hover();
    public void Unhover();
}

[RequireComponent(typeof(InputManager))]
public class PlayerInteract : MonoBehaviour
{
    public float InteractionRange = 10f;
    private IInteractable focusedInteractable = null;
    private IHoverable focusedHoverable = null;
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

        // Interactable interface
        if (interactableInRange && hitInfo.collider.gameObject.TryGetComponent(out IInteractable tempInteractable))
        {
            focusedInteractable = tempInteractable;
            if (HUDManager.Instance != null) HUDManager.Instance.HoveringGrabbable = true;
        }
        else
        {
            focusedInteractable = null;
            if (HUDManager.Instance != null) HUDManager.Instance.HoveringGrabbable = false;
        }

        // Hoverable interface, only send "pulses" when first hovering and unhovering
        if (interactableInRange && hitInfo.collider.gameObject.TryGetComponent(out IHoverable tempHoverable))
        {
            if (tempHoverable != focusedHoverable)
            {
                if (focusedHoverable != null) focusedHoverable.Unhover();
                focusedHoverable = tempHoverable;
                focusedHoverable.Hover();
            }
        }
        else
        {
            if (focusedHoverable != null) focusedHoverable.Unhover();
            focusedHoverable = null;
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
