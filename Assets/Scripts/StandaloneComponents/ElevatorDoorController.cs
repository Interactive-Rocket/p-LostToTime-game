using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AnimationClip))]
public class ElevatorDoorController : MonoBehaviour
{
    private Animator m_animator;
    [Tooltip("The Animation Clip the controller is intrested in")]
    [SerializeField] private AnimationClip m_openingAnimationClip;

    void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void OpenDoors()
    {
        //Checks if the current state is the same as the openingAnimation
        if (m_animator == null) return;
        if (m_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == m_openingAnimationClip.name) return;
        m_animator.Play(m_openingAnimationClip.name, 0, 0.0f);
        m_animator.speed = 1;
    }
    public void Lift()
    {
        //TODO play Lift animation

    }
}
