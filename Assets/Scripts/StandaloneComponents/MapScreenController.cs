using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MapScreenController : MonoBehaviour
{
    private string[] m_screens = new string[] { "map_1", "map_2", "map_3" };
    private int m_currentScreen = 0;
    [Tooltip("Array containing all possible screens on the map")]
    [SerializeField] private Material[] m_screenMaterials;
    [Tooltip("The index of the material that represents the screen, if there are many materials on the mesh")]
    [SerializeField] private int m_screenIndexInMaterials;
    private Material m_currentScreenMaterial;
    private MeshRenderer m_meshRenderer;
    void Awake()
    {
        GetGameObjectScreenMaterial();
    }

    private void GetGameObjectScreenMaterial()
    {
        if (m_screenMaterials.Length == 0) return; // Check if the component has screens to show
        m_meshRenderer = GetComponent<MeshRenderer>();
        if (m_meshRenderer.materials.Length == 0 || m_screenIndexInMaterials >= m_meshRenderer.materials.Length) return;
        m_currentScreenMaterial = m_meshRenderer.materials[m_screenIndexInMaterials];
    }

    private void UpdateMeshScreenMaterial(int index)
    {
        m_currentScreenMaterial = m_screenMaterials[index];
        Material[] materials = m_meshRenderer.materials;
        materials[m_screenIndexInMaterials] = m_currentScreenMaterial;
        m_meshRenderer.materials = materials;
        Debug.Log(m_meshRenderer);
        Debug.Log("Currently at screen:" + m_screenMaterials[m_currentScreen]);
    }

    public void SetScreen(int screenIndex)
    {
        m_currentScreen = screenIndex >= m_screenMaterials.Length ? m_currentScreen : m_currentScreen = screenIndex;
        /*
            Currently, the player could potentially go directly to the daugther screen 
            without selecting the subject details.
        */
        UpdateMeshScreenMaterial(m_currentScreen);
    }

    public void GoBackScreen()
    {
        if (m_currentScreen > 0) m_currentScreen--;
        UpdateMeshScreenMaterial(m_currentScreen);
        //m_currentScreenMaterial = m_screenMaterials[m_currentScreen];
        //m_meshRenderer.materials[m_screenIndexInMaterials] = m_currentScreenMaterial;
        //Debug.Log("Currently at screen:" + m_screens[m_currentScreen]);
    }

    public void CloseScreen()
    {
        m_currentScreen = 0;
        UpdateMeshScreenMaterial(m_currentScreen);
        //m_currentScreenMaterial = m_screenMaterials[m_currentScreen];
        //m_meshRenderer.materials[m_screenIndexInMaterials] = m_currentScreenMaterial;
        //Debug.Log("Currently at screen:" + m_screens[m_currentScreen]);
    }
}
