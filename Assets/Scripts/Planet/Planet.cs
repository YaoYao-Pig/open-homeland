using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{   
    [Range(2,1024)] public int resolution = 10;
    
    [SerializeField,HideInInspector]
    MeshFilter[] meshFilters;

    TerrainFace[] terrainFaces;
    [HideInInspector] public bool shapeSettingsFoldout;
    [HideInInspector] public bool colourSettingsFoldout;
    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;

    public ShapeGenerator shapeGenerator=new ShapeGenerator();
    private ColourGenerator colourGenerator=new ColourGenerator();
    public int[] faceResolutions;
    private void Awake()
    {
        GeneratePlanet();
        faceResolutions = new int[6] { resolution, resolution, resolution, resolution, resolution, resolution };
    }
    
    /*    [Range(2, 1024)] public int resolutionUp = 10;
    [Range(2, 1024)] public int resolutionDown = 10;
    [Range(2, 1024)] public int resolutionLeft = 10;
    [Range(2, 1024)] public int resolutionRight = 10;
    [Range(2, 1024)] public int resolutionForward = 10;
    [Range(2, 1024)] public int resolutionBack = 10;*/
    private void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colourSettings);
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
            
        }
        
        terrainFaces = new TerrainFace[6];
        

        Vector3[] directions = new Vector3[]
                { Vector3.up, Vector3.down,
                    Vector3.left, Vector3.right,
                    Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; ++i)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObject = new GameObject("mesh" + i.ToString());
                meshObject.transform.parent = transform;
                meshObject.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObject.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;
            terrainFaces[i] = new TerrainFace(shapeGenerator,gameObject,meshFilters[i].sharedMesh, faceResolutions[i], directions[i]);
            
        }
    }


    private void Initialize(int face)
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colourSettings);
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];

        }

        terrainFaces = new TerrainFace[6];


        Vector3[] directions = new Vector3[]
                { Vector3.up, Vector3.down,
                    Vector3.left, Vector3.right,
                    Vector3.forward, Vector3.back };

        
            if (meshFilters[face] == null)
            {
                GameObject meshObject = new GameObject("mesh"+face.ToString());
                meshObject.transform.parent = transform;
                meshObject.AddComponent<MeshRenderer>();
                meshFilters[face] = meshObject.AddComponent<MeshFilter>();
                meshFilters[face].sharedMesh = new Mesh();
            }
            meshFilters[face].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;
            terrainFaces[face] = new TerrainFace(shapeGenerator, gameObject, meshFilters[face].sharedMesh, faceResolutions[face], directions[face]);
        }
   

    public void GeneratePlanet()
    {

        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    public void GeneratePlanet(int face,int _r)
    {
        faceResolutions[face] = _r;
        Initialize();
        GenerateMesh(terrainFaces[face]);
        GenerateColours(terrainFaces[face]);
    }

    public void GeneratePlanet(int[] resolutions)
    {
        faceResolutions = resolutions;
        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    public void GenerateMeshCollider()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.GenerateMeshCollider();
        }
    }

    public void OnShapeSettingsUpdated()
    {
        Initialize();
        GenerateMesh();
    }

    public void OnColourSettingsUpdated()
    {
        Initialize();
        GenerateColours();
    }

    private void GenerateMesh()
    {
        foreach(TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }

        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    private void GenerateMesh(TerrainFace face)
    {
        face.ConstructMesh();
        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    private void GenerateColours()
    {
        colourGenerator.UpdateColours();

        for(int i = 0; i < 6; ++i)
        {
             terrainFaces[i].UpdateUVs(colourGenerator);
        }
    }
    private void GenerateColours(TerrainFace face)
    {
        colourGenerator.UpdateColours();

        face.UpdateUVs(colourGenerator);
       
    }



}
