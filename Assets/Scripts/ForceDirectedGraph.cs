using UnityEngine;
using System.Collections.Generic;

public class ForceDirectedGraph : MonoBehaviour
{
    public List<Transform> parentNodes; // 父节点列表
    public float attractionForce = 1.0f; // 吸引力强度
    public float repulsionForce = 100.0f; // 排斥力强度
    public float minimumDistance = 2.0f; // 父节点之间的最小距离
    public float maximumDistance = 5.0f; // 父节点之间的最大距离
    public float childMinimumDistance = 1.0f; // 子节点与父节点之间的最小距离
    public float childMaximumDistance = 3.0f; // 子节点与父节点之间的最大距离
    public int childrenPerParent = 5; // 每个父节点的子节点数量
    public float childRadius = 2.0f; // 子节点与父节点之间的半径




    private void Awake()
    {
        parentNodes = new List<Transform>();
      
    }

    private void Start()
    {
        // 初始化父节点
        foreach (GameObject p in WorldManager.Instance.repoObjectList)
        {
            parentNodes.Add(p.transform);
            ArrangeChildrenAroundParent(p.transform); // 初始化子节点
        }

        // 调整位置直到达到平衡状态
        AdjustPositions();
    }

    private void ArrangeChildrenAroundParent(Transform parent)
    {
        // 确保子节点均匀分布
        for (int i = 0; i < childrenPerParent; i++)
        {
            float angle = i * Mathf.PI * 2 / childrenPerParent; // 计算每个子节点的角度
            Vector3 childPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * childRadius;
            Transform child = new GameObject("Child").transform; // 创建子节点
            child.position = parent.position + childPosition; // 设置子节点位置
            child.parent = parent; // 将子节点设置为父节点的子物体
        }
    }

    private void AdjustPositions()
    {
        const int maxIterations = 10; // 最大迭代次数
        int iterationCount = 0;

        bool isStable = false;

        while (!isStable && iterationCount < maxIterations)
        {
            isStable = true;
            iterationCount++;

            foreach (var parent in parentNodes)
            {
                Vector3 force = Vector3.zero;

                // 计算与子节点的吸引力和排斥力
                foreach (Transform child in parent)
                {
                    Vector3 direction = parent.position - child.position;
                    float distance = direction.magnitude;

                    // 控制子节点与父节点之间的距离
                    if (distance < childMinimumDistance)
                    {
                        float repulsion = repulsionForce / (distance * distance);
                        force += direction.normalized * repulsion; // 向外的排斥力
                    }
                    else if (distance > childMaximumDistance)
                    {
                        float attraction = attractionForce / (distance * distance);
                        force += direction.normalized * attraction; // 向内的吸引力
                    }
                }

                // 计算与其他父节点的排斥力
                foreach (var other in parentNodes)
                {
                    if (other != parent)
                    {
                        Vector3 direction = parent.position - other.position;
                        float distance = direction.magnitude;

                        // 添加排斥力以保持最小和最大距离
                        if (distance < minimumDistance)
                        {
                            float repulsion = repulsionForce / (distance * distance);
                            force += direction.normalized * repulsion; // 向外的排斥力
                        }
                        else if (distance > maximumDistance)
                        {
                            float attraction = attractionForce / (distance * distance);
                            force += direction.normalized * attraction; // 向内的吸引力
                        }
                    }
                }

                // 更新位置
                parent.position += force * Time.deltaTime;

                // 检查稳定性
                if (force.magnitude > 0.01f)
                {
                    isStable = false; // 如果力仍然存在，则不稳定
                }
            }
        }

    }
}
