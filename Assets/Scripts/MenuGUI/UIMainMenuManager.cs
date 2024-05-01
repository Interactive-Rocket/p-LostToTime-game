using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenuManager : UIMenuManager
{ 
    public static UIMainMenuManager Instance { get; private set; }
    [Tooltip("Path to the first level, must be specified in build settings as well.")]
    [SerializeField] private string firstLevel;
    [Header("Canvases")]
    [SerializeField] private GameObject MainCanvas;
    [SerializeField] private GameObject InstructionsCanvas;
    [SerializeField] private GameObject CreditsCanvas;

    override protected void SetMenuStates()
    {
        menuStates = new List<GameObject>
        {
            MainCanvas,
            InstructionsCanvas,
            CreditsCanvas
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

    public void StartGame()
    {
        if (SceneManagerSingleton.Instance != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            SceneManagerSingleton.Instance.LoadScene(firstLevel);
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
