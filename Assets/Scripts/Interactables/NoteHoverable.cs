using UnityEngine;

public class NoteHoverable : MonoBehaviour, IHoverable
{
    [SerializeField] private int tooltipIndex = 0;
    private bool canDisplayTooltip = true;

    public void Hover()
    {
        //Debug.Log("Hovered note");
        if (HUDManager.Instance != null && canDisplayTooltip)
        {
            HUDManager.Instance.displayedTooltipIndex = tooltipIndex;
            HUDManager.Instance.DisplayingTooltip = true;
        }
    }

    public void Unhover()
    {
        //Debug.Log("Unhovered note");
        if (HUDManager.Instance != null) HUDManager.Instance.DisplayingTooltip = false;
    }

    public void DisableHovering() {
        Unhover();
        canDisplayTooltip = false;
    }
}
