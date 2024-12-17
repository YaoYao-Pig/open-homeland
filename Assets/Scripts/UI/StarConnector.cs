using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarConnector : MonoBehaviour
{

    private LineRenderer lineRenderer;
    [SerializeField] StarController starController;
    private int segmentCount = 4;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }


    public void IniteEdgeLine(GameObject fromNode,GameObject toNode)
    {
            DrawLine(fromNode, fromNode.transform.position, toNode.transform.position);
    }

    private void DrawLine(GameObject from, Vector3 start, Vector3 end)
    {
        
        // 设置线条的起点和终点
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        lineRenderer.useWorldSpace = true;


        lineRenderer.positionCount = segmentCount;

        // 计算每个细分点的位置
        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);  // t 介于 0 和 1 之间，用于插值
            Vector3 pointOnLine = Vector3.Lerp(start, end, t);  // 插值计算每个点的位置
            lineRenderer.SetPosition(i, pointOnLine);  // 设置每个插值点
        }

    }
}
