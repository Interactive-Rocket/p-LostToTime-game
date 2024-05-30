using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabManager : MonoBehaviour
{
    public float GrabRange = 10f;
    public float GrabStrength = 10f;
    public float GrabDamping = 0.5f;
    private Rigidbody grabbed = null;
    private float grabOffset;
    private bool isGrabbing = false; // Added to control grab toggle
    private int layerMask = ~(1 << 6);

    void Update()
    {
        print(LayerMask.NameToLayer("Invisible Wall"));
        // Lazy add, should be broken off into a separate method we can call
        Vector3 screenCenterPoint = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, GrabRange,layerMask))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out Rigidbody _))
            {
                if (HUDManager.Instance != null) HUDManager.Instance.HoveringGrabbable = true;
                return;
            }
        }

        if (HUDManager.Instance != null) HUDManager.Instance.HoveringGrabbable = false;
    }

    public void UpdateGrabStatus(bool isInputActive)
    {
        if (isInputActive && !isGrabbing)
        {
            FindGrab();
            isGrabbing = true;
        }
        else if (!isInputActive)
        {
            isGrabbing = false;
        }

        if (grabbed != null)
        {
            SendGrab();
        }
    }

    private void FindGrab()
    {
        Debug.Log("Grab input detected!");
        if (grabbed != null)
        {
            grabbed.useGravity = true;
            grabbed = null;
        }
        else
        {
            Vector3 screenCenterPoint = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, GrabRange,layerMask))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out Rigidbody rb))
                {
                    grabbed = rb;
                    grabbed.useGravity = false;
                    grabOffset = hitInfo.distance;
                }
            }
        }
    }

    private void SendGrab()
    {
        Vector3 vecToTargetPos = Camera.main.transform.position + Camera.main.transform.forward * grabOffset - grabbed.position;
        Vector3 dampingForce = -grabbed.velocity * GrabDamping;
        if (Vector3.Angle(grabbed.velocity, vecToTargetPos) > 90f)
        {
            grabbed.velocity = Vector3.zero;
        }
        grabbed.AddForce(vecToTargetPos * Mathf.Pow(vecToTargetPos.magnitude, 2) * GrabStrength + dampingForce);
    }
}

