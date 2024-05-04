using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// I wish I could call it "SceneManager" but this is
// already taken by the built-in Unity SceneManager
public class SceneManagerSingleton : MonoBehaviour
{
    public static SceneManagerSingleton Instance { get; private set; }
    public enum TimeState { Running, Paused }
    public TimeState CurrentTimeState { get; private set; } = TimeState.Running;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadScene(bool asyncLoad = false)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (!asyncLoad) LoadScene(sceneName);
        else LoadSceneAsync(sceneName);
    }

    // Asynchronous scene loading
    // Right now I think the synchronous loading works better
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncRoutine(sceneName));
    }

    private IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Time.timeScale = 1.0f;
    }

    public void PauseGame()
    {
        ChangeTimeState(TimeState.Paused);
    }

    public void UnpauseGame()
    {
        PlayerPrefs.Save();
        ChangeTimeState(TimeState.Running);
    }

    private void ChangeTimeState(TimeState newTimeState)
    {
        if (newTimeState == CurrentTimeState) return;

        switch (newTimeState)
        {
            case TimeState.Paused:
                Time.timeScale = 0;
                if (PlayerManager.Instance != null) PlayerManager.Instance.controlEnabled = false;
                Cursor.lockState = CursorLockMode.None;
                break;
            default:
                Time.timeScale = 1.0f;
                if (PlayerManager.Instance != null) PlayerManager.Instance.controlEnabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                break;
        }

        CurrentTimeState = newTimeState;
    }
}
