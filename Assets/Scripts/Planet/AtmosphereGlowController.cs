using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AtmosphereGlowController : MonoBehaviour
{
    [Header("Atmosphere Settings")]
    [SerializeField] private Color atmosphereColor = new Color(0.5f, 0.7f, 1.0f, 1.0f);
    [Range(1, 10)]
    [SerializeField] private float glowFactor = 5f;
    [Range(0, 0.2f)]
    [SerializeField] private float vertexOffset = 0.05f;
    [Range(0, 5)]
    [SerializeField] private float lightIntensity = 1.5f;
    [Range(0, 10)]
    [SerializeField] private float rimPower = 3f;
    
    [Header("Light Settings")]
    [SerializeField] private Transform sunTransform;
    [SerializeField] private bool useSunPosition = true;
    [SerializeField] private Vector3 customLightDirection = new Vector3(0, 1, 0);
    
    [Header("Animation")]
    [SerializeField] private bool animateSun = false;
    [SerializeField] private float rotationSpeed = 10f;
    
    private Material atmosphereMaterial;
    private MeshRenderer meshRenderer;
    private Planet planet;
    
    private void Awake()
    {
        // Get references
        meshRenderer = GetComponent<MeshRenderer>();
        planet = GetComponentInParent<Planet>();
        transform.localScale = new Vector3(1100, 1100, 1100);
        // Create a new material instance
        atmosphereMaterial = new Material(Shader.Find("Planet/AtmosphereGlow"));
        meshRenderer.material = atmosphereMaterial;
        
        // If no sun is assigned, try to find one
        if (sunTransform == null && useSunPosition)
        {
            GameObject sunObject = GameObject.Find("Directional Light");
            if (sunObject != null)
            {
                sunTransform = sunObject.transform;
            }
        }
    }
    
    private void Start()
    {
        // Create a sphere mesh for the atmosphere if needed
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter.sharedMesh == null)
        {
            meshFilter.sharedMesh = CreateSphereMesh(1f, 32, 32);
        }
        
        // Set initial material properties
        UpdateMaterialProperties();
    }
    
    private void Update()
    {
        // Animate the sun if enabled
        if (animateSun && sunTransform != null)
        {
            sunTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        
        // Update material properties
        UpdateMaterialProperties();
    }
    
    private void UpdateMaterialProperties()
    {
        if (atmosphereMaterial != null)
        {
            // Set basic properties
            atmosphereMaterial.SetColor("_AtmoColor", atmosphereColor);
            atmosphereMaterial.SetFloat("_InnerRingFactor", glowFactor);
            atmosphereMaterial.SetFloat("_Offset", vertexOffset);
            atmosphereMaterial.SetFloat("_LightIntensity", lightIntensity);
            atmosphereMaterial.SetFloat("_RimPower", rimPower);
            
            // Set sun direction
            if (useSunPosition && sunTransform != null)
            {
                atmosphereMaterial.SetVector("_SunDir", sunTransform.forward);
            }
            else
            {
                atmosphereMaterial.SetVector("_SunDir", customLightDirection.normalized);
            }
        }
    }
    
    // Helper method to create a sphere mesh
    private Mesh CreateSphereMesh(float radius, int latitudeSegments, int longitudeSegments)
    {
        Mesh mesh = new Mesh();
        
        // Create vertices
        Vector3[] vertices = new Vector3[(latitudeSegments + 1) * (longitudeSegments + 1)];
        for (int lat = 0; lat <= latitudeSegments; lat++)
        {
            float theta = lat * Mathf.PI / latitudeSegments;
            float sinTheta = Mathf.Sin(theta);
            float cosTheta = Mathf.Cos(theta);
            
            for (int lon = 0; lon <= longitudeSegments; lon++)
            {
                float phi = lon * 2 * Mathf.PI / longitudeSegments;
                float sinPhi = Mathf.Sin(phi);
                float cosPhi = Mathf.Cos(phi);
                
                float x = cosPhi * sinTheta;
                float y = cosTheta;
                float z = sinPhi * sinTheta;
                
                vertices[lat * (longitudeSegments + 1) + lon] = new Vector3(x, y, z) * radius;
            }
        }
        
        // Create triangles
        int[] triangles = new int[latitudeSegments * longitudeSegments * 6];
        int index = 0;
        for (int lat = 0; lat < latitudeSegments; lat++)
        {
            for (int lon = 0; lon < longitudeSegments; lon++)
            {
                int current = lat * (longitudeSegments + 1) + lon;
                int next = current + 1;
                int bottom = current + (longitudeSegments + 1);
                int bottomNext = bottom + 1;
                
                triangles[index++] = current;
                triangles[index++] = bottomNext;
                triangles[index++] = bottom;
                
                triangles[index++] = current;
                triangles[index++] = next;
                triangles[index++] = bottomNext;
            }
        }
        
        // Create normals (pointing outward from the center)
        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            normals[i] = vertices[i].normalized;
        }
        
        // Assign to mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        
        return mesh;
    }
    
    // Method to adjust atmosphere settings based on planet properties
    public void AdjustToMatchPlanet()
    {
        if (planet != null)
        {
            if (planet.shapeSettings == null)
            {
                if (PlanetManager.Instance.currentScence == ScenceType.Main)
                {
                    planet.shapeSettings = Resources.Load<ShapeSettings>("Settings/Shape_main");
                }
                else if (PlanetManager.Instance.currentScence == ScenceType.Start)
                {
                    planet.shapeSettings = Resources.Load<ShapeSettings>("Settings/Shape_start");
                }
            }
            // Scale the atmosphere based on the planet's radius
            float planetRadius = planet.shapeSettings.planetRadius;
            transform.localScale = Vector3.one * planetRadius;
        }
    }
}
