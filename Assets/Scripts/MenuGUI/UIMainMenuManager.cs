using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenuManager : MonoBehaviour
{
    public static UIMainMenuManager Instance { get; private set; }
    [Tooltip("Path to the first level, must be specified in build settings as well.")]
    [SerializeField] private string firstLevel;
    [Header("Canvases")]
    [SerializeField] private GameObject MainCanvas;
    [SerializeField] private GameObject InstructionsCanvas;
    [SerializeField] private GameObject CreditsCanvas;

    private GameObject currentMenuState;
    private List<GameObject> menuStates;

    void Awake()
    {
        // Ensure it's a singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        menuStates = new List<GameObject>
        {
            MainCanvas,
            InstructionsCanvas,
            CreditsCanvas
        };

        SetMainCanvasActive();
    }

    public void SetMainCanvasActive()
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

    private void ChangeMenuState(GameObject newState)
    {
        if (newState == null) return;
        currentMenuState = newState;

        foreach (var state in menuStates)
        {
            if (state != null) state.SetActive(state == newState);
        }
    }

    public void StartGame()
    {
        if (SceneManagerSingleton.Instance != null) {
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
