using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawLineToParent : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private NodeComponent nodeComponent;

    public Color lineStartColor;
    public Color lineEndColor;
    public float lineWidth=0.1f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        nodeComponent = GetComponent<UserNodeComponent>();
        lineStartColor = Color.red;
        lineEndColor = Color.blue;
    }
    void Start()
    {
        
        lineRenderer.positionCount = 2; // �����ĵ�����
        lineRenderer.material = nodeComponent.lineMaterial;
        lineRenderer.startColor = lineStartColor;
        lineRenderer.endColor = lineEndColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        



        // ��ȡ���ڵ�
        Transform parentTransform = transform.parent;

        if (parentTransform != null)
        {
            // ������������ʼ����յ�
            lineRenderer.SetPosition(0, transform.position); // �ӽڵ�λ��
            lineRenderer.SetPosition(1, parentTransform.position); // ���ڵ�λ��
        }
    }

    void Update()
    {
        // ��ÿ֡����������λ��
        if (lineRenderer.positionCount == 2)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.parent.position);
        }
    }
}
