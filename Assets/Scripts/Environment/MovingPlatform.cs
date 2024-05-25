using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 _previous_pos;
    public Quaternion _previous_rotation;
    public Vector3 _angle_change;
    public Vector3 _pos_diff;
    public Vector3 _position;
    public float _last_time;

    BoxCollider trigger;
    public float offset = 0.5f;
    Vector3 colliderOffset;
    // Start is called before the first frame update
    void Start()
    {
        trigger = gameObject.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        colliderOffset = new Vector3(0, offset, 0);
    }

    // Update is called once per frame
    void Update()
    {   
        colliderOffset = new Vector3(0, offset, 0);
        UpdateColliderCenter();
        //UpdatePosition();
    }

    public void UpdatePosition(){
        _position = transform.position;
        _pos_diff = _position - _previous_pos;
        _angle_change = transform.rotation.eulerAngles - _previous_rotation.eulerAngles;
        _previous_rotation = transform.rotation;
        _previous_pos = transform.position;
        _last_time = Time.time;
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag("Player")){
            other.gameObject.GetComponent<PlayerMovement>().setMovingPlatform(this);
        }
    }

    void OnTriggerExit(Collider other){
        if (other.gameObject.CompareTag("Player")){
            //other.gameObject.GetComponent<PlayerMovement>().queueRemoveMovingPlatform(this);

        }
    }
    
    public bool ManualColliderCheck(Collider other){
        if (other.gameObject.CompareTag("Player") && trigger.bounds.Intersects(other.bounds)){
            return true;
        }
        return false;
    }

    void UpdateColliderCenter() {
    Quaternion platformRotation = transform.rotation;
    Vector3 newColliderCenter = Quaternion.Inverse(platformRotation) * colliderOffset;
    trigger.center = new Vector3(newColliderCenter.x/transform.localScale.x,
                                    newColliderCenter.y/transform.localScale.y,
                                    newColliderCenter.z/transform.localScale.z);
    }

    public void Disable(){

    }

    public void Enable(){

    }

}
