using UnityEngine;
using System.Collections.Generic;

public class TurretController : MonoBehaviour
{
    public Transform targetContainer; // Assign this in the Unity Inspector
    private List<Transform> targets = new List<Transform>();
    public float rotationSpeed = 1f;
    public int currentTargetIndex = 0;
    public Transform aimer;
    public float allowedError = 0.5f;
    public float waitingTime = 2f; // Time in seconds to wait before switching targets
    private float waitingTimer = 0f;  // Make sure the timer starts at 0
    private TimeEntity turretTimeEntity;


    void Start()
    {
        PopulateTargetList();
        turretTimeEntity = GetComponentInParent<TimeEntity>();
        if (turretTimeEntity != null)
        {
            Debug.Log("No time entity found for turret.");
        }

        // Look at initial target
        Transform target = targets[currentTargetIndex];
        aimer.rotation = Quaternion.LookRotation(target.position - aimer.position);
    }

    void Update()
    {
        if (turretTimeEntity.IsRewinding)
        {
            Debug.Log("Turret is rewinding.");
            return;
        }


        Transform target = targets[currentTargetIndex];
        RotateTowards(target);

        if (IsAimedAtTarget(target))
        {
            if (waitingTimer <= 0f)
            {
                waitingTimer = waitingTime; // Reset the timer
                currentTargetIndex = (currentTargetIndex + 1) % targets.Count; // Move to the next target
            }
            else
            {
                waitingTimer -= Time.deltaTime; // Count down the timer
            }
        }
    }

    void PopulateTargetList()
    {
        if (targetContainer == null) return;

        foreach (Transform child in targetContainer.GetComponentsInChildren<Transform>())
        {
            if (child != targetContainer)
            {
                targets.Add(child);
            }
        }
        if (targets.Count == 0) Debug.LogError("No targets found in turret controller.");
    }

    private void RotateTowards(Transform target)
    {
        Vector3 targetDirection = (target.position - aimer.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        aimer.rotation = Quaternion.RotateTowards(aimer.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private bool IsAimedAtTarget(Transform target)
    {
        float angle = Quaternion.Angle(aimer.rotation, Quaternion.LookRotation(target.position - aimer.position));
        return angle < allowedError;
    }

}
