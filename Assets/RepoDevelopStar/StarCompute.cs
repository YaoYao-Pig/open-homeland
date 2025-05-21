using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class StarCompute : MonoBehaviour
{
    public ComputeShader craterComputeShader;  // 引用 Compute Shader
    //public int craterNum = 1;
    public float rimSteepness = 10.0f;  // 环形陡峭度
    public float rimWidth = 0.2f;  // 环形宽度
    public Vector2 craterSizeMinMax = new Vector2(0.01f, 0.1f);
    public float smoothFactor = 0.1f;  // 平滑度
    public Vector2 smoothMinMax = new Vector2 (0.4f, 1.5f);
    public float floorHeight = 0.0f;  // 地面高度
    public int craterSeed=0;
    public int masterSeed=1;
    [Range(0, 1)]
    public float sizeDistribution = 0.6f;


    private ComputeBuffer vertexBuffer;  // 顶点数据缓冲区
    private ComputeBuffer heightBuffer;  // 高度数据缓冲区
    private ComputeBuffer craterBuffer;  // 陨石坑数据缓冲区

    public float[] Initialize(Mesh mesh,int craterNum)
    {
        // 获取原始顶点数据
        Vector3[] vertices = mesh.vertices;
        int numVertices = vertices.Length;


        Random.InitState(craterSeed + masterSeed);
        PRNG prng = new PRNG(masterSeed);

        // 创建 ComputeBuffer 用于存储顶点数据和高度数据
        vertexBuffer = new ComputeBuffer(numVertices, sizeof(float) * 3);  // float3 大小
        heightBuffer = new ComputeBuffer(numVertices, sizeof(float));  // 存储计算出的高度
        craterBuffer = new ComputeBuffer(craterNum, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Crater))); 
        // 创建陨石坑数据并传递给 Shader
        Crater[] craters = new Crater[craterNum];  // 假设有 5 个陨石坑
        for (int i = 0; i < craterNum; i++)
        {
            float t = prng.ValueBiasLower(sizeDistribution);
            craters[i] = new Crater
            {
                centre = Random.onUnitSphere,  // 假设每个陨石坑间隔 5 单位
                radius = Mathf.Lerp(craterSizeMinMax.x, craterSizeMinMax.y, t),  // 陨石坑半径随编号递增
                floor = Mathf.Lerp(-1.2f, -0.2f, t + prng.ValueBiasLower(0.3f)),  // 陨石坑的底部高度
                smoothness = Mathf.Lerp(smoothMinMax.x, smoothMinMax.y, 1 - t)  // 陨石坑的平滑度
            };
        }
        // 将顶点数据传递到 ComputeShader
        vertexBuffer.SetData(vertices);
        craterBuffer.SetData(craters);

        // 获取内核句柄
        int kernelHandle = craterComputeShader.FindKernel("CSMain");
        Debug.Log("kernelHandle:" + kernelHandle);
        // 设置 Shader 参数
        craterComputeShader.SetBuffer(kernelHandle, "vertices", vertexBuffer);
        craterComputeShader.SetBuffer(kernelHandle, "heights", heightBuffer);
        craterComputeShader.SetBuffer(kernelHandle, "craters", craterBuffer);
        craterComputeShader.SetInt("numCraters", craterNum);
        craterComputeShader.SetFloat("floorHeight", floorHeight);
        craterComputeShader.SetFloat("rimSteepness", rimSteepness);
        craterComputeShader.SetFloat("rimWidth", rimWidth);
        craterComputeShader.SetFloat("smoothFactor", smoothFactor);

        // 执行计算
        int threadGroups = Mathf.CeilToInt(numVertices / 512.0f);  // 计算需要的线程组数量
        craterComputeShader.Dispatch(kernelHandle, threadGroups,1, 1);

        // 从 GPU 中读取计算结果
        float[] heights = new float[numVertices];
        heightBuffer.GetData(heights);

/*        for (int i = 0; i < numVertices; i++)
        {
            Debug.Log(heights[i]);
        }*/

        // 将新的顶点数据赋回 Mesh
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // 清理 ComputeBuffer
        vertexBuffer.Release();
        heightBuffer.Release();
        craterBuffer.Release();
        return heights;
    }
}
[System.Serializable]
public struct Crater
{
    public Vector3 centre;
    public float radius;
    public float floor;
    public float smoothness;
}