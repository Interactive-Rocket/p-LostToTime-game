using UnityEngine;


[RequireComponent(typeof(InputManager))]
public class PlayerPause : MonoBehaviour
{
    private InputManager _input;

    void Awake()
    {
        _input = GetComponent<InputManager>();
    }

    private void Update()
    {
        if (_input.IsPausing())
        {
            bool isPaused;

            if (SceneManagerSingleton.Instance != null)
            {
                _input.PauseInput(false);

                if (SceneManagerSingleton.Instance.CurrentTimeState == SceneManagerSingleton.TimeState.Running)
                {
                    SceneManagerSingleton.Instance.PauseGame();
                    isPaused = true;
                }
                else
                {
                    SceneManagerSingleton.Instance.UnpauseGame();
                    isPaused = false;
                }
            }
            else return;

            if (UIPauseMenuManager.Instance != null)
            {
                UIPauseMenuManager.Instance.MenuVisibility(isPaused);
            }
        }
    }
}
