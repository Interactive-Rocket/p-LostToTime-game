using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InputManager))]
public class PlayerInteract : MonoBehaviour
{
    public float InteractionRange = 10f;
    private IInteractable focusedInteractable = null;
    private IHoverable focusedHoverable = null;
    private InputManager _input;
    public AudioClip interactSound;
    public AudioClip interactGrabbedSound;
    public AudioClip interactFailedSound;
	public UnityAction interactActions;
    private int layerMask = ~(1 << 6);

    void Awake()
    {
        _input = GetComponent<InputManager>();
        interactActions += SendInteract;
    }

    private void Update()
    {
        CheckForInteractable();

        if (_input.IsInteracting())
        {
            interactActions.Invoke();
        }
    }

    private void LateUpdate()
    {
        /* This might not work if we have the interaction and grabbing be the same button
           Keep it for now, but if this presents any issues we scrap it.
           Ideally, if we have a component on grabbable objects we can check for that, and
           if no such component exists we can make _input.InteractInput false.
        */
        _input.InteractInput(false);
    }

    private void CheckForInteractable()
    {
        Vector3 screenCenterPoint = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        bool interactableInRange = Physics.Raycast(ray, out RaycastHit hitInfo, InteractionRange,layerMask);

        // Interactable interface
        if (interactableInRange && hitInfo.collider.gameObject.TryGetComponent(out IInteractable tempInteractable))
        {
            focusedInteractable = tempInteractable;
            if (HUDManager.Instance != null)
            {
                if (focusedInteractable is IInteractableHand interactableHand) HUDManager.Instance.HoveringGrabbable = interactableHand.ShowInteractableHand();
                else HUDManager.Instance.HoveringGrabbable = true;
            }
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

            if (focusedInteractable is not IInteractableSound && AudioManager.Instance != null)
            {
                if (focusedInteractable is GrabbedController) AudioManager.Instance.PlayOneShot(interactGrabbedSound);
                else AudioManager.Instance.PlayOneShot(interactSound);
            }
        }
        else if (AudioManager.Instance != null) AudioManager.Instance.PlayOneShot(interactFailedSound);
    }
}

/* Base interactable interface
 * 
*/
public interface IInteractable
{
    public void Interact();
}

/* Hoverable interface
 * Sends pulses on hover and unhover.
 * Doesn't do much else, needs to be called separately because it only
 * calls these functions when hovering and unhovering. 
*/
public interface IHoverable
{
    public void Hover();
    public void Unhover();
}

/* Interactable Sound Interface
 * If we wish to play a sound different from the default interact sound,
 * extend implement this interface directly on the script of the IInteractable.
*/
public interface IInteractableSound
{
    public void PlaySound();
}

/* Interactable Hand Interface
 * If the Interactable should show the animated hand or not, really only useful for
 * Interactables which can't really be used (like locked doors).
*/
public interface IInteractableHand
{
    public bool ShowInteractableHand();
}
