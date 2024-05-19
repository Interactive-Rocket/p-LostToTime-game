using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HUDCrosshairAnimator : MonoBehaviour
{
    private Image crosshairIcon;
    [SerializeField] private Sprite[] crosshairArray;
    [SerializeField] private float transitionTime = 0.03f;
    private bool currentState = false;
    private int currentIconIndex = 0;
    private Coroutine crosshairCoroutine;


    void Awake()
    {
        crosshairIcon = GetComponent<Image>();
        UpdateIcon();
    }

    private void UpdateIcon(int iconIndex = 0)
    {
        if (crosshairArray.Length < 1)
        {
            Debug.LogError("No sprites in the crosshair array");
            return;
        }

        crosshairIcon.sprite = crosshairArray[iconIndex];
    }

    public void CrosshairHovering()
    {
        currentState = true;
        if (crosshairCoroutine != null)
        {
            StopCoroutine(crosshairCoroutine);
        }
        crosshairCoroutine = StartCoroutine(AnimateCrosshair(true));
    }

    public void CrosshairNotHovering()
    {
        currentState = false;
        if (crosshairCoroutine != null)
        {
            StopCoroutine(crosshairCoroutine);
        }
        crosshairCoroutine = StartCoroutine(AnimateCrosshair(false));
    }

    private IEnumerator AnimateCrosshair(bool hovering)
    {
        if (hovering)
        {
            while (currentIconIndex < crosshairArray.Length - 1)
            {
                yield return new WaitForSeconds(transitionTime);
                currentIconIndex++;
                UpdateIcon(currentIconIndex);
            }
        }
        else
        {
            while (currentIconIndex > 0)
            {
                yield return new WaitForSeconds(transitionTime);
                currentIconIndex--;
                UpdateIcon(currentIconIndex);
            }
        }
    }
}