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
    private float distance = 2.5f;//用来限制线条终点的最终位置
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
        
        lineRenderer.positionCount = 2; // 线条的点数量
        lineRenderer.material = nodeComponent.lineMaterial;
        lineRenderer.startColor = lineStartColor;
        lineRenderer.endColor = lineEndColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        //distance = parentNodeComponet.node.radius;


        // 获取父节点
        Transform parentTransform = transform.parent;

        if (parentTransform != null)
        {
            // 设置线条的起始点和终点
            lineRenderer.SetPosition(0, transform.position); // 子节点位置

            Vector3 dir = Vector3.Normalize(transform.position-parentTransform.position);
            Vector3 offset = dir * distance;

            lineRenderer.SetPosition(1, parentTransform.position+offset); // 父节点位置
        }
    }

    void Update()
    {
/*        // 在每帧更新线条的位置
        if (lineRenderer.positionCount == 2)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.parent.position);
        }*/
    }
}
