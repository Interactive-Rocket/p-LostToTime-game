using UnityEngine;


[RequireComponent(typeof(InputManager))]
public class PlayerReset : MonoBehaviour
{
    private InputManager _input;

    void Awake()
    {
        _input = GetComponent<InputManager>();
    }

    private void Update()
    {
        if (_input.IsReloadingScene())
        {
            if (SceneManagerSingleton.Instance != null)
            {
                _input.ReloadSceneInput(false);
                SceneManagerSingleton.Instance.ReloadScene();
            }
        }
    }
}
