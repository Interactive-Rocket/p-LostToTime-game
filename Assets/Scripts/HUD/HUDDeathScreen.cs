using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Update()
    {
        if (deathScreen != null && PlayerManager.Instance != null)
        {
            if (!PlayerManager.Instance.isAlive)
            {
                deathScreen.SetActive(true);
            }
        }
    }
}
