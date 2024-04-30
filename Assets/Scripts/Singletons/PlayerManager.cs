using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    // Reference to the player
    public GameObject PlayerGameObject { get; private set; }

    [Header("Spawning")]
    [Tooltip("Player prefab. Spawns the player at the spawnPoint.")]
    [SerializeField] private GameObject playerPrefab;
    [Tooltip("HUD prefab. Instantiated at scene load.")]
    [SerializeField] private GameObject HUDPrefab;
    [Tooltip("Game object where the player is instantiated.")]
    [SerializeField] private GameObject spawnPoint;

    [Header("Setable fields and states")]
    public bool isAlive = true;
    public bool controlEnabled = true;

    void Awake()
    {
        // Ensure it's a singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("No Player Prefab selected.");
        }
        else if (spawnPoint == null)
        {
            Debug.LogError("No spawn point attached to the Player Manager.");
        }
        else
        {
            PlayerGameObject = Instantiate(playerPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }

        if (HUDPrefab == null) {
            Debug.LogError("No HUD Prefab selected.");
        }
        else {
            Instantiate(HUDPrefab);
        }
    }

    void Update()
    {
        if (!isAlive)
        {
            if (HUDManager.Instance != null) HUDManager.Instance.PlayerDeathScreen = true;
            controlEnabled = false;
        }
    }
}