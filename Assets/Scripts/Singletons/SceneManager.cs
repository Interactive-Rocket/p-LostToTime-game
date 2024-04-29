using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// I wish I could call it "SceneManager" but this is
// already taken by the built-in Unity SceneManager
public class SceneManagerSingleton : MonoBehaviour
{
    public static SceneManagerSingleton Instance { get; private set; }

    private void Awake()
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
}
