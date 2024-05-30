using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDiegeticMenuManager : UIMenuManager
{
    public static UIDiegeticMenuManager Instance { get; private set; }
    [Header("The firts level to start a new game")]
    [Tooltip("Path to all levels must be specified in build settings as well.")]
    [SerializeField] private string m_firstLevel;
    [Header("Other levels to Load")]
    [SerializeField] private string m_secondLevel;
    [SerializeField] private string m_thirdLevel;
    [SerializeField] private string m_fourthLevel;
    private string[] m_levels;

    [Header("Canvases")]
    [SerializeField] private GameObject NevigationCanvas;
    [SerializeField] private GameObject MainCanvas;
    [SerializeField] private GameObject InstructionsCanvas;
    [SerializeField] private GameObject CreditsCanvas;
    [SerializeField] private GameObject LoadLevelCanvas;

    override protected void SetMenuStates()
    {
        m_levels = new string[]
        {
            m_firstLevel,
            m_secondLevel,
            m_thirdLevel,
            m_fourthLevel
        }; //Not the best approach, but will do

        menuStates = new List<GameObject>
        {
            MainCanvas,
            InstructionsCanvas,
            CreditsCanvas,
            LoadLevelCanvas
        };
    }

    protected override void AssignSelfSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    override public void SetMainCanvasActive()
    {
        ChangeMenuState(MainCanvas);
    }

    public void SetInstructionsCanvasActive()
    {
        ChangeMenuState(InstructionsCanvas);
    }

    public void SetCreditsCanvasActive()
    {
        ChangeMenuState(CreditsCanvas);
    }
    public void SetLoadLevelCanvasActive()
    {
        ChangeMenuState(LoadLevelCanvas);
    }
    public void StartGame()
    {
        if (SceneManagerSingleton.Instance != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            SceneManagerSingleton.Instance.LoadScene(m_firstLevel);
        }
    }
    public void StartGameAtLevel(int levelIndex)
    {
        if (levelIndex >= m_levels.Length || levelIndex < 0)
        {
            Debug.LogError("Level " + levelIndex + " does not exit, to load a level use values between 0 to " + (m_levels.Length - 1) + " only.");
        }
        else
        {
            if (SceneManagerSingleton.Instance != null)
            {
                Cursor.lockState = CursorLockMode.Locked;
                SceneManagerSingleton.Instance.LoadScene(m_levels[levelIndex]);
            }
        }

    }

    public void KillGame()
    {
#if UNITY_STANDALONE
            Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
