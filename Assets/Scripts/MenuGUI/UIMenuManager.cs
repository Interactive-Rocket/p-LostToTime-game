using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIMenuManager : MonoBehaviour
{
    protected GameObject currentMenuState;
    protected List<GameObject> menuStates;

    void Awake()
    {
        AssignSelfSingleton();
        SetMenuStates();
        SetMainCanvasActive();
    }

    protected abstract void AssignSelfSingleton();

    protected abstract void SetMenuStates();

    public abstract void SetMainCanvasActive();

    protected void ChangeMenuState(GameObject newState)
    {
        if (newState == null) return;
        currentMenuState = newState;

        foreach (var state in menuStates)
        {
            if (state != null) state.SetActive(state == newState);
        }
    }
}
