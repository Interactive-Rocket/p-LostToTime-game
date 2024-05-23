using UnityEngine;
using UnityEngine.Events;

public class GrabbedController : MonoBehaviour, IInteractable
{
    private InputManager _input;
    private bool stillHasNotReleasedThaMofoButton;
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

    void Start()
    {
        tent = GetComponent<TimeEntity>();
    }

    void WoweeMeGotGrabd()
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

            _input = player.GetComponent<InputManager>();
            grabOffset = player.GetComponent<PlayerInteract>().InteractionRange;
            rb = GetComponent<Rigidbody>();
            Vector3 bounds = GetComponentInChildren<MeshRenderer>().bounds.extents;
            size = Mathf.Max(bounds.x, Mathf.Max(bounds.y, bounds.z)); //we wanna be able to grab big objects without them going inside us (UWU)
        }

        oldAngularDrag = rb.angularDrag;
        rb.angularDrag = newAngularDrag;

        grabbed = true;
        stillHasNotReleasedThaMofoButton = true;
        initialized = true;
    }

    void MeFreee()
    {
        rb.angularDrag = oldAngularDrag;
        grabbed = false;
    }

    public void Interact()
    {
        if (!stillHasNotReleasedThaMofoButton && !grabbed && !tent.IsRewinding)
        {
            WoweeMeGotGrabd();
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
        if (initialized)
        {
            if (stillHasNotReleasedThaMofoButton && !_input.IsInteracting())
            {
                stillHasNotReleasedThaMofoButton = false;
            }

            if (grabbed)
            {
                if (!stillHasNotReleasedThaMofoButton && _input.IsInteracting())
                {
                    stillHasNotReleasedThaMofoButton = true;
                    MeFreee(); //you are free now! Run, box, run!
                }
                if (tent.IsRewinding)
                {
                    MeFreee();
                }
                ApplyGrab();
            }
        }
    }
}