using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AnimationClip))]
public class LiftingController : MonoBehaviour
{
    private Animator m_animator;
    [Tooltip("The Animation Clip the controller is intrested in")]
    [SerializeField] private AnimationClip m_liftAnimationClip;

    void Awake()
    {
        m_animator = GetComponent<Animator>();
    }
    public void Lift()
    {
        if (m_animator == null) return;
        if (m_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == m_liftAnimationClip.name) return;
        m_animator.Play(m_liftAnimationClip.name, 0, 0.0f);
        m_animator.speed = 1;
    }
}
