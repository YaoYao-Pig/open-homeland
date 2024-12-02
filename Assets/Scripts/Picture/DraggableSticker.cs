using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableSticker : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector2 initialPosition;
    private RectTransform rectTransform;
    private Canvas canvas;

    public Vector2 stickerPosition; // 存储贴纸的位置，用于在截图时参考

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>(); // 获取 Canvas
        initialPosition = rectTransform.position; // 记录初始位置
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out localPosition);
        rectTransform.localPosition = localPosition; // 更新贴纸位置
        stickerPosition = localPosition; // 更新存储的贴纸位置
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 可以在拖拽结束时进行一些操作
    }
}
