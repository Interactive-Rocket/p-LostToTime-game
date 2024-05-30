using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SnapshotsPathTracer : MonoBehaviour
{
    // Start is called before the first frame update
    //public TargetedTimeEntity targetedTimeEntity;
    private LinkedList<TimeSnapshot> snapshots;

    // Creates a line renderer that follows a Sin() function
    // and animates it.

    public int lengthOfLineRenderer = 20;
    private bool _isPathVisible = false;

    LineRenderer lineRenderer;

    void Update()
    {

    }

    public void RemoveSelf()
    {
        Destroy(lineRenderer);
        Destroy(this);
    }

    public void ChangeMaterial(Material newMaterial)
    {
        if (lineRenderer != null)
        {
            // Set the material and ensure it supports transparency
            lineRenderer.material = newMaterial;
            lineRenderer.material.SetFloat("_Mode", 3);
            lineRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineRenderer.material.SetInt("_ZWrite", 0);
            lineRenderer.material.DisableKeyword("_ALPHATEST_ON");
            lineRenderer.material.EnableKeyword("_ALPHABLEND_ON");
            lineRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            lineRenderer.material.renderQueue = 3000;

        }
        else
        {
            Debug.Log("asdf lineRenderer is not initialized");
        }
    }

    public void ViewPath()
    {
        if (snapshots != null && _isPathVisible)
        {
            lineRenderer.positionCount = snapshots.Count;

            var points = new Vector3[snapshots.Count];
            var i = 0;
            foreach (TimeSnapshot snapshot in snapshots)
            {
                points[i] = snapshot.Position;
                i++;
            }

            lineRenderer.SetPositions(points);

            // Simplify the line to smooth out small artifacts
            lineRenderer.Simplify(0.05f); // Adjust the tolerance value as needed
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    public LineRenderer SetUpLineRenderer(LinkedList<TimeSnapshot> snapshots)
    {
        Debug.Log("Setting up line renderer with snapshots: " + snapshots.Count);
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.widthMultiplier = 1.0f; // Use widthMultiplier to scale the widths defined by the curve
        lineRenderer.textureMode = LineTextureMode.DistributePerSegment;
        lineRenderer.shadowCastingMode = ShadowCastingMode.Off;

        // Set up width curve
        AnimationCurve widthCurve = new AnimationCurve();
        widthCurve.AddKey(1.0f, 0.3f); // Width at the start
        widthCurve.AddKey(0.0f, 0.01f); // Width at the end with exponential drop

        lineRenderer.widthCurve = widthCurve;

        SetEntitySnapshots(snapshots);
        SetPathVisibility(true);
        ViewPath();
        return lineRenderer;
    }

    public void SetPathVisibility(bool visibility)
    {
        _isPathVisible = visibility;
    }

    public void SetEntitySnapshots(LinkedList<TimeSnapshot> targetedSnapshots)
    {
        snapshots = targetedSnapshots;
        ViewPath();
    }

}
