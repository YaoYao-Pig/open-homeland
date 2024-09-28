using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{   
    [Range(2,256)] public int resolution = 10;
    
    [SerializeField,HideInInspector]
    MeshFilter[] meshFilters;

    TerrainFace[] terrainFaces;


    private void OnValidate()
    {
        Initialize();
        GenerateMesh();
    }

    private void Initialize()
    {
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
                GameObject meshObject = new GameObject("mesh");
                meshObject.transform.parent = transform;
                meshObject.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObject.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }
    private void GenerateMesh()
    {
        foreach(TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }

    }

}
