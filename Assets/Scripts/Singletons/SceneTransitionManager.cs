using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; set; }

    [Tooltip("The StateMachine of the FadeIn and Out animations")]
    [SerializeField] private Animator m_transitionsAnimator;
    [Tooltip("The Fade In animation")]
    [SerializeField] private AnimationClip m_fadeIn;
    [Tooltip("The Fade Out animation")]
    [SerializeField] private AnimationClip m_fadOut;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
        //Makes sure the fade-in is the first state once the scene starts
        FadeIn();
    }

    public void FadeIn()
    {
        if (m_transitionsAnimator) m_transitionsAnimator.Play(m_fadeIn.name, 0, 0.0f);
    }

    public void FadeOut()
    {
        if (m_transitionsAnimator) m_transitionsAnimator.Play(m_fadOut.name, 0, 0.0f);
    }

    private void Update() 
    {
        //if(m_transitionsAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == m_fadOut.name) Debug.Log(m_transitionsAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);    
    }
}
