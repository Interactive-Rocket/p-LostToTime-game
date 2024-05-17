using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapInteractable : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update
    [Tooltip("Events that happend when player interacts with the map")]
    public UnityEvent OnMapInteracted;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        Debug.Log("Interaction with Map");
        OnMapInteracted.Invoke();
    }


}
