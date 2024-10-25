using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSphere : MonoBehaviour
{

    public float offset = 0;
    private void Awake()
    {
        GenerateSphereStar();
    }


    public void GenerateSphereStar()
    {
        InitializeSphere();
        GenerateMeshSphere();
    }

    private void GenerateMeshSphere()
    {
        throw new NotImplementedException();
    }

    private void InitializeSphere()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Vector3[] vertics = meshFilter.sharedMesh.vertices;

        for(int i = 0; i < vertics.Length; ++i)
        {
            vertics[i].x += 1 + Mathf.Sin(vertics[i].y * offset) * 0.05f;
            vertics[i].z += 1 + Mathf.Sin(vertics[i].y * offset) * 0.05f;
        }
    }


}
