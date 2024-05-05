using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public float spawnDelay;
    public float autoRemoveAfter = 0;

    public 
    // Start is called before the first frame update
    void Start()
    {
        // Schedule first object spawn
        Invoke("SpawnPrefab",spawnDelay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnPrefab(){
        if (prefabToSpawn != null)
        {
            // If a prefab is defined, spawn it and schedule its destruction
            GameObject spawnedObject = Instantiate(prefabToSpawn,transform.position,Quaternion.identity);
            Destroy(spawnedObject,(autoRemoveAfter==0)?10f:autoRemoveAfter);
        }
        // Schedule next object spawn
        Invoke("SpawnPrefab",spawnDelay);
    }
}
