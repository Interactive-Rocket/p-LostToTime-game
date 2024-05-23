using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AnimationClip))]
public class ElevatorDoorController : MonoBehaviour
{
    [SerializeField] private Animator m_animator;
    [SerializeField] private AnimationClip m_openingAnimationClip;

    public void OpenDoors()
    {
        //Checks if the current state is the same as the openingAnimation
        if (m_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == m_openingAnimationClip.name) return;
        m_animator.Play(m_openingAnimationClip.name, 0, 0.0f);
        m_animator.speed = 1;
    }
}
