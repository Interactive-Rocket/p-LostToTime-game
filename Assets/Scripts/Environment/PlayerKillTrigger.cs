using UnityEngine;

public class PlayerKillTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (PlayerManager.Instance != null) PlayerManager.Instance.isAlive = false;
        }
    }
}
