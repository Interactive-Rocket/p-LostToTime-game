using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GrabbedController : MonoBehaviour, IInteractable
{
    private PlayerInteract _interact;
    private bool grabbed = false;
    private float grabOffset;
    private Rigidbody rb;
    private float size;
    private bool initialized = false;
    private float MagicGrabMoveSpeedNumber = 20f;
    private float cameraHeight = 1.375f;
    private TimeEntity tent;
    private float oldAngularDrag = 0f;
    private float newAngularDrag = 100f;
    private bool releasedThisFrame = false;

    void Start()
    {
        tent = GetComponent<TimeEntity>();
    }

    void Grab()
    {
        if (!initialized)
        {
            GameObject player;

            if (PlayerManager.Instance != null)
            {
                player = PlayerManager.Instance.PlayerCapsuleGameObject;
                MagicGrabMoveSpeedNumber = PlayerManager.Instance.MagicGrabMoveSpeedNumber;
                newAngularDrag = PlayerManager.Instance.GrabbedObjectAngularDrag;
            }

            else player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            _interact = player.GetComponent<PlayerInteract>();
            if (_interact != null) grabOffset = _interact.InteractionRange;

            rb = GetComponent<Rigidbody>();
            Vector3 bounds = GetComponentInChildren<MeshRenderer>().bounds.extents;
            size = Mathf.Max(bounds.x, Mathf.Max(bounds.y, bounds.z)); //we wanna be able to grab big objects without them going inside us (UWU)
        }

        oldAngularDrag = rb.angularDrag;
        rb.angularDrag = newAngularDrag;

        grabbed = true;
        initialized = true;
        if (_interact != null) _interact.interactActions += Release;
    }

    void Release()
    {
        if (!rb.IsDestroyed())
            rb.angularDrag = oldAngularDrag;
        grabbed = false;
        releasedThisFrame = true;
        if (_interact != null) _interact.interactActions -= Release;
    }

    public void Interact()
    {
        if (!grabbed && !tent.IsRewinding && !releasedThisFrame)
        {
            Grab();
        }
    }

    void ApplyGrab()
    {
        Vector3 toPos = Camera.main.transform.position + Camera.main.transform.forward * (grabOffset + size);
        toPos.y = Mathf.Max(toPos.y, Camera.main.transform.position.y - cameraHeight + size); // grabbed obj cant go below us, stops propflying
        Vector3 vecToPos = toPos - rb.position;
        rb.velocity = vecToPos * MagicGrabMoveSpeedNumber;
        //rb.MovePosition(toPos);
    }

    void Update()
    {
        if (initialized && grabbed)
        {
            if (tent.IsRewinding)
            {
                Release();
                return;
            }
            ApplyGrab();
        }
    }

    void LateUpdate()
    {
        releasedThisFrame = false;
    }
}