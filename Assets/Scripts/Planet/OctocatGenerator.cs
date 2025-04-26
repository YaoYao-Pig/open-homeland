using UnityEngine;

public class OctocatGenerator : MonoBehaviour
{
    [Header("Octocat Settings")]
    [SerializeField] private Material octocatMaterial;
    [SerializeField] private float headSize = 1f;
    [SerializeField] private float tentacleLength = 0.5f;
    [SerializeField] private float tentacleWidth = 0.1f;
    [SerializeField] private float eyeSize = 0.2f;
    
    private GameObject octocatObject;
    
    [ContextMenu("Generate Octocat")]
    public void GenerateOctocat()
    {
        // Clean up any existing octocat
        if (octocatObject != null)
        {
            DestroyImmediate(octocatObject);
        }
        
        // Create parent object
        octocatObject = new GameObject("Octocat");
        octocatObject.transform.parent = transform;
        octocatObject.transform.localPosition = Vector3.zero;
        octocatObject.transform.localRotation = Quaternion.identity;
        
        // Create head
        GameObject head = CreateSphere("Head", headSize);
        head.transform.parent = octocatObject.transform;
        
        // Create tentacles
        CreateTentacles();
        
        // Create eyes
        CreateEyes();
        
        // Apply material
        ApplyMaterialToAll();
    }
    
    private GameObject CreateSphere(string name, float size)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = name;
        sphere.transform.localScale = Vector3.one * size;
        return sphere;
    }
    
    private GameObject CreateCylinder(string name, float length, float width)
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.name = name;
        cylinder.transform.localScale = new Vector3(width, length / 2, width);
        return cylinder;
    }
    
    private void CreateTentacles()
    {
        // Create 5 tentacles around the top of the head
        for (int i = 0; i < 5; i++)
        {
            float angle = i * (360f / 5) * Mathf.Deg2Rad;
            float x = Mathf.Sin(angle) * (headSize * 0.8f);
            float z = Mathf.Cos(angle) * (headSize * 0.8f);
            
            GameObject tentacle = CreateCylinder("Tentacle_" + i, tentacleLength, tentacleWidth);
            tentacle.transform.parent = octocatObject.transform;
            tentacle.transform.localPosition = new Vector3(x, headSize * 0.5f, z);
            
            // Rotate tentacle to point outward
            tentacle.transform.LookAt(tentacle.transform.position + new Vector3(x, headSize, z));
        }
    }
    
    private void CreateEyes()
    {
        // Create two eyes on the front of the head
        float eyeOffset = headSize * 0.3f;
        
        GameObject leftEye = CreateSphere("LeftEye", eyeSize);
        leftEye.transform.parent = octocatObject.transform;
        leftEye.transform.localPosition = new Vector3(-eyeOffset, eyeOffset, headSize * 0.8f);
        
        GameObject rightEye = CreateSphere("RightEye", eyeSize);
        rightEye.transform.parent = octocatObject.transform;
        rightEye.transform.localPosition = new Vector3(eyeOffset, eyeOffset, headSize * 0.8f);
    }
    
    private void ApplyMaterialToAll()
    {
        if (octocatMaterial != null)
        {
            Renderer[] renderers = octocatObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.material = octocatMaterial;
            }
        }
    }
    
    // Call this method to create a prefab from the generated Octocat
    public GameObject CreatePrefab()
    {
        if (octocatObject == null)
        {
            GenerateOctocat();
        }
        
        return octocatObject;
    }
}
