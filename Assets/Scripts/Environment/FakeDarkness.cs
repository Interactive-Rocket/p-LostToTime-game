using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEngine;

public class FakeDarkness : MonoBehaviour
{
    public int _amountOfLayers;
    public float _finalOpacity = 1;
    public Material _baseMaterial;
    public float _alpha;

    // Start is called before the first frame update
    void Start()
    {
        float alpha = 1 - Mathf.Pow(0.001f, 1.0f / _amountOfLayers);
        _alpha = alpha;
        for(int i = 0; i<_amountOfLayers;i++){
            Material material = new Material(_baseMaterial);
            GameObject layer = GameObject.CreatePrimitive(PrimitiveType.Plane);
            float y =  i*1.0f / _amountOfLayers  - 0.5f;
            layer.transform.SetParent(transform);
            layer.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
            layer.transform.localRotation = Quaternion.Euler(transform.localRotation.x,-transform.localRotation.y,-transform.localRotation.z);
            layer.transform.localPosition = layer.transform.localRotation * new Vector3(0,y,0) ;
            layer.GetComponent<Collider>().enabled = false;
            // Assign the material to the plane's renderer
            Color modifiedColor =  new Color(0,0,0,1f/(_amountOfLayers-i));
            material.color = modifiedColor;
            material.SetColor("_BaseColor",modifiedColor);
            material.SetColor("_Color",modifiedColor);
            layer.GetComponent<Renderer>().material = material;
        }
        GetComponent<MeshRenderer>().enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
