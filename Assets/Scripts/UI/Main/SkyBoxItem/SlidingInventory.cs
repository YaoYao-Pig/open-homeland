using UnityEngine;

public class SlidingInventory : MonoBehaviour
{
    [SerializeField] private RectTransform inventoryPanel; // ��Ʒ���� RectTransform
    [SerializeField] private float slideSpeed = 500f; // �����ٶ�
    [SerializeField] private float edgeThreshold = 10f; // ��꿿����Ļ��Ե�ľ���
    [SerializeField] private float heightOffset = 10f;
    [SerializeField] private RectTransform iconPrefab;
    private Vector2 hiddenPosition; // ����ʱ��λ��
    private Vector2 shownPosition;  // ��ʾʱ��λ��
    private bool isSlidingOut = false; // �Ƿ����ڻ���

   

    void Start()
    {
        // ����ʱ��λ�ã���Ļ�·�����ȫ���ɼ���
        hiddenPosition = inventoryPanel.position;

        // ��ʾʱ��λ�ã�������Ļ�ײ���
        shownPosition = new Vector2(0, inventoryPanel.position.y + iconPrefab.rect.height+ heightOffset);

    }

    void Update()
    {
        // ��ȡ����λ��
        Vector3 mousePosition = Input.mousePosition;

        // �ж�����Ƿ�ӽ���Ļ�ײ�
        if (mousePosition.y <= edgeThreshold)
        {
            isSlidingOut = true; // ���ӽ��ײ�����ʼ����
        }
        else if (mousePosition.y > inventoryPanel.position.y+ inventoryPanel.rect.height)
        {
            isSlidingOut = false; // ����뿪һ����Χ����ʼ����
        }



        // Ŀ��λ�ã�����������
        Vector2 targetPosition = isSlidingOut ? shownPosition : hiddenPosition;

        // ʹ�ò�ֵ������Lerp��ƽ���ƶ���Ŀ��λ��
        inventoryPanel.anchoredPosition = Vector2.Lerp(inventoryPanel.anchoredPosition, targetPosition, Time.deltaTime * slideSpeed);
    }

}
