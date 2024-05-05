using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [Tooltip("Array containing death sounds")]
    [SerializeField] private AudioClip[] deathSounds;
    private bool isDead = false;
    private bool hasKilledPlayer = false;

    void Update()
    {
        if (PlayerManager.Instance != null) isDead = !PlayerManager.Instance.isAlive;

        if (isDead && !hasKilledPlayer)
        {
            OnPlayerDeath();
        }
    }

    public void KillPlayer()
    {
        isDead = true;
    }

    private void OnPlayerDeath() {
        hasKilledPlayer = true;
        if (PlayerManager.Instance != null) PlayerManager.Instance.isAlive = false;

        // Play death sound if there is an AudioManager in the scene
        if (AudioManager.Instance != null) AudioManager.Instance.PlayOneShot(deathSounds);
    }
}