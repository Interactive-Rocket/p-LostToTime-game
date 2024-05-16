using UnityEngine;

public class EntityDestroyTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag("Player") && collider.GetComponent<Rigidbody>() != null)
        {
            Destroy(collider.gameObject);
        }
    }
}
