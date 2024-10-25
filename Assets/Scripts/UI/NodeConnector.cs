using UnityEngine;
using System.Collections.Generic;

public class NodeConnector : MonoBehaviour
{
    // 字典存储节点名称与相连节点名称列表
    private int segmentCount = 4;
    private float alpha = 0.5f;
    private void Start()
    {
        IniteEdgeLine();
        //IniteParentLine();
    }
    private void IniteParentLine()
    {
        GameObject fromNode = GetComponentInParent<GameObject>();
        DrawLine(transform.position, fromNode.transform.position);
    }

    private void IniteEdgeLine()
    {
        GameObject fromNode = WorldManager.Instance.userNodeInstanceDic[gameObject.name];
        if(WorldManager.Instance.repoEdgeNameDic.ContainsKey(gameObject.name))
        // 遍历 WorldManager.Instance.repoEdgeNameDic 进行连线
            foreach (var connectedNode in WorldManager.Instance.repoEdgeNameDic[gameObject.name])
            {
                if (WorldManager.Instance.userNodeInstanceDic.ContainsKey(connectedNode)){
                    GameObject toNode = WorldManager.Instance.userNodeInstanceDic[connectedNode];
                    DrawLine(fromNode.transform.position, toNode.transform.position);
                }

            }
        else
        {
            ;
        }
    }

    // 绘制连线的函数
    private void DrawLine(Vector3 start, Vector3 end)
    {
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
        // 设置线条的起点和终点
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        lineRenderer.useWorldSpace = true;

            // 设置线条宽度
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        Color startColor = lineRenderer.startColor;
        startColor.a = alpha;
        Color endColor = lineRenderer.endColor;
        endColor.a = alpha;
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;

        lineRenderer.positionCount = segmentCount;

        // 计算每个细分点的位置
        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);  // t 介于 0 和 1 之间，用于插值
            Vector3 pointOnLine = Vector3.Lerp(start, end, t);  // 插值计算每个点的位置
            lineRenderer.SetPosition(i, pointOnLine);  // 设置每个插值点
        }

            // 创建颜色渐变
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.yellow, 1.0f) }, // 从蓝色渐变到黄色
            new GradientAlphaKey[] { new GradientAlphaKey(0, 0.0f), new GradientAlphaKey(alpha, 1.0f) } // 设置 Alpha 为不透明
        );

        // 将渐变应用到 LineRenderer
        lineRenderer.colorGradient = gradient;

        // 设置材质
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }
}
