using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour
{

    [SerializeField] private SphereDivide sphereDivide;
    [SerializeField] private StarCompute starCompute;
    [SerializeField] private float radius;
    public static StarController Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void GenerateStars()
    {
        float lastOpenRank = GetData();
        sphereDivide.GenerateSphere();
        Debug.Log("crater number:"+(int)lastOpenRank);
        float[] heights = starCompute.Initialize(gameObject.GetComponent<MeshFilter>().sharedMesh,(int)lastOpenRank==0?100: (int)lastOpenRank*100);
        UpdateSphereHeight(gameObject.GetComponent<MeshFilter>().sharedMesh, heights);
    }

    private float GetData()
    {
        Repo_Read_OpenRank repoOpenRank =GameData.Instance.GetRepoOpenRankList();
        
        return repoOpenRank.lastOpenRank;
    }

    private void UpdateSphereHeight(Mesh mesh, float[] heights)
    {
        // ��ȡ��ǰ����Ķ���
        Vector3[] vertices = mesh.vertices;

        // ȷ�� heights ����ĳ����붥������һ��
        if (vertices.Length != heights.Length)
        {
            Debug.LogError("Height array size does not match vertex count!");
            return;
        }

        // ���¶���ĸ߶�
        for (int i = 0; i < vertices.Length; i++)
        {
            // ���¶���� y ����Ϊ��������ĸ߶�
            vertices[i]*= heights[i];
            vertices[i] *= radius;
        }

        // �����º�Ķ�������Ӧ�õ� Mesh ��
        mesh.vertices = vertices;

        // ���¼��㷨�ߣ�ʹ���忴��������Ȼ
        mesh.RecalculateNormals();

    }
}
