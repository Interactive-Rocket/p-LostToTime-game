using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HUDManager))]
public class HUDTooltip : MonoBehaviour
{
    [SerializeField] private GameObject[] tooltipArray;
    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 endPosition;
    [SerializeField] private float animationDuration = 1.0f;
    private int currentIndex = 0;

    void Awake()
    {
        OnHideTooltip();
    }

    public void OnDisplayTooltip()
    {
        if (HUDManager.Instance != null) currentIndex = HUDManager.Instance.displayedTooltipIndex;
        OnDisplayTooltip(currentIndex);
    }

    public void OnDisplayTooltip(int index)
    {
        currentIndex = index;

        if (tooltipArray.Length > currentIndex)
        {

            if (tooltipArray.Length > currentIndex)
            {
                if (tooltipArray[currentIndex] != null)
                {
                    StopAllCoroutines();
                    OnHideTooltip();
                    tooltipArray[currentIndex].SetActive(true);
                    StartCoroutine(AnimateTooltip(tooltipArray[currentIndex]));
                }
            }
        }
    }

    public void OnHideTooltip()
    {
        if (tooltipArray.Length > 0)
        {
            for (int i = 0; i < tooltipArray.Length; i++)
            {
                if (tooltipArray[i] != null)
                {
                    tooltipArray[i].SetActive(false);
                    tooltipArray[i].transform.localPosition = startPosition;
                }
            }
        }
    }

    private IEnumerator AnimateTooltip(GameObject tooltip)
    {
        float elapsedTime = 0f;
        Vector2 initialPosition = startPosition;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            t = Mathf.SmoothStep(0f, 1f, t);
            tooltip.transform.localPosition = Vector2.Lerp(initialPosition, endPosition, t);
            yield return null;
        }

        tooltip.transform.localPosition = endPosition;
    }
}
