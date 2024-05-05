using UnityEngine;

[RequireComponent(typeof(HUDManager))]
public class HUDDeathScreen : MonoBehaviour
{
    [SerializeField] private GameObject deathScreen;

    void Awake()
    {
        if (deathScreen != null)
        {
            deathScreen.SetActive(false);
        }
    }

    public void OnPlayerDeathScreen()
    {
        if (deathScreen != null && HUDManager.Instance != null)
        {
            deathScreen.SetActive(HUDManager.Instance.PlayerDeathScreen);
        }
    }
}
