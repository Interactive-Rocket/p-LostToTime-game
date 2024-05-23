using UnityEngine;

public class ObjectDespawnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Despawnable"))
        {
            if (RewindManager.Instance != null)
            {
                if (RewindManager.Instance.ObjectIsSelected(collider.gameObject))
                {
                    RewindManager.Instance.DeselectObject(collider.gameObject);
                }
            }

            Destroy(collider.gameObject);
        }
    }
}
