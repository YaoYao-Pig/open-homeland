using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
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
    void Start()
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

            Vector3 dir = Vector3.Normalize(transform.position-parentTransform.position);
            Vector3 offset = dir * distance;

            lineRenderer.SetPosition(1, parentTransform.position+offset); // ���ڵ�λ��
        }
    }

    void Update()
    {
/*        // ��ÿ֡����������λ��
        if (lineRenderer.positionCount == 2)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.parent.position);
        }*/
    }
}
