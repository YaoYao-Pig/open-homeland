using UnityEngine;
using XCharts;
using UnityEngine.EventSystems;
using XCharts.Runtime;

public class MouseUIAction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float zoomScale = 1.5f; // 放大的比例
    public float offset = 0.5f;

    private Vector3 position2Center;
    private GameObject centerPoint;
    private Vector3 originalScale;
    private Vector3 originalLocalPosition;
    private void Start()
    {
        // 存储初始缩放
        originalScale = gameObject.transform.localScale;
        originalLocalPosition = gameObject.transform.localPosition;

        centerPoint = GameObject.Find("chartCenterPoint");


        if (centerPoint == null) throw new System.Exception("MouseUIAction:centerPoint can't get");
        position2Center = centerPoint.transform.localPosition - originalLocalPosition;
        Debug.Log("position2Center:" + position2Center.ToString());
    }


    
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 鼠标进入时直接放大
        gameObject.transform.localScale = originalScale * zoomScale;
        gameObject.transform.localPosition += offset*position2Center;
        transform.SetAsLastSibling();

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 鼠标离开时恢复原始大小
        gameObject.transform.localScale = originalScale;
        gameObject.transform.localPosition = originalLocalPosition;
    }
}
