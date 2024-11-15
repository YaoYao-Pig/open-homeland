using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class StarCompute : MonoBehaviour
{
    public ComputeShader craterComputeShader;  // ���� Compute Shader
    //public int craterNum = 1;
    public float rimSteepness = 10.0f;  // ���ζ��Ͷ�
    public float rimWidth = 0.2f;  // ���ο��
    public Vector2 craterSizeMinMax = new Vector2(0.01f, 0.1f);
    public float smoothFactor = 0.1f;  // ƽ����
    public Vector2 smoothMinMax = new Vector2 (0.4f, 1.5f);
    public float floorHeight = 0.0f;  // ����߶�
    public int craterSeed=0;
    public int masterSeed=1;
    [Range(0, 1)]
    public float sizeDistribution = 0.6f;


    private ComputeBuffer vertexBuffer;  // �������ݻ�����
    private ComputeBuffer heightBuffer;  // �߶����ݻ�����
    private ComputeBuffer craterBuffer;  // ��ʯ�����ݻ�����

    public float[] Initialize(Mesh mesh,int craterNum)
    {
        // ��ȡԭʼ��������
        Vector3[] vertices = mesh.vertices;
        int numVertices = vertices.Length;


        Random.InitState(craterSeed + masterSeed);
        PRNG prng = new PRNG(masterSeed);

        // ���� ComputeBuffer ���ڴ洢�������ݺ͸߶�����
        vertexBuffer = new ComputeBuffer(numVertices, sizeof(float) * 3);  // float3 ��С
        heightBuffer = new ComputeBuffer(numVertices, sizeof(float));  // �洢������ĸ߶�
        craterBuffer = new ComputeBuffer(craterNum, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Crater))); 
        // ������ʯ�����ݲ����ݸ� Shader
        Crater[] craters = new Crater[craterNum];  // ������ 5 ����ʯ��
        for (int i = 0; i < craterNum; i++)
        {
            float t = prng.ValueBiasLower(sizeDistribution);
            craters[i] = new Crater
            {
                centre = Random.onUnitSphere,  // ����ÿ����ʯ�Ӽ�� 5 ��λ
                radius = Mathf.Lerp(craterSizeMinMax.x, craterSizeMinMax.y, t),  // ��ʯ�Ӱ뾶���ŵ���
                floor = Mathf.Lerp(-1.2f, -0.2f, t + prng.ValueBiasLower(0.3f)),  // ��ʯ�ӵĵײ��߶�
                smoothness = Mathf.Lerp(smoothMinMax.x, smoothMinMax.y, 1 - t)  // ��ʯ�ӵ�ƽ����
            };
        }
        // ���������ݴ��ݵ� ComputeShader
        vertexBuffer.SetData(vertices);
        craterBuffer.SetData(craters);

        // ��ȡ�ں˾��
        int kernelHandle = craterComputeShader.FindKernel("CSMain");
        Debug.Log("kernelHandle:" + kernelHandle);
        // ���� Shader ����
        craterComputeShader.SetBuffer(kernelHandle, "vertices", vertexBuffer);
        craterComputeShader.SetBuffer(kernelHandle, "heights", heightBuffer);
        craterComputeShader.SetBuffer(kernelHandle, "craters", craterBuffer);
        craterComputeShader.SetInt("numCraters", craterNum);
        craterComputeShader.SetFloat("floorHeight", floorHeight);
        craterComputeShader.SetFloat("rimSteepness", rimSteepness);
        craterComputeShader.SetFloat("rimWidth", rimWidth);
        craterComputeShader.SetFloat("smoothFactor", smoothFactor);

        // ִ�м���
        int threadGroups = Mathf.CeilToInt(numVertices / 512.0f);  // ������Ҫ���߳�������
        craterComputeShader.Dispatch(kernelHandle, threadGroups,1, 1);

        // �� GPU �ж�ȡ������
        float[] heights = new float[numVertices];
        heightBuffer.GetData(heights);

/*        for (int i = 0; i < numVertices; i++)
        {
            Debug.Log(heights[i]);
        }*/

        // ���µĶ������ݸ��� Mesh
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // ���� ComputeBuffer
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