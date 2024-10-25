using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarFace 
{

    Mesh mesh;
    int resolution;
    Vector3 localUp;//面的法线所指的“点”
    Vector3 axisA;
    Vector3 axisB;
    float offset;


    private float rimWidth = 5.0f;
    private float rimSteepness=10f;
    private float floorHeight=0.5f;
    private int numCrater = 1;
    private float craterRadius = 1.0f;

    public StarFace(Mesh mesh, int resolution, Vector3 localUp, float offset)
    {
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
        this.offset = offset;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    private float CavityShape(float x)
    {
        return x * x * x - 1;
    }

    private float RimShape(float x)
    {
        x = Mathf.Min(x - 1 - rimWidth, 0);
        return rimSteepness * x * x;
    }

    private float FloorShape(float x)
    {
        return floorHeight;
    }

    private float CraterShape(float x)
    {
        float cavity = CavityShape(x);
        float rim = RimShape(x);
        float craterShape = Mathf.Max(cavity, FloorShape(x));
        return Mathf.Min(craterShape, rim);
    }

    public void ConstructStarMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        Vector2[] uv = (mesh.uv.Length == vertices.Length) ? mesh.uv : new Vector2[vertices.Length];

        int triIndex = 0;
        for (int y = 0; y < resolution; ++y)
        {
            for (int x = 0; x < resolution; ++x)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                float craterHeight = 0;
                for (int crater = 0; crater < numCrater; crater++)
                {
                    float distanceToCrater = Vector3.Distance(pointOnUnitSphere, Vector3.zero) / craterRadius;
                    craterHeight += CraterShape(distanceToCrater) * craterRadius;
                }

                vertices[i] = pointOnUnitSphere;
                vertices[i] += new Vector3(0, craterHeight, 0);

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;
                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uv;
    }

}
