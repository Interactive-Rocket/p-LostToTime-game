using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    // Reference to the player
    public GameObject PlayerRootGameObject { get; private set; }
    public GameObject PlayerCapsuleGameObject;

    [Header("Spawning")]
    [Tooltip("Player prefab. Spawns the player at the spawnPoint.")]
    [SerializeField] private GameObject playerPrefab;
    [Tooltip("HUD prefab. Instantiated at scene load.")]
    [SerializeField] private GameObject HUDPrefab;
    [Tooltip("Pause menu prefab. Instantiated at scene load.")]
    [SerializeField] private GameObject PauseMenuPrefab;
    [Tooltip("Game object where the player is instantiated.")]
    [SerializeField] private GameObject spawnPoint;

    [Header("Setable fields and states")]
    public bool isAlive = true;
    public bool controlEnabled = true;
    public float volume = 1.0f;
    public float sensitivity = 1.0f;

    [Header("Grabbed Objects Fields")]
    [Tooltip("Please make a better name for this.")]
    public float MagicGrabMoveSpeedNumber = 20f;
    [Tooltip("Angular drag for grabbed objects, to resist spinning.")]
    public float GrabbedObjectAngularDrag = 100f;

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
            PlayerRootGameObject = Instantiate(playerPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }

        if (HUDPrefab == null)
        {
            Debug.LogError("No HUD Prefab selected.");
        }
        else
        {
            Instantiate(HUDPrefab);
        }

        if (PauseMenuPrefab == null)
        {
            Debug.LogError("No pause menu prefab selected.");
        }
        else
        {
            Instantiate(PauseMenuPrefab);
        }

        // Load volume and sensitivity
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            sensitivity = PlayerPrefs.GetFloat("Sensitivity");
        }

        if (PlayerPrefs.HasKey("Volume"))
        {
            volume = PlayerPrefs.GetFloat("Volume");
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