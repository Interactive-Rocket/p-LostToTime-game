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
    public AudioClip interactSound;

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
            AudioManager.Instance.PlayOneShot(interactSound);
        }
    }
}
