using UnityEngine;

/* Data for logging attributes at a given time step
* Example of an extended class for more attributes
*/
public class TimeSnapshot
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }

    public TimeSnapshot(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}

public class TimeSnapshotSound : TimeSnapshot
{
    public AudioClip audioClip { get; set; } 
    public float playbackTime { get; set; } 

    public TimeSnapshotSound(Vector3 position, Quaternion rotation, AudioClip audioClip, float playbackTime) : base(position, rotation)
    {
        this.audioClip = audioClip;
        this.playbackTime = playbackTime;
    }
}

public class TimeSnapshotRB : TimeSnapshot
{
    
    public Vector3 Velocity { get; set; }
    public Vector3 AngularVelocity { get; set; }

    public TimeSnapshotRB(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity) : base(position, rotation)
    {
        Velocity = velocity;
        AngularVelocity = angularVelocity;
    }
}

public class TimeSnapshotRBLocalTime : TimeSnapshot
{
    public Vector3 Velocity { get; set; }
    public Vector3 AngularVelocity { get; set; }
    public float LocalTime { get; set; }

    public TimeSnapshotRBLocalTime(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity, float localTime) : base(position, rotation)
    {
        Velocity = velocity;
        AngularVelocity = angularVelocity;
        LocalTime = localTime;
    }
}


public class TimeSnapshotButton : TimeSnapshotSound
{
    public bool IsPushed { get; set; }
    public float Cooldown { get; set; }
    public float CooldownTime { get; set; }

    public TimeSnapshotButton(Vector3 position, Quaternion rotation, bool isPushed, float cooldown, float cooldownTime, AudioClip audioClip, float playbackTime) : base(position, rotation,audioClip,playbackTime)
    {
        IsPushed = isPushed;
        Cooldown = cooldown;
        CooldownTime = cooldownTime;
    }
}


public class TimeSnapshotDoor : TimeSnapshotSound
{
    public float animationProgress { get; set; }

    public TimeSnapshotDoor(Vector3 position, Quaternion rotation, float animationProgress, AudioClip audioClip, float playbackTime) : base(position, rotation,audioClip,playbackTime)
    {
       this.animationProgress = animationProgress;
    }
}


public class TimeSnapshotConveyor : TimeSnapshotSound
{
    public float pushForce { get; set; }

    public TimeSnapshotConveyor(Vector3 position, Quaternion rotation, float pushForce, AudioClip audioClip, float playbackTime) : base(position, rotation,audioClip,playbackTime)
    {
       this.pushForce = pushForce;
    }
}