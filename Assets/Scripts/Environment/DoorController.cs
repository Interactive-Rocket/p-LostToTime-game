using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator animator;
    public AnimationClip doorAnimation;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;
    public bool isOpen;
    private bool oldState;
    public float animationProgress = 0f;
    public float animationSpeed = 0.5f;
    private TimeEntity doorTimeEntity;
    private SoundController soundController;
    public AudioClip openSound;
    public AudioClip closeSound;

    void OnEnable()
    {
        doorTimeEntity = GetComponentInParent<TimeEntity>();
        if (doorTimeEntity != null)
        {
            Debug.Log("No time entity found for door.");
        }
    }

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        soundController = GetComponent<SoundController>();
    }

    void Update()
    {
        float animationProgressTemp = animationProgress;
    
        if(!doorTimeEntity.IsRewinding) {
            if(isOpen) {
                animationProgress = Mathf.Clamp(animationProgress + Time.deltaTime * animationSpeed, 0, 1);
            }
            else {
                animationProgress = Mathf.Clamp(animationProgress - Time.deltaTime * animationSpeed, 0, 1);
                
            }
        }
        
        // Update the animation progress if animationProgress changes
        if(animationProgressTemp != animationProgress || doorTimeEntity.IsRewinding)
        {
            Debug.Log("Door animation progress: " + animationProgress);
            Debug.Log("Is rewinding: " + doorTimeEntity.IsRewinding);
            SetDoorAnimation(animationProgress);
        }
        
    }

    public void OpenDoor()
    {
        ChangeDoorState(true);
        soundController.Play(openSound);
    }

    public void CloseDoor()
    {
        ChangeDoorState(false);
        soundController.Play(closeSound);
    }

    public void ToggleDoor()
    {
        ChangeDoorState(!isOpen);
    }

    public void ChangeDoorState(bool newState)
    {
        isOpen = newState;
        oldState = !newState;
    }

    private void SetDoorAnimation(float animationProgress)
    {
        Debug.Log($"Setting door animation progress to: {animationProgress}");
        animator.Play("DoorOpen", 0, animationProgress);
        animator.speed = 0;
    }
}
