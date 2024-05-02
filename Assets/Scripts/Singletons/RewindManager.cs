using System.Collections.Generic;
using UnityEngine;

public class RewindManager : MonoBehaviour {
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
    private bool isRewinding;

    private enum EntityState {
        Focused,
        Selected,
        Rewinding
    }

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        RewindObjects(false);
    }

    void Update() {
        UpdateSelectedObjectPaths();
        RaycastAndHandleFocus();
    }

    private void RaycastAndHandleFocus()
    {
        Vector3 screenCenterPoint = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit hit)) {}
            GameObject hitObject = hit.collider.gameObject;
            //Debug.Log(hitObject.name);
            TimeEntity timeEntity = hitObject.GetComponentInParent<TimeEntity>();
            if(timeEntity != null) {
                // SetupFocus(hitObject);
                Debug.Log("Focusing on " + hitObject.name);
                // SetupFocus(hitObject);
            }
    }

    public void SelectObject(GameObject obj) {
        if (!selectedObjects.Contains(obj)) {
            selectedObjects.Add(obj);
            Debug.Log("Selected " + obj.name);
            Debug.Log("All selected objects: " + selectedObjects.Count);
            UpdateVisuals(obj, EntityState.Selected);
        } else {
            // DeselectObject(obj);
            // Debug.Log("Deselected " + obj.name);
        }
    }

    public void DeselectObject(GameObject obj) {
        if (selectedObjects.Contains(obj)) {
            selectedObjects.Remove(obj);
        }
    }

    public void ClearSelection() {
        selectedObjects.Clear();
    }

    public void RewindObjects(bool rewind) {
        isRewinding = rewind;
        foreach (GameObject obj in selectedObjects) {
            TimeEntity timeEntity = obj.GetComponentInParent<TimeEntity>();
            if (timeEntity != null) {
               if (timeEntity.Rewind(rewind)) {
                    UpdateVisuals(obj, EntityState.Rewinding);
                } else {
                    UpdateVisuals(obj, EntityState.Selected);
                }
            }
        }
    }

    public void UpdateSelectedObjectPaths() {
        foreach (GameObject obj in selectedObjects) {
            SnapshotsPathTracer path = obj.GetComponent<SnapshotsPathTracer>();
            TimeEntity timeEntity = obj.GetComponentInParent<TimeEntity>();
            if (path != null && timeEntity != null) {
                Debug.Log("Updating path for " + obj.name);
                path.SetEntitySnapshots(timeEntity.GetSnapshots());
            }
        }
    }

    private void UpdateVisuals(GameObject obj, EntityState state) {
        UpdateParticleSystem(obj, state);
        UpdatePath(obj, state);
    }

    private void UpdatePath(GameObject obj, EntityState state) {
        SnapshotsPathTracer path = obj.GetComponent<SnapshotsPathTracer>();
        TimeEntity timeEntity = obj.GetComponentInParent<TimeEntity>();
        Debug.Log("Updating path for " + obj.name + ", path is null: " + (path == null) + ", time entity is null: " + (timeEntity == null));
        if (path == null && timeEntity != null) {
            Debug.Log("Adding path tracer to " + obj.name);
            path = obj.AddComponent<SnapshotsPathTracer>();
            path.SetUpLineRenderer(timeEntity.GetSnapshots());
        }

        UpdatePathColor(obj, path, state);
    }

    private void UpdatePathColor(GameObject obj, SnapshotsPathTracer path, EntityState state) {
        Debug.Log("asdfasdf path: " + path + ", state: " + state);
        if(path != null) {
                switch (state) {
                    case EntityState.Focused:
                        path.ChangeMaterial(pathMaterialFocused);
                        break;
                    case EntityState.Selected:
                        Debug.Log("updating and setting path color to selected");
                        path.ChangeMaterial(pathMaterialSelected);
                        break;
                    case EntityState.Rewinding:
                        Debug.Log("updating and setting path color to rewinding");
                        path.ChangeMaterial(pathMaterialRewinding);
                        break;
                    default:
                        break;
                }
            } else {
                Debug.Log("Path failed to instantiate, obj: " + obj.name);
            }
    }
        

    private void UpdateParticleSystem(GameObject obj, EntityState state) {
        ParticleSystem ps = obj.GetComponentInChildren<ParticleSystem>();
        Debug.Log("Updating particle system color for " + obj.name + " with state " + state + " and ps: " + ps);

        if (ps == null) {
            GameObject particleSystemInstance = Instantiate(particleSystemPrefab, obj.transform);
            // particleSystemInstance.transform.SetParent(obj.transform);

            ps = particleSystemInstance.GetComponent<ParticleSystem>(); 
            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.MeshRenderer;
            shape.meshRenderer = obj.GetComponent<MeshRenderer>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // must pass ps because else it will be null because it is not yet instantiated
        UpdateParticleSystemColor(obj, ps, state);
         if (!ps.isPlaying) {
            ps.Play();
        }
    }

    private void UpdateParticleSystemColor(GameObject obj, ParticleSystem ps, EntityState state) {
        Debug.Log("Updating particle system color for " + obj.name + " with state " + state + " and ps: " + ps);
        if (ps != null) {
            ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
            if (renderer != null) {
                switch (state) {
                    case EntityState.Focused:
                        renderer.material = particleMaterialFocused;
                        break;
                    case EntityState.Selected:
                        Debug.Log("updating and setting particle color to selected");    
                        renderer.material = particleMaterialSelected;
                        break;
                    case EntityState.Rewinding:
                        renderer.material = particleMaterialRewinding;
                        break;
                    default:
                        break;
                }
            } else {
                Debug.Log("No renderer found for " + obj.name);
            }
        } else {
            Debug.Log("No particle system found for " + obj.name);
        }
    }
}
