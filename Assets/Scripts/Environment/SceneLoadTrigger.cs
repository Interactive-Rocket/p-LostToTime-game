using UnityEngine;

public class SceneLoadTrigger : MonoBehaviour
{
    [Tooltip("String name of the scene to load.")]
    public string sceneToLoad;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            SceneTransitionManager.Instance.FadeOut();
            //if (SceneManagerSingleton.Instance != null) SceneManagerSingleton.Instance.LoadScene(sceneToLoad);
        }
    }
}
