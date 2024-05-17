using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ComputerScreenController : MonoBehaviour
{   
    [Tooltip("Array containing all possible screens.")]
    [SerializeField] private Material[] screenMaterials;
    [Tooltip("The index of the screen, if there are multiple materials on one object.")]
    [SerializeField] private int screenIndex = 0;

    private MeshRenderer meshRenderer;
    private int currentState = 0;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void NextScreen()
    {
        if (screenMaterials.Length == 0) return;

        currentState++;
        currentState = currentState < screenMaterials.Length ? currentState : 0;

        // If there are multiple materials on one object, then we have to get
        // the material that represents the screen

        Material[] materials = meshRenderer.materials;
        if (materials.Length == 0 || screenIndex > materials.Length - 1) return;

        materials[screenIndex] = screenMaterials[currentState];

        meshRenderer.materials = materials;
    }
}