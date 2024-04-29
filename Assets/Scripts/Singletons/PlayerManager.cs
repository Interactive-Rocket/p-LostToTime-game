using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    // Reference to the player
    public GameObject PlayerGameObject { get; private set; }

    [Header("Spawning")]
    [Tooltip("Player prefab. Currently does nothing.")]
    [SerializeField]
    private GameObject playerPrefab;
    [Tooltip("Game object where the player is instantiated. Currently does nothing.")]
    [SerializeField]
    private GameObject spawnPoint;

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
    }

    void Update() {
        if (!isAlive) controlEnabled = false;
    }
}