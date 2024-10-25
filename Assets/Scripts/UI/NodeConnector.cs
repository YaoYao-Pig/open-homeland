using UnityEngine;
using System.Collections.Generic;

public class NodeConnector : MonoBehaviour
{
    // �ֵ�洢�ڵ������������ڵ������б�
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
        // ���� WorldManager.Instance.repoEdgeNameDic ��������
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

    // �������ߵĺ���
    private void DrawLine(Vector3 start, Vector3 end)
    {
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
        // ���������������յ�
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        lineRenderer.useWorldSpace = true;

            // �����������
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        Color startColor = lineRenderer.startColor;
        startColor.a = alpha;
        Color endColor = lineRenderer.endColor;
        endColor.a = alpha;
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;

        lineRenderer.positionCount = segmentCount;

        // ����ÿ��ϸ�ֵ��λ��
        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);  // t ���� 0 �� 1 ֮�䣬���ڲ�ֵ
            Vector3 pointOnLine = Vector3.Lerp(start, end, t);  // ��ֵ����ÿ�����λ��
            lineRenderer.SetPosition(i, pointOnLine);  // ����ÿ����ֵ��
        }

            // ������ɫ����
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.yellow, 1.0f) }, // ����ɫ���䵽��ɫ
            new GradientAlphaKey[] { new GradientAlphaKey(0, 0.0f), new GradientAlphaKey(alpha, 1.0f) } // ���� Alpha Ϊ��͸��
        );

        // ������Ӧ�õ� LineRenderer
        lineRenderer.colorGradient = gradient;

        // ���ò���
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }
}
