using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(AudioSource))]
public class MapInteractable : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update
    [Tooltip("Events that happend when player interacts with the map")]
    private AudioSource m_audioSource;
    public UnityEvent OnMapInteracted;
    void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        Debug.Log("Interaction with Map");
        if(m_audioSource == null) return;
        m_audioSource.PlayOneShot(m_audioSource.clip,0.1f);
        OnMapInteracted.Invoke();

    }

}
