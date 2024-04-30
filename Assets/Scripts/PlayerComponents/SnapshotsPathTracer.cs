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

    public Material lineMaterial;
    public int lengthOfLineRenderer = 20;
    private bool _isPathVisible = false;

    LineRenderer lineRenderer;

    void Start()
    {
        setUpLineRenderer(lineRenderer);
    }
    void Update()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;

        ViewPath(lineRenderer);
        // PreviewPath(lineRenderer);

    }

    public void ChangeColor(Color newColor) {
        lineMaterial.SetColor("_Color", newColor);
    }

    private void setUpLineRenderer(LineRenderer lineRenderer)
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.2f;
    }

    public void ViewPath(LineRenderer lineRenderer)
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
    }
}