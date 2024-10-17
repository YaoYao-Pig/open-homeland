using UnityEngine;
using System.Collections.Generic;

public class ForceDirectedGraph : MonoBehaviour
{
    public List<Transform> parentNodes; // ���ڵ��б�
    public float attractionForce = 1.0f; // ������ǿ��
    public float repulsionForce = 100.0f; // �ų���ǿ��
    public float minimumDistance = 2.0f; // ���ڵ�֮�����С����
    public float maximumDistance = 5.0f; // ���ڵ�֮���������
    public float childMinimumDistance = 1.0f; // �ӽڵ��븸�ڵ�֮�����С����
    public float childMaximumDistance = 3.0f; // �ӽڵ��븸�ڵ�֮���������
    public int childrenPerParent = 5; // ÿ�����ڵ���ӽڵ�����
    public float childRadius = 2.0f; // �ӽڵ��븸�ڵ�֮��İ뾶




    private void Awake()
    {
        parentNodes = new List<Transform>();
      
    }

    private void Start()
    {
        // ��ʼ�����ڵ�
        foreach (GameObject p in WorldManager.Instance.repoObjectList)
        {
            parentNodes.Add(p.transform);
            ArrangeChildrenAroundParent(p.transform); // ��ʼ���ӽڵ�
        }

        // ����λ��ֱ���ﵽƽ��״̬
        AdjustPositions();
    }

    private void ArrangeChildrenAroundParent(Transform parent)
    {
        // ȷ���ӽڵ���ȷֲ�
        for (int i = 0; i < childrenPerParent; i++)
        {
            float angle = i * Mathf.PI * 2 / childrenPerParent; // ����ÿ���ӽڵ�ĽǶ�
            Vector3 childPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * childRadius;
            Transform child = new GameObject("Child").transform; // �����ӽڵ�
            child.position = parent.position + childPosition; // �����ӽڵ�λ��
            child.parent = parent; // ���ӽڵ�����Ϊ���ڵ��������
        }
    }

    private void AdjustPositions()
    {
        const int maxIterations = 10; // ����������
        int iterationCount = 0;

        bool isStable = false;

        while (!isStable && iterationCount < maxIterations)
        {
            isStable = true;
            iterationCount++;

            foreach (var parent in parentNodes)
            {
                Vector3 force = Vector3.zero;

                // �������ӽڵ�����������ų���
                foreach (Transform child in parent)
                {
                    Vector3 direction = parent.position - child.position;
                    float distance = direction.magnitude;

                    // �����ӽڵ��븸�ڵ�֮��ľ���
                    if (distance < childMinimumDistance)
                    {
                        float repulsion = repulsionForce / (distance * distance);
                        force += direction.normalized * repulsion; // ������ų���
                    }
                    else if (distance > childMaximumDistance)
                    {
                        float attraction = attractionForce / (distance * distance);
                        force += direction.normalized * attraction; // ���ڵ�������
                    }
                }

                // �������������ڵ���ų���
                foreach (var other in parentNodes)
                {
                    if (other != parent)
                    {
                        Vector3 direction = parent.position - other.position;
                        float distance = direction.magnitude;

                        // ����ų����Ա�����С��������
                        if (distance < minimumDistance)
                        {
                            float repulsion = repulsionForce / (distance * distance);
                            force += direction.normalized * repulsion; // ������ų���
                        }
                        else if (distance > maximumDistance)
                        {
                            float attraction = attractionForce / (distance * distance);
                            force += direction.normalized * attraction; // ���ڵ�������
                        }
                    }
                }

                // ����λ��
                parent.position += force * Time.deltaTime;

                // ����ȶ���
                if (force.magnitude > 0.01f)
                {
                    isStable = false; // �������Ȼ���ڣ����ȶ�
                }
            }
        }

    }
}
