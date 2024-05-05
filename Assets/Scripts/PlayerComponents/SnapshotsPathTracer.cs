using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    public void RemoveSelf() {
        Destroy(lineRenderer);
        Destroy(this);
    }

    public void ChangeMaterial(Material newMaterial) {
        Debug.Log("asdf Attempting to change material to " + newMaterial.name);
        if (lineRenderer != null) {
            lineRenderer.material = newMaterial;
            Debug.Log("asdf Material changed successfully to " + newMaterial.name);
        } else {
            Debug.Log("asdf lineRenderer is not initialized");
        }
    }


    public LineRenderer SetUpLineRenderer(LinkedList<TimeSnapshot> snapshots)
    {
        Debug.Log("Setting up line renderer with snapshots: " + snapshots.Count);
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.widthMultiplier = 0.2f;
        SetEntitySnapshots(snapshots);
        SetPathVisibility(true);
        ViewPath();
        return lineRenderer;
    }

    public void ViewPath()
    {

        if (snapshots != null && _isPathVisible)
        {
            lineRenderer.positionCount = snapshots.Count;

            var points = new Vector3[snapshots.Count];
            var t = Time.time;
            var i = 0;
            foreach (TimeSnapshot snapshot in snapshots)
            {
                points[i] = snapshot.Position;
                i++;
            }

            lineRenderer.SetPositions(points);
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
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