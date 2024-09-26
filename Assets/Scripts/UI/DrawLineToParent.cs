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
        
        lineRenderer.positionCount = 2; // 线条的点数量
        lineRenderer.material = nodeComponent.lineMaterial;
        lineRenderer.startColor = lineStartColor;
        lineRenderer.endColor = lineEndColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        



        // 获取父节点
        Transform parentTransform = transform.parent;

        if (parentTransform != null)
        {
            // 设置线条的起始点和终点
            lineRenderer.SetPosition(0, transform.position); // 子节点位置
            lineRenderer.SetPosition(1, parentTransform.position); // 父节点位置
        }
    }

    void Update()
    {
        // 在每帧更新线条的位置
        if (lineRenderer.positionCount == 2)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.parent.position);
        }
    }
}
