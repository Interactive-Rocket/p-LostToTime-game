using System;
using System.Collections.Generic;
using UnityEngine;

public class RewindManager : MonoBehaviour
{
    public static RewindManager Instance { get; private set; }
    private List<GameObject> selectedObjects = new List<GameObject>();

    public Material particleMaterialFocused;
    public Material particleMaterialSelected;
    public Material particleMaterialRewinding;

    public Material pathMaterialFocused;
    public Material pathMaterialSelected;
    public Material pathMaterialRewinding;

    private GameObject currentFocusObject;
    public GameObject particleSystemPrefab;
    private int currentlyFocusedObj;
    private bool isRewinding;
    public AudioClip select;
    public AudioClip deselect;
    public AudioClip focusSound;
    public AudioClip rewindSound;
    public AudioClip rewindEndSound;

    private enum EntityState
    {
        Focused,
        Selected,
        Rewinding,
        Deselected
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        RewindObjects(false);
    }

    void Update()
    {
        UpdateSelectedObjectPaths();
        RaycastAndHandleFocus();
    }

    private void RaycastAndHandleFocus()
    {
        Vector3 screenCenterPoint = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            TimeEntity timeEntity = hitObject.GetComponentInParent<TimeEntity>();
            GameObject timeEntityObject = timeEntity != null ? timeEntity.gameObject : null;

            if (timeEntity != null)
            {
                if (timeEntityObject != currentFocusObject)
                {
                    if (currentFocusObject != null)
                    {
                        if (selectedObjects.Contains(currentFocusObject))
                        {
                            UpdateVisuals(currentFocusObject, EntityState.Selected);
                        }
                        else
                        {
                            ResetVisuals(currentFocusObject);
                        }
                    }
                    currentFocusObject = timeEntityObject;
                }

                // update to focused state only if not in selectedObjects
                if (!selectedObjects.Contains(timeEntityObject))
                {
                    UpdateVisuals(timeEntityObject, EntityState.Focused);
                }
                else
                {
                    UpdateVisuals(timeEntityObject, EntityState.Selected);
                }
            }
            else
            {
                if (currentFocusObject != null)
                {
                    if (selectedObjects.Contains(currentFocusObject))
                    {
                        UpdateVisuals(currentFocusObject, EntityState.Selected);
                    }
                    else
                    {
                        ResetVisuals(currentFocusObject);
                    }
                    currentFocusObject = null;
                }
            }
        }
        else
        {
            if (currentFocusObject != null)
            {
                if (selectedObjects.Contains(currentFocusObject))
                {
                    UpdateVisuals(currentFocusObject, EntityState.Selected);
                }
                else
                {
                    ResetVisuals(currentFocusObject);
                }
                currentFocusObject = null;
            }
        }
    }


    public void SelectObject(GameObject obj)
    {
        TimeEntity timeEntity = obj.GetComponentInParent<TimeEntity>();
        GameObject timeEntityObject = timeEntity != null ? timeEntity.gameObject : null;
        if (!selectedObjects.Contains(timeEntityObject))
        {
            selectedObjects.Add(timeEntityObject);
            UpdateVisuals(timeEntityObject, EntityState.Selected);
            AudioManager.Instance.PlayOneShot(select, 1f);
        }
    }

    public void DeselectAll()
    {
        if (selectedObjects.Count == 0)
        {
            return;
        }

        for (int i = selectedObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = selectedObjects[i];
            TimeEntity timeEntity = obj.GetComponentInParent<TimeEntity>();
            if (timeEntity != null)
            {
                timeEntity.Rewind(false);
                ResetVisuals(obj);
            }
            selectedObjects.RemoveAt(i);
        }
        AudioManager.Instance.PlayOneShot(deselect, 1f);
    }

    public void DeselectObject(GameObject obj)
    {
        TimeEntity timeEntity = obj.GetComponentInParent<TimeEntity>();
        GameObject timeEntityObject = timeEntity != null ? timeEntity.gameObject : null;
        if (selectedObjects.Contains(timeEntityObject))
        {
            selectedObjects.Remove(timeEntityObject);
            ResetVisuals(timeEntityObject);
        }
    }

    public void RewindObjects(bool rewind)
    {
        if (!isRewinding && rewind && selectedObjects.Count > 0)
        {
            AudioManager.Instance.PlayOneShot(rewindSound, 0.5f);
        }
        else if (isRewinding && !rewind && selectedObjects.Count > 0)
        {
            AudioManager.Instance.PlayOneShot(rewindEndSound, 1f);
        }

        isRewinding = rewind;

        for (int i = selectedObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = selectedObjects[i];
            if (obj == null)
            {
                continue;
            }
            TimeEntity timeEntity = obj.GetComponent<TimeEntity>();
            if (timeEntity != null)
            {
                if (timeEntity.Rewind(rewind))
                {
                    UpdateVisuals(obj, EntityState.Rewinding);
                }
                else
                {
                    if (timeEntity.HasSnapshots())
                    {
                        UpdateVisuals(obj, EntityState.Selected);
                    }
                    else
                    {
                        DeselectObject(obj);
                    }
                }
            }
        }
    }

    public void UpdateSelectedObjectPaths()
    {
        for (int i = selectedObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = selectedObjects[i];

            if (obj != null)
            {
                TimeEntity timeEntity = obj.GetComponent<TimeEntity>();
                SnapshotsPathTracer path = obj.GetComponent<SnapshotsPathTracer>();

                if (timeEntity != null && path != null)
                {
                    path.SetEntitySnapshots(timeEntity.GetSnapshots());
                }
            }
            else
            {
                selectedObjects.RemoveAt(i);
            }
        }
    }

    private void UpdateVisuals(GameObject obj, EntityState state)
    {
        TimeEntity timeEntity = obj.GetComponent<TimeEntity>();
        GameObject timeEntityObject;

        if (timeEntity == null)
        {
            timeEntity = obj.GetComponentInParent<TimeEntity>();
            timeEntityObject = timeEntity != null ? timeEntity.gameObject : null;
        }
        else
        {
            timeEntityObject = obj;
        }

        if (timeEntity != null)
        {
            UpdatePath(timeEntityObject, state);
        }

        MeshRenderer[] meshRenderers = timeEntityObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            GameObject meshRendererObj = meshRenderer.gameObject;
            UpdateParticleSystem(meshRendererObj, state);
        }
    }

    private void ResetVisuals(GameObject obj)
    {
        TimeEntity timeEntity = obj.GetComponentInParent<TimeEntity>();
        GameObject timeEntityObject = timeEntity != null ? timeEntity.gameObject : null;

        MeshRenderer[] meshRenderers = timeEntityObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            GameObject meshRendererObj = meshRenderer.gameObject;

            // Destroy all ParticleSystems attached to the object
            ParticleSystem[] particleSystems = meshRendererObj.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in particleSystems)
            {
                Destroy(ps.gameObject);
            }
        }

        // Reset the path for the object with TimeEntity
        if (timeEntity != null)
        {
            SnapshotsPathTracer path = timeEntityObject.GetComponent<SnapshotsPathTracer>();
            if (path != null)
            {
                path.RemoveSelf();
            }
        }
    }

    private void UpdatePath(GameObject obj, EntityState state)
    {
        TimeEntity timeEntity = obj.GetComponentInParent<TimeEntity>();
        GameObject timeEntityObject = timeEntity != null ? timeEntity.gameObject : null;
        SnapshotsPathTracer path = timeEntityObject.GetComponent<SnapshotsPathTracer>();

        if (path == null && timeEntity != null)
        {
            path = timeEntityObject.AddComponent<SnapshotsPathTracer>();
            path.SetUpLineRenderer(timeEntity.GetSnapshots());
        }
        else
        {
            path?.SetEntitySnapshots(timeEntity.GetSnapshots());
        }

        UpdatePathColor(timeEntityObject, path, state);
    }

    private void UpdatePathColor(GameObject obj, SnapshotsPathTracer path, EntityState state)
    {
        if (path != null)
        {
            switch (state)
            {
                case EntityState.Focused:
                    path.ChangeMaterial(pathMaterialFocused);
                    AudioManager.Instance.PlayOneShot(focusSound, 1f);
                    break;
                case EntityState.Selected:
                    path.ChangeMaterial(pathMaterialSelected);
                    break;
                case EntityState.Rewinding:
                    path.ChangeMaterial(pathMaterialRewinding);
                    break;
                default:
                    break;
            }
        }
    }

    private void UpdateParticleSystem(GameObject obj, EntityState state)
    {
        ParticleSystem ps = obj.GetComponentInChildren<ParticleSystem>();

        if (ps == null)
        {
            GameObject particleSystemInstance = Instantiate(particleSystemPrefab, obj.transform);

            ps = particleSystemInstance.GetComponent<ParticleSystem>();
            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.MeshRenderer;
            shape.meshRenderer = obj.GetComponent<MeshRenderer>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        UpdateAllParticleSystemsColor(obj, state);
        if (!ps.isPlaying)
        {
            ps.Play();
        }
    }

    private void UpdateAllParticleSystemsColor(GameObject obj, EntityState state)
    {
        ParticleSystem[] particleSystems = obj.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps != null)
            {
                ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
                if (renderer != null)
                {
                    switch (state)
                    {
                        case EntityState.Focused:
                            renderer.material = particleMaterialFocused;
                            AudioManager.Instance.PlayOneShot(focusSound, 1f);
                            break;
                        case EntityState.Selected:
                            renderer.material = particleMaterialSelected;
                            break;
                        case EntityState.Rewinding:
                            renderer.material = particleMaterialRewinding;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
