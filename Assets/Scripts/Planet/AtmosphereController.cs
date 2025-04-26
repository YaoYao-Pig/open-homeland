using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AtmosphereController : MonoBehaviour
{
    [Header("Atmosphere Settings")]
    [SerializeField] private Color atmosphereColor = new Color(0.5f, 0.7f, 1.0f, 1.0f);
    [Range(0, 1)]
    [SerializeField] private float atmosphereAlpha = 0.5f;
    [Range(0, 0.5f)]
    [SerializeField] private float atmosphereHeight = 0.1f;
    [Range(0, 10)]
    [SerializeField] private float atmosphereFalloff = 3f;
    [Range(0, 20)]
    [SerializeField] private float atmosphereRimPower = 5f;
    
    [Header("Light Scattering")]
    [SerializeField] private Transform sunTransform;
    [Range(0, 10)]
    [SerializeField] private float sunIntensity = 1f;
    [Range(0, 10)]
    [SerializeField] private float rayleighCoefficient = 1f;
    [Range(0, 10)]
    [SerializeField] private float mieCoefficient = 0.5f;
    [Range(0, 1)]
    [SerializeField] private float mieDirectionalG = 0.8f;
    
    [Header("Time of Day")]
    [SerializeField] private bool animateSun = false;
    [SerializeField] private float dayLength = 120f; // seconds
    
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
        atmosphereMaterial = new Material(Shader.Find("Planet/Atmosphere"));
        meshRenderer.material = atmosphereMaterial;
        
        // If no sun is assigned, create a directional light
        if (sunTransform == null)
        {
            GameObject sunObject = GameObject.Find("Directional Light");
            if (sunObject != null)
            {
                sunTransform = sunObject.transform;
            }
            else
            {
                sunObject = new GameObject("Sun");
                Light light = sunObject.AddComponent<Light>();
                light.type = LightType.Directional;
                sunTransform = sunObject.transform;
                sunTransform.position = new Vector3(0, 0, 0);
                sunTransform.rotation = Quaternion.Euler(50, 30, 0);
            }
        }
    }
    
    private void Start()
    {
        // Create a sphere mesh for the atmosphere
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
            float angle = (Time.time % dayLength) / dayLength * 360f;
            sunTransform.rotation = Quaternion.Euler(angle, 30, 0);
        }
        
        // Update material properties
        UpdateMaterialProperties();
    }
    
    private void UpdateMaterialProperties()
    {
        if (atmosphereMaterial != null)
        {
            // Set basic properties
            atmosphereMaterial.SetColor("_AtmosphereColor", atmosphereColor);
            atmosphereMaterial.SetFloat("_AtmosphereAlpha", atmosphereAlpha);
            atmosphereMaterial.SetFloat("_AtmosphereHeight", atmosphereHeight);
            atmosphereMaterial.SetFloat("_AtmosphereFalloff", atmosphereFalloff);
            atmosphereMaterial.SetFloat("_AtmosphereRimPower", atmosphereRimPower);
            
            // Set sun and scattering properties
            if (sunTransform != null)
            {
                atmosphereMaterial.SetVector("_SunDir", -sunTransform.forward);
            }
            atmosphereMaterial.SetFloat("_SunIntensity", sunIntensity);
            atmosphereMaterial.SetFloat("_RayleighCoefficient", rayleighCoefficient);
            atmosphereMaterial.SetFloat("_MieCoefficient", mieCoefficient);
            atmosphereMaterial.SetFloat("_MieDirectionalG", mieDirectionalG);
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
            // Scale the atmosphere based on the planet's radius
            float planetRadius = planet.shapeSettings.planetRadius;
            transform.localScale = Vector3.one * planetRadius;
        }
    }
}
