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
        // 获取当前球体的顶点
        Vector3[] vertices = mesh.vertices;

        // 确保 heights 数组的长度与顶点数量一致
        if (vertices.Length != heights.Length)
        {
            Debug.LogError("Height array size does not match vertex count!");
            return;
        }

        // 更新顶点的高度
        for (int i = 0; i < vertices.Length; i++)
        {
            // 更新顶点的 y 坐标为计算出来的高度
            vertices[i]*= heights[i];
            vertices[i] *= radius;
        }

        // 将更新后的顶点重新应用到 Mesh 上
        mesh.vertices = vertices;

        // 重新计算法线，使球体看起来更自然
        mesh.RecalculateNormals();

    }
}
