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
    private SoundController soundController;
    private TimeEntity timeEntity;
    public bool reversing;
    // Start is called before the first frame update
    void Start()
    {
        pushedObjects = new List<GameObject>();
        soundController = GetComponent<SoundController>();
        timeEntity = GetComponent<TimeEntity>();
    }

    // Update is called once per frame
    void Update()
    {   
        
    }

    void FixedUpdate(){
        if (pushedObjects.Count > 0)
            foreach (GameObject pushedObject in pushedObjects){
                pushedObject.transform.Translate((reversing?-1:1) * timeEntity._timeScale * pushDirection*pushForce/100,Space.World);
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
