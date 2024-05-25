using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SoundController))]
[RequireComponent(typeof(TimeEntity))]
public class ConveyorController : MonoBehaviour
{
    List<GameObject> pushedObjects;
    public float pushForce = 5;
    Vector3 pushDirection = Vector3.back;
    public AudioClip activeSound;
    [SerializeField]
    private SoundController soundController;
    [SerializeField]
    private TimeEntity timeEntity;
    public bool reversing;
    // Start is called before the first frame update
    void Start()
    {
        pushedObjects = new List<GameObject>();
        soundController = GetComponent<SoundController>();
        if (soundController == null){
            print("missing sound controller");
        }
        if (activeSound == null){
            print("missing activeSound");
        }
        soundController.audioClip = activeSound;
        soundController.Play(activeSound);
        float skiptime = UnityEngine.Random.Range(0,1f);
        print(skiptime + " skipped");
        soundController.SeekTo(skiptime);
        timeEntity = GetComponent<TimeEntity>();
    }

    // Update is called once per frame
    void Update()
    {   
        
    }

    void FixedUpdate(){
        List<GameObject> destroyedObjects = new List<GameObject>();
        foreach (GameObject pushedObject in pushedObjects){
            if(pushedObject.IsDestroyed()){
                destroyedObjects.Add(pushedObject);
                continue;
            }
            pushedObject.transform.Translate((reversing?-1:1) * timeEntity._timeScale * pushDirection*pushForce/100,Space.World);
        }
        
        foreach (GameObject destroyedObject in destroyedObjects){
                    pushedObjects.Remove(destroyedObject);
        }
    }

    private void OnCollisionEnter(Collision other){
        Debug.Log("New Colission" + other.gameObject.name);
        Rigidbody otherBody = other.gameObject.GetComponent<Rigidbody>();
        if (otherBody != null){
            Debug.Log("RigidBody added");
            pushedObjects.Add(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Player")){
            Debug.Log("Player added");
            pushedObjects.Add(other.gameObject.transform.parent.gameObject);
        }
    }

    private void OnCollisionExit(Collision other){
        Rigidbody otherBody = other.gameObject.GetComponent<Rigidbody>();
        if (otherBody != null){
            pushedObjects.Remove(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Player")){
            pushedObjects.Remove(other.gameObject.transform.parent.gameObject);
        }
    }
}
