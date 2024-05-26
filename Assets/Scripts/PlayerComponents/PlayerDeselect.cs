using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class PlayerDeselect : MonoBehaviour 
{
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
