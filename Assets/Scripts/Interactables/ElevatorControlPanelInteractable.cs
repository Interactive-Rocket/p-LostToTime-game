using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ElevatorControlPanelInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEvent m_OnOpenDoors;
    
    public void Interact()
    {
        m_OnOpenDoors.Invoke();
    }
}
