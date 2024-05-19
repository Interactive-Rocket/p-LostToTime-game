using UnityEngine;

[RequireComponent(typeof(HUDManager))]
public class HUDTooltip : MonoBehaviour
{
    [SerializeField] private GameObject[] tooltipArray;
    private int currentIndex = 0;

    void Awake()
    {
        OnHideTooltip();
    }

    public void OnDisplayTooltip()
    {
        if (tooltipArray != null && HUDManager.Instance != null) currentIndex = HUDManager.Instance.displayedTooltipIndex;
        OnDisplayTooltip(currentIndex);
    }

    public void OnDisplayTooltip(int index)
    {
        currentIndex = index;

        if (tooltipArray.Length > currentIndex)
        {
            if (tooltipArray[currentIndex] != null) tooltipArray[currentIndex].SetActive(true);
        }
    }

    public void OnHideTooltip()
    {
        if (tooltipArray.Length > 0)
        {
            for (int i = 0; i < tooltipArray.Length; i++)
            {
                if (tooltipArray[i] != null) tooltipArray[i].SetActive(false);
            }
        }
    }
}
