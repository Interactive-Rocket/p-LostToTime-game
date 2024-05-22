using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [SerializeField]
    private string[] m_objectives =
    {
        "Scape from the lab",
        "Cross the chasm",
        "Jump the fence",
        "Rescue your Daughter",
    };
    [Header("Level Objective")]
    [Tooltip("Set the objective depending on the Level")]
    [SerializeField] private int m_currentObjective;
    [SerializeField] private float m_displayTime = 5.0f;
    private const float MINIMUM_READABLE_TIME = 5.0f;
    public float DisplayTime => m_displayTime < MINIMUM_READABLE_TIME ? MINIMUM_READABLE_TIME : m_displayTime;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetObjective(int objectiveIndex)
    {
        if (objectiveIndex >= m_objectives.Length || objectiveIndex < 0) Debug.LogError("To set an objective, pick a number between 0 and " + (m_objectives.Length - 1));
        else m_currentObjective = objectiveIndex;
    }

    public string GetObjective()
    {
        return m_objectives[m_currentObjective];
    }
}
