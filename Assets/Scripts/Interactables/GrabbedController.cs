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
    public float MagicGrabMoveSpeedNumber = 20f; //this REALLY should not be a per object (on component) variable lmaooo!!!! (if we want it tweakable in editor)
    private float cameraHeight = 1.375f;
    private TimeEntity tent;

    void Start() {
        tent = GetComponent<TimeEntity>();
    }

    void WoweeMeGotGrabd()
    {
        if (_input == null || grabOffset == null || rb == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            _input = player.GetComponent<InputManager>();
            grabOffset = player.GetComponent<PlayerInteract>().InteractionRange;
            rb = GetComponent<Rigidbody>();
        }

        Vector3 bounds = GetComponent<Renderer>().bounds.extents;
        size = Mathf.Max(bounds.x, Mathf.Max(bounds.y, bounds.z)); //we wanna be able to grab big objects without them going inside us (UWU)
        grabbed = true;
        stillHasNotReleasedThaMofoButton = true;
        initialized = true;
    }

    void MeFreee()
    {
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
        toPos.y = Mathf.Max(toPos.y, Camera.main.transform.position.y - cameraHeight + size);
        Vector3 vecToPos = toPos - rb.position;
        rb.velocity = vecToPos * 15;
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
                if (tent.IsRewinding) {
                    MeFreee();
                }
                ApplyGrab();
            }
        }
    }
}