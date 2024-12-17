using UnityEngine;

public class SlidingInventory : MonoBehaviour
{
    [SerializeField] private RectTransform inventoryPanel; // 物品栏的 RectTransform
    [SerializeField] private float slideSpeed = 500f; // 滑动速度
    [SerializeField] private float edgeThreshold = 10f; // 鼠标靠近屏幕边缘的距离
    [SerializeField] private float heightOffset = 10f;
    [SerializeField] private RectTransform iconPrefab;
    private Vector2 hiddenPosition; // 隐藏时的位置
    private Vector2 shownPosition;  // 显示时的位置
    private bool isSlidingOut = false; // 是否正在滑出

   

    void Start()
    {
        // 隐藏时的位置（屏幕下方，完全不可见）
        hiddenPosition = inventoryPanel.position;

        // 显示时的位置（紧贴屏幕底部）
        shownPosition = new Vector2(0, inventoryPanel.position.y + iconPrefab.rect.height+ heightOffset);

    }

    void Update()
    {
        // 获取鼠标的位置
        Vector3 mousePosition = Input.mousePosition;

        // 判断鼠标是否接近屏幕底部
        if (mousePosition.y <= edgeThreshold)
        {
            isSlidingOut = true; // 鼠标接近底部，开始滑出
        }
        else if (mousePosition.y > inventoryPanel.position.y+ inventoryPanel.rect.height)
        {
            isSlidingOut = false; // 鼠标离开一定范围，开始隐藏
        }



        // 目标位置：滑出或隐藏
        Vector2 targetPosition = isSlidingOut ? shownPosition : hiddenPosition;

        // 使用插值函数（Lerp）平滑移动到目标位置
        inventoryPanel.anchoredPosition = Vector2.Lerp(inventoryPanel.anchoredPosition, targetPosition, Time.deltaTime * slideSpeed);
    }

}
