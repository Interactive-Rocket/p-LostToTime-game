using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AnimationClip))]
public class ElevatorDoorController : MonoBehaviour
{
    [SerializeField] private Animator m_animator;
    [SerializeField] private AnimationClip m_openingAnimationClip;
    [SerializeField] private AnimationClip m_clossingAnimationClip;
    void Awake()
    {
        //if(m_animator) m_animator.StopPlayback();
        if (m_animator) m_animator.Play("OpeningElevatorDoorsAnimation", 0, 0.0f); //Tell the Animator Interface WHAT keyframes to animate
        m_animator.speed = 0;
        //if(m_animator) m_animator.Play("ClossingElevatorDoorsAnimation", 0, 0.0f); //Tell the Animator Interface WHAT keyframes to animate
        //m_animator.speed = 0;
    }

    private void Update()
    {
        //Debug.Log("m_animationClip");
        //Debug.Log(m_animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
    }

    public void OpenDoors()
    {
        //Debug.Log("Open Elevator doors");
        //m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime = 0;
        m_animator.Play("OpeningElevatorDoorsAnimation", 0, 0.0f);
        m_animator.speed = 1;//m_animator.speed == 1 ? -1 : 1;
        
        //m_animator.speed = 0;
    }
    public void CloseDoors()
    {
        Debug.Log("Close Elevator doors");
        m_animator.speed = 1;
    }
}
