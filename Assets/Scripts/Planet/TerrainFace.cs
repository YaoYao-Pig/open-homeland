using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//һ���������������
public class TerrainFace
{
    ShapeGenerator shapeGenerator;
    Mesh mesh;
    int resoulutoin;
    Vector3 localUp;//��ķ�����ָ�ġ��㡱
    Vector3 axisA;
    Vector3 axisB;


    public TerrainFace(ShapeGenerator _shapeGenerator,Mesh _mesh,int _resoulution,Vector3 _localUp)
    {
        shapeGenerator = _shapeGenerator;
        mesh = _mesh;
        resoulutoin = _resoulution;
        localUp = _localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);

        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertics = new Vector3[resoulutoin * resoulutoin];
        int[] triangles = new int[(resoulutoin - 1) * (resoulutoin - 1) * 6];//�����ζ��㣨index������
        Vector2[] uv = (mesh.uv.Length==vertics.Length)?mesh.uv:new Vector2[vertics.Length];

        
        int triIndex = 0;
        for(int y = 0; y < resoulutoin; ++y)
        {
            for(int x = 0; x < resoulutoin; ++x)
            {
                int i = x + y * resoulutoin;
                Vector2 percent = new Vector2(x, y) / (resoulutoin - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                float unscaledElevation = shapeGenerator.CalculateUnscaledElevation(pointOnUnitSphere);
                vertics[i] = pointOnUnitSphere * shapeGenerator.GetScaleElevation(unscaledElevation);
                uv[i].y = unscaledElevation;

                if (x != resoulutoin - 1 && y != resoulutoin - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex+1] = i+resoulutoin+1;
                    triangles[triIndex+2] = i + resoulutoin ;
                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resoulutoin+1;
                    triIndex += 6;
                }
            }
        }
        mesh.Clear();

        mesh.vertices = vertics;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uv;
    }
    public void UpdateUVs(ColourGenerator colourGenerator)
    {
        Vector2[] uv = mesh.uv;

        for (int y = 0; y < resoulutoin; ++y)
        {
            for (int x = 0; x < resoulutoin; ++x)
            {
                int i = x + y * resoulutoin;
                Vector2 percent = new Vector2(x, y) / (resoulutoin - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;


                //uv��x���ִ洢����ʲôBiome��������y����ֵΪ0��ʵ���������uvֻ����Ϊһ��������Ϣ�Ĵ洢
                uv[i].x = colourGenerator.BiomePercentFromPoint(pointOnUnitSphere);
            }
        }
        mesh.uv = uv;
    }
}
