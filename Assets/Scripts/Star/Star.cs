using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [Range(2, 512)] public int starResolution;

    StarFace[] starFaces;
    MeshFilter[] meshFilters;

    public Material material;
    public float offset;

    private void Awake()
    {
        GenerateStar();
    }


/*    public void GenerateSphereStar()
    {
        InitializeSphere();
        GenerateMeshSphere();
    }*/

    public void GenerateStar()
    {
        Initialize();
        GenerateMesh();
    }
    private void Initialize()
    {
        if (meshFilters == null|| meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }

        starFaces = new StarFace[6];

        Vector3[] directions = new Vector3[]
        {
                    Vector3.up, Vector3.down,
                    Vector3.left, Vector3.right,
                    Vector3.forward, Vector3.back
        };
        for(int i = 0; i < 6; ++i)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObject = new GameObject("starMesh" + i.ToString());
                meshObject.transform.parent = transform;
                meshObject.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObject.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = material;
            starFaces[i] = new StarFace(meshFilters[i].sharedMesh, starResolution, directions[i], offset);

        }

    }

    private void GenerateMesh()
    {
        foreach (StarFace face in starFaces)
        {
            face.ConstructStarMesh();
        }

    }
}
