using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDObjectivePrompt : MonoBehaviour
{
    [SerializeField] private GameObject ObjectivePrompt;
    private TMP_Text[] m_textMeshes; // TMP_Text is the wierd name of the TextMeshPro component
    private RectTransform[] m_images; // TMP_Text is the wierd name of the TextMeshPro component
    private float m_displayLifeTime = 10f;
    private float m_lifeTimeCounter = 0f;
    void Awake()
    {
        if (ObjectivePrompt)
        {
            ObjectivePrompt.SetActive(false);
            m_textMeshes = ObjectivePrompt.GetComponentsInChildren<TMP_Text>();
            m_images = ObjectivePrompt.GetComponentsInChildren<RectTransform>();

            Debug.Log("m_images:" + m_images.Length + ", content: " + m_images[2]);

            m_displayLifeTime = ObjectiveManager.Instance?  ObjectiveManager.Instance.DisplayTime : 10f;
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
        m_images[2].localScale = new Vector3(1.0f - (m_lifeTimeCounter / m_displayLifeTime), 1f, 1f);

        if (m_lifeTimeCounter > m_displayLifeTime)
        {
            ObjectivePrompt.SetActive(false);
            m_lifeTimeCounter = 0;
            m_images[2].localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
