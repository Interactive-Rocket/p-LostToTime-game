using UnityEngine;

public class TargetableFocus : MonoBehaviour
{
    public ParticleSystem focusParticleSystemPrefab;
    private ParticleSystem activeParticleSystem;
    public Color focusColor;
    public Color rewindColor;
    public Material particleMaterial;
    private GameObject currentFocusObject;
    public bool timeVision;
    private SnapshotsPathTracer path;

    void Start()
    {
        path = GetComponent<SnapshotsPathTracer>();
    }
    void Update()
    {
        RaycastAndHandleFocus();
        
    }

    private void RaycastAndHandleFocus()
    {
        Vector3 screenCenterPoint = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 10f))
        {
            GameObject hitObject = hitInfo.collider.gameObject;
            if (hitObject.CompareTag("Targetable"))
            {
                if (currentFocusObject != hitObject)
                {
                    SetupFocus(hitObject);
                }
                // Ensure path visibility is always on while hovering over a targetable object
                Debug.Log("setting path visibility");
                path.SetPathVisibility(true);
            }
            else
            {
                ClearFocus();
            }
        }
        else
        {
            ClearFocus();
        }
    }

    private void SetupFocus(GameObject newFocusObject)
    {
        if (currentFocusObject != newFocusObject)
        {
            ClearFocus();
            currentFocusObject = newFocusObject;
            activeParticleSystem = Instantiate(focusParticleSystemPrefab, newFocusObject.transform);

            var shape = activeParticleSystem.shape;
            shape.shapeType = ParticleSystemShapeType.MeshRenderer;
            shape.meshRenderer = newFocusObject.GetComponent<MeshRenderer>();

            TimeEntity timeEntity = newFocusObject.GetComponentInParent<TimeEntity>();
            if (timeEntity != null)
            {
                Debug.Log("listening to events");
                timeEntity.OnRewindingChanged += UpdateParticleSystemColor;
                UpdateParticleSystemColor(timeEntity.IsRewinding); // Initial color setup
                
            }

            activeParticleSystem.Play();
        }
    }

    private void UpdateParticleSystemColor(bool isRewinding)
    {
        if (activeParticleSystem != null)
        {
            Debug.Log("updating particle color to " + isRewinding);
            particleMaterial.SetColor("_Color", isRewinding ? rewindColor : focusColor);
            path.ChangeColor(isRewinding ? rewindColor : focusColor);
        }
    }

    private void ClearFocus()
    {
        if (currentFocusObject != null)
        {
            TimeEntity timeEntity = currentFocusObject.GetComponentInParent<TimeEntity>();
            if (timeEntity != null && !timeEntity.IsRewinding)
            {
                timeEntity.OnRewindingChanged -= UpdateParticleSystemColor;
            if (activeParticleSystem != null)
            {
                Destroy(activeParticleSystem.gameObject);
                activeParticleSystem = null;
            }
                Debug.Log("removing path visibility");
                path.SetPathVisibility(false);
                currentFocusObject = null;
            }
        }
    }


    void OnDestroy()
    {
        ClearFocus(); // Ensure cleanup on destroy
    }
}
