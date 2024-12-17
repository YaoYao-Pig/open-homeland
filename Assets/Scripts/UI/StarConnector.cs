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
        
        // ���������������յ�
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        lineRenderer.useWorldSpace = true;


        lineRenderer.positionCount = segmentCount;

        // ����ÿ��ϸ�ֵ��λ��
        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);  // t ���� 0 �� 1 ֮�䣬���ڲ�ֵ
            Vector3 pointOnLine = Vector3.Lerp(start, end, t);  // ��ֵ����ÿ�����λ��
            lineRenderer.SetPosition(i, pointOnLine);  // ����ÿ����ֵ��
        }

    }
}
