using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class PlayerDeselect : MonoBehaviour 
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

        if (_input.IsDeselecting())
        {
            DeselectAll();
        }
    }

    public void DeselectAll()
    {
        RewindManager.Instance.DeselectAll();
    }
}
