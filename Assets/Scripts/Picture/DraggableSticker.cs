using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableSticker : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector2 initialPosition;
    private RectTransform rectTransform;
    private Canvas canvas;

    public Vector2 stickerPosition; // �洢��ֽ��λ�ã������ڽ�ͼʱ�ο�

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>(); // ��ȡ Canvas
        initialPosition = rectTransform.position; // ��¼��ʼλ��
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out localPosition);
        rectTransform.localPosition = localPosition; // ������ֽλ��
        stickerPosition = localPosition; // ���´洢����ֽλ��
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // ��������ק����ʱ����һЩ����
    }
}
