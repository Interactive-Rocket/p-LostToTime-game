using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDObjectivePrompt : MonoBehaviour
{
    [SerializeField] private GameObject ObjectivePrompt;
    private TMP_Text[] m_textMeshes; // TMP_Text is the wierd name of the TextMeshPro component
    private float m_displayLifeTime = 10f;
    private float m_lifeTimeCounter = 0f;
    void Awake()
    {
        if (ObjectivePrompt)
        {
            ObjectivePrompt.SetActive(false);
            m_textMeshes = ObjectivePrompt.GetComponentsInChildren<TMP_Text>();
            Debug.Log("TextMesh:" + m_textMeshes.Length + ", content: " + m_textMeshes[0].text);
        }
    }

    public void OnDisplayObjective()
    {
        if (ObjectivePrompt)
        {
            //Debug.Log("HUDObjectivePrompt -> NEW Objective:" + ObjectiveManager.Instance.GetObjective());
            m_textMeshes[0].text = ObjectiveManager.Instance.GetObjective();
            ObjectivePrompt.SetActive(true);
        }
    }

    void Update()
    {
        if (ObjectivePrompt.activeSelf) DisplayObjectiveLifeTimeCountdown();
    }

    private void DisplayObjectiveLifeTimeCountdown()
    {
        m_lifeTimeCounter += Time.deltaTime;
        if (m_lifeTimeCounter > m_displayLifeTime)
        {
            ObjectivePrompt.SetActive(false);
            m_lifeTimeCounter = 0;
        }
    }
}
