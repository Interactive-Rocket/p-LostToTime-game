using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class FakeDarkness : MonoBehaviour
{
    public int _amountOfLayers;
    public int _finalOpacity = 1;
    public Material _baseMaterial;

    // Start is called before the first frame update
    void Start()
    {
        float alpha = math.ceil(Mathf.Pow(_finalOpacity-0.01f,1.0f/_amountOfLayers));
        for(int i = 0; i<_amountOfLayers;i++){
            GameObject layer = GameObject.CreatePrimitive(PrimitiveType.Plane);
            float y = /*transform.localScale.y**/ (i*1.0f / _amountOfLayers  - 0.5f);
            layer.transform.SetParent(transform);
            layer.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
            layer.transform.localRotation = Quaternion.Euler(transform.localRotation.x,-transform.localRotation.y,-transform.localRotation.z);
            layer.transform.localPosition = layer.transform.localRotation * new Vector3(0,y,0) ;
            layer.GetComponent<Collider>().enabled = false;
            // Assign the material to the plane's renderer
            _baseMaterial.color = new Color(0,0,0,alpha);
            layer.GetComponent<Renderer>().material = _baseMaterial;
        }
        GetComponent<MeshRenderer>().enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
