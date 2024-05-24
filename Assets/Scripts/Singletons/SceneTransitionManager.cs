using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; set; }
    [Header("The next scene")]
    [Tooltip("String name of the scene to load.")]
    [SerializeField] private string m_sceneToLoad;
    [SerializeField] private SceneManagerSingleton m_sceneManagerSingleton;
    [Header("Transition animations")]
    [Tooltip("The overlaying screen")]
    [SerializeField] private Canvas m_fadingCanvas;

    [Tooltip("The StateMachine of the FadeIn and Out animations")]
    [SerializeField] private Animator m_transitionsAnimator;
    [Tooltip("The Fade In animation")]
    [SerializeField] private AnimationClip m_fadeIn;
    [Tooltip("The Fade Out animation")]
    [SerializeField] private AnimationClip m_fadOut;
    
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
        //CheckForSceneManager();
        //Makes sure the fade-in is the first state once the scene starts
        FadeIn();
    }

    /* This checking does not work, it appears that sometime the SceneManagerSingleton is instanciated after this class so trows an error
    private void CheckForSceneManager()
    {
        if (SceneManagerSingleton.Instance != null) return;
        Debug.LogError("The SceneTransitionManager needs a SceneManager to work, add it to the scene");
    }*/
    public void FadeIn()
    {
        //Debug.Log(m_canvas);
        if (m_fadingCanvas == null) Debug.LogError("The Fading Canvas is missing");
        m_fadingCanvas.gameObject.SetActive(true);
        if (m_transitionsAnimator) m_transitionsAnimator.Play(m_fadeIn.name, 0, 0.0f);
    }

    public void FadeOut()
    {
        if (m_transitionsAnimator) m_transitionsAnimator.Play(m_fadOut.name, 0, 0.0f);
        //float t = m_transitionsAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void NextScene()
    {
        if (SceneManagerSingleton.Instance != null) SceneManagerSingleton.Instance.LoadScene(m_sceneToLoad);
    }
    public void NextScene(string sceneName)
    {
        if (SceneManagerSingleton.Instance != null) SceneManagerSingleton.Instance.LoadScene(sceneName);
    }
}
