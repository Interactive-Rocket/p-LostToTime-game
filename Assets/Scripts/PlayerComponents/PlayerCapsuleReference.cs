using UnityEngine;

public class PlayerCapsuleReference : MonoBehaviour
{
    // Does nothing more than update the PlayerManager with a reference to the player capsule
    void OnEnable()
    {
        if (PlayerManager.Instance != null) PlayerManager.Instance.PlayerCapsuleGameObject = gameObject;
    }
}