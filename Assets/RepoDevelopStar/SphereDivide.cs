using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereDivide : MonoBehaviour
{
    // ϸ�ִ���
    public int subdivisions = 3;

    private Mesh mesh;


    public void GenerateSphere()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        // ���ɳ�ʼ��ʮ������
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // ������ʼ�������
        CreateIcosahedron(vertices, triangles);

        // ϸ��
        for (int i = 0; i < subdivisions; i++)
        {
            Subdivide(ref vertices, ref triangles);
        }

        // ��������
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    void CreateIcosahedron(List<Vector3> vertices, List<int> triangles)
    {
        float phi = (1f + Mathf.Sqrt(5f)) / 2f;

        // ���㣨12���������嶥�㣩
        vertices.Add(new Vector3(-1, phi, 0).normalized);
        vertices.Add(new Vector3(1, phi, 0).normalized);
        vertices.Add(new Vector3(-1, -phi, 0).normalized);
        vertices.Add(new Vector3(1, -phi, 0).normalized);
        vertices.Add(new Vector3(0, -1, phi).normalized);
        vertices.Add(new Vector3(0, 1, phi).normalized);
        vertices.Add(new Vector3(0, -1, -phi).normalized);
        vertices.Add(new Vector3(0, 1, -phi).normalized);
        vertices.Add(new Vector3(phi, 0, -1).normalized);
        vertices.Add(new Vector3(phi, 0, 1).normalized);
        vertices.Add(new Vector3(-phi, 0, -1).normalized);
        vertices.Add(new Vector3(-phi, 0, 1).normalized);

        // �棨20�������Σ�
        triangles.AddRange(new int[] { 0, 11, 5 });
        triangles.AddRange(new int[] { 0, 5, 1 });
        triangles.AddRange(new int[] { 0, 1, 7 });
        triangles.AddRange(new int[] { 0, 7, 10 });
        triangles.AddRange(new int[] { 0, 10, 11 });
        triangles.AddRange(new int[] { 1, 5, 9 });
        triangles.AddRange(new int[] { 5, 11, 4 });
        triangles.AddRange(new int[] { 11, 10, 2 });
        triangles.AddRange(new int[] { 10, 7, 6 });
        triangles.AddRange(new int[] { 7, 1, 8 });
        triangles.AddRange(new int[] { 3, 9, 4 });
        triangles.AddRange(new int[] { 3, 4, 2 });
        triangles.AddRange(new int[] { 3, 2, 6 });
        triangles.AddRange(new int[] { 3, 6, 8 });
        triangles.AddRange(new int[] { 3, 8, 9 });
        triangles.AddRange(new int[] { 4, 9, 5 });
        triangles.AddRange(new int[] { 2, 4, 11 });
        triangles.AddRange(new int[] { 6, 2, 10 });
        triangles.AddRange(new int[] { 8, 6, 7 });
        triangles.AddRange(new int[] { 9, 8, 1 });
    }

    void Subdivide(ref List<Vector3> vertices, ref List<int> triangles)
    {
        Dictionary<string, int> middlePointCache = new Dictionary<string, int>();
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();

        // ������еĶ���
        newVertices.AddRange(vertices);

        // ϸ��ÿ��������
        for (int i = 0; i < triangles.Count; i += 3)
        {
            int v1 = triangles[i];
            int v2 = triangles[i + 1];
            int v3 = triangles[i + 2];

            // ��ȡ�ߵ��е�
            int a = GetMiddlePoint(v1, v2, ref middlePointCache, ref newVertices);
            int b = GetMiddlePoint(v2, v3, ref middlePointCache, ref newVertices);
            int c = GetMiddlePoint(v3, v1, ref middlePointCache, ref newVertices);

            // ����4���µ�������
            newTriangles.Add(v1);
            newTriangles.Add(a);
            newTriangles.Add(c);

            newTriangles.Add(v2);
            newTriangles.Add(b);
            newTriangles.Add(a);

            newTriangles.Add(v3);
            newTriangles.Add(c);
            newTriangles.Add(b);

            newTriangles.Add(a);
            newTriangles.Add(b);
            newTriangles.Add(c);
        }

        vertices = newVertices;
        triangles = newTriangles;
    }

    int GetMiddlePoint(int p1, int p2, ref Dictionary<string, int> cache, ref List<Vector3> vertices)
    {
        string key = p1 < p2 ? p1 + "-" + p2 : p2 + "-" + p1;

        if (cache.ContainsKey(key))
            return cache[key];

        Vector3 middle = (vertices[p1] + vertices[p2]) / 2f;
        middle = middle.normalized; // ʹ�е��׼��

        vertices.Add(middle);
        int index = vertices.Count - 1;
        cache[key] = index;

        return index;
    }
}