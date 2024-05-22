using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDObjectivePrompt : MonoBehaviour
{
    [SerializeField] private GameObject ObjectivePrompt;
    void Awake()
    {
        if (ObjectivePrompt) ObjectivePrompt.SetActive(false);
    }

    public void OnDisplayObjective()
    {
        if (ObjectivePrompt)
        {
            //
            Debug.Log("HUDObjectivePrompt -> NEW Objective:" + ObjectiveManager.Instance.GetObjective());
            ObjectivePrompt.SetActive(true);
        }
    }
}
