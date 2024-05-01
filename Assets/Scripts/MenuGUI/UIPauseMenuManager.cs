using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPauseMenuManager : UIMenuManager
{
    public static UIPauseMenuManager Instance { get; private set; }
    [Tooltip("Path to the main menu, must be specified in build settings as well.")]
    [SerializeField] private string mainMenuScene;
    [Header("Canvases")]
    [Tooltip("Contains all other canvases, so we can easily hide the pause menu.")]
    [SerializeField] private GameObject ContainerCanvas;
    [SerializeField] private GameObject MainCanvas;
    [SerializeField] private GameObject InstructionsCanvas;
    private bool isVisible = false;

    void Start()
    {
        if (ContainerCanvas != null) MenuVisibility(false);
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

    override protected void SetMenuStates()
    {
        menuStates = new List<GameObject>
        {
            MainCanvas,
            InstructionsCanvas,
        };
    }

    override public void SetMainCanvasActive()
    {
        ChangeMenuState(MainCanvas);
    }

    public void SetInstructionsCanvasActive()
    {
        ChangeMenuState(InstructionsCanvas);
    }

    public void GoToMainMenu()
    {
        if (SceneManagerSingleton.Instance != null)
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManagerSingleton.Instance.LoadScene(mainMenuScene);
        }
    }

    public void UnpauseGame()
    {
        if (SceneManagerSingleton.Instance != null)
        {
            SetMainCanvasActive();
            SceneManagerSingleton.Instance.UnpauseGame();
            MenuVisibility(false);
        }
    }

    // Toggles the menu on or off
    public void MenuVisibility()
    {
        MenuVisibility(!isVisible);
    }

    public void MenuVisibility(bool visible)
    {
        isVisible = visible;
        ContainerCanvas.SetActive(visible);
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
