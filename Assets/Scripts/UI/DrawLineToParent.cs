using UnityEngine;

public class DrawLineToParent : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private NodeComponent nodeComponent;
    private NodeComponent parentNodeComponet;

    private Color lineStartColor;
    private Color lineEndColor;
    private float lineWidth=0.1f;
    private float distance = 2.5f;//�������������յ������λ��
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        nodeComponent = GetComponent<UserNodeComponent>();
        parentNodeComponet = GetComponentInParent<RepoNodeComponent>();


        lineStartColor = Color.red;
        lineEndColor = Color.blue;
    }

    public void Draw() 
    {
        lineRenderer.positionCount = 2; // �����ĵ�����
        lineRenderer.material = nodeComponent.lineMaterial;
        lineRenderer.startColor = lineStartColor;
        lineRenderer.endColor = lineEndColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        //distance = parentNodeComponet.node.radius;


        // ��ȡ���ڵ�
        Transform parentTransform = transform.parent;

        if (parentTransform != null)
        {
            // ������������ʼ����յ�
            lineRenderer.SetPosition(0, transform.position); // �ӽڵ�λ��

            Vector3 dir = Vector3.Normalize(transform.position - parentTransform.position);
            Vector3 offset = dir * distance;

            lineRenderer.SetPosition(1, parentTransform.position + offset); // ���ڵ�λ��
        }
    }

    public void ReFresh()
    {
        // �������������ΪĬ��״̬
        lineRenderer.positionCount = 0; // ���õ�����Ϊ0���������

        // ��Ҳ���������������������ԣ������Ҫ�Ļ�
        lineRenderer.startColor = Color.clear; // ������ʼ��ɫΪ͸��
        lineRenderer.endColor = Color.clear; // ���ý�����ɫΪ͸��
    }

}
