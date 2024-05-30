using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; set; }
    [Header("The next scene")]
    [Tooltip("String name of the scene to load. Can be overriden by other load triggers.")]
    [SerializeField] private string m_sceneToLoad;
    [Header("Transition animations")]
    [Tooltip("The overlaying screen")]
    [SerializeField] private Canvas m_fadingCanvas;

    [Tooltip("The StateMachine of the FadeIn and Out animations")]
    [SerializeField] private Animator m_transitionsAnimator;
    [Tooltip("The Fade In animation")]
    [SerializeField] private AnimationClip m_fadeIn;
    [Tooltip("The Fade Out animation")]
    [SerializeField] private AnimationClip m_fadeOut;
    private SceneManagerSingleton m_sceneManagerSingleton;
    
    public float CurrentAnimatorStateTime => m_transitionsAnimator ? m_transitionsAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime : 0f;

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
    
    void Start()
    {
        if (SceneManagerSingleton.Instance != null) return;
        Debug.LogError("The SceneTransitionManager needs a SceneManager to work, add it to the scene");
    }

    public void FadeIn()
    {
        //Debug.Log(m_canvas);
        if (m_fadingCanvas == null) Debug.LogError("The Fading Canvas is missing");
        m_fadingCanvas.gameObject.SetActive(true);
        if (m_transitionsAnimator) m_transitionsAnimator.Play(m_fadeIn.name, 0, 0.0f);
    }

    // Fades out and loads the next scene
    public void FadeOut()
    {
        if (m_transitionsAnimator) m_transitionsAnimator.Play(m_fadeOut.name, 0, 0.0f);
        //float t = m_transitionsAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    // Fades out and loads the next scene, overloaded method to call a specific scene
    public void FadeOut(string sceneName)
    {
        m_sceneToLoad = sceneName;
        FadeOut();
    }

    // Directly loads the next scene
    public void NextScene()
    {
        if (SceneManagerSingleton.Instance != null) SceneManagerSingleton.Instance.LoadScene(m_sceneToLoad);
    }
}
