using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ITargetable {}


public class TimeEntity : MonoBehaviour, ITargetable
{
    public event Action<bool> OnRewindStatusChanged;
    [Tooltip("How long we record time for, in seconds")]
    public float maxTime = 60f;
    public TimePattern currentTimePattern = TimePattern.Default;
    public bool _isStopped = false;
    public float _timeScale = 1.0f;
    private LinkedList<TimeSnapshot> snapshots;

    public event Action<bool> OnRewindingChanged;

    private bool _isRewinding;
    public bool IsRewinding
    {
        get => _isRewinding;
            set
            {
                Debug.Log($"Attempting to change IsRewinding from {_isRewinding} to {value}");
                if (_isRewinding != value)
                {
                    _isRewinding = value;
                    Debug.Log($"IsRewinding changed: {_isRewinding}");
                }
            }

    }

    /* Different types of patterns for time keeping
    * Default is for rigidbody physics objects, only keeping rotation, position, and velocity
    * DefaultLocalTime is the same but keeps track of object local time
    */
    public enum TimePattern
    {
        Default,
        DefaultRB,
        DefaultRBLocalTime,
        DefaultButton,
        DefaultDoor,
        DefaultConveyor
    }

    // Keep track of different attributes from the various time snapshot structures:
    public float localTime;
    private Rigidbody rb;
    private ButtonController buttonController;
    private DoorController doorController;
    private SoundController soundController;
    private ConveyorController conveyorController;

    void Start()
    {
        snapshots = new LinkedList<TimeSnapshot>();
        localTime = Time.time;

        switch (currentTimePattern)
        {
            case TimePattern.DefaultRB:
                rb = GetComponent<Rigidbody>();
                break;
            case TimePattern.DefaultRBLocalTime:
                rb = GetComponent<Rigidbody>();
                break;
            case TimePattern.DefaultButton:
                buttonController = GetComponent<ButtonController>();
                soundController = GetComponent<SoundController>();
                break;
            case TimePattern.DefaultDoor:
                doorController = GetComponent<DoorController>();
                soundController = GetComponent<SoundController>();
                break;
            case TimePattern.DefaultConveyor:
                conveyorController = GetComponent<ConveyorController>();
                soundController = GetComponent<SoundController>();
                break;
            default:
                break;
        }
    }
    void Update()
    {
        // _isStopped = _timeScale == 0.0f;
        _timeScale = _isRewinding ? -1.0f : 1.0f;
        // _isRewinding = _isStopped ? false : _timeScale == -1.0f;

        // Update local time, if time is stopped or rewinding do nothing
        localTime = !_isStopped && !_isRewinding ? localTime + Time.deltaTime * _timeScale : localTime;

        // When rewinding or stopped time we may want to make sure the object is kinematic (forces won't affect it)
        // rb.isKinematic = _isStopped || _isRewinding;
    }

    void FixedUpdate()
    {
        // If the LinkedList is too big, we remove the entry at the start
        if (snapshots.Count > Mathf.Round(maxTime / Time.fixedDeltaTime))
        {
            snapshots.RemoveFirst();
        }

        if (_isRewinding)
        {
            if (snapshots.Count > 0)
            {
                TimeSnapshot lastSnapshot = snapshots.Last.Value;
                snapshots.RemoveLast();
                ReadSnapshot(lastSnapshot);
            }
            else
            {
                // List is empty, can't rewind
                Debug.Log("No more positions to rewind.");
                this.Rewind(false);
            }
        }
        else if (!_isStopped) // Only record positions if the game is not stopped
        {
            CreateSnapshot();
        }
    }

    // In case we want to locally rewind
    public bool Rewind(bool rewind)
    {
        Debug.Log("Rewinding object from TargetedTimeEntity");
        IsRewinding = snapshots.Count > 0 ? rewind : false;
        return IsRewinding;
    }

    // In case we want to locally stop time
    public void Freeze()
    {
        _isStopped = true;
    }

    void ReadSnapshot(TimeSnapshot lastSnapshot)
    {
        transform.position = lastSnapshot.Position;
        transform.rotation = lastSnapshot.Rotation;

        switch (lastSnapshot)
        {
            case TimeSnapshotRBLocalTime timeSnapshotLocalTime:
                localTime = timeSnapshotLocalTime.LocalTime;
                if (!rb.isKinematic)
                {
                    rb.velocity = timeSnapshotLocalTime.Velocity;
                    rb.angularVelocity = timeSnapshotLocalTime.AngularVelocity;
                }
                break;
            case TimeSnapshotRB timeSnapshotRB:
                if (!rb.isKinematic)
                {
                    rb.velocity = timeSnapshotRB.Velocity;
                    rb.angularVelocity = timeSnapshotRB.AngularVelocity;
                }
                break;
            case TimeSnapshotButton timeSnapshotButton:
                buttonController.isButtonPushed = timeSnapshotButton.IsPushed;
                buttonController.cooldown = timeSnapshotButton.Cooldown;
                buttonController.cooldownTime = timeSnapshotButton.CooldownTime;
                soundController.playbackTime = timeSnapshotButton.playbackTime;
                soundController.audioClip = timeSnapshotButton.audioClip;
                break;
            case TimeSnapshotDoor timeSnapshotDoor:
                doorController.animationProgress = timeSnapshotDoor.animationProgress;
                soundController.playbackTime = timeSnapshotDoor.playbackTime;
                soundController.audioClip = timeSnapshotDoor.audioClip;
                break;
            case TimeSnapshotConveyor timeSnapshotConveyor:
                conveyorController.pushForce = timeSnapshotConveyor.pushForce;
                soundController.playbackTime = timeSnapshotConveyor.playbackTime;
                soundController.audioClip = timeSnapshotConveyor.audioClip;
                break;
            default:
                break;
        }
    }

    void CreateSnapshot()
    {
        TimeSnapshot currentSnapshot;
        switch (currentTimePattern)
        {
            case TimePattern.DefaultRB:
                currentSnapshot = new TimeSnapshotRB(transform.position, transform.rotation, rb.velocity, rb.angularVelocity);
                break;
            case TimePattern.DefaultRBLocalTime:
                currentSnapshot = new TimeSnapshotRBLocalTime(transform.position, transform.rotation, rb.velocity, rb.angularVelocity, localTime);
                break;
            case TimePattern.DefaultButton:
                currentSnapshot = new TimeSnapshotButton(transform.position, transform.rotation, buttonController.isButtonPushed, buttonController.cooldown, buttonController.cooldownTime, soundController.audioClip, soundController.playbackTime);
                break;
            case TimePattern.DefaultDoor:
                currentSnapshot = new TimeSnapshotDoor(transform.position, transform.rotation, doorController.animationProgress, soundController.audioClip, soundController.playbackTime);
                break;
            case TimePattern.DefaultConveyor:
                currentSnapshot = new TimeSnapshotConveyor(transform.position, transform.rotation,conveyorController.pushForce, soundController.audioClip, soundController.playbackTime);
                break;
            default:
                currentSnapshot = new TimeSnapshot(transform.position, transform.rotation);
                break;
        }
        snapshots.AddLast(currentSnapshot);
    }
    
    //Get the snapshots to draw the line
    public LinkedList<TimeSnapshot> GetSnapshots()
    {
        return snapshots;
    }

}
