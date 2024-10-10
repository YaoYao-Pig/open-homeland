using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f; // 移动速度
    public float mouseSensitivity = 2f; // 鼠标灵敏度
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleMouseLook();
    }

    void HandleMovement()
    {
        // 获取输入
        float moveHorizontal = Input.GetAxis("Horizontal"); // A/D 或 左/右箭头
        float moveVertical = Input.GetAxis("Vertical"); // W/S 或 上/下箭头

        // 计算移动方向
        Vector3 forward = transform.forward; // 玩家面朝的方向
        Vector3 right = transform.right; // 玩家右侧方向

        // 计算实际的移动向量
        Vector3 movement = (right * moveHorizontal + forward * moveVertical).normalized;

        // 将移动向量投影到球面
        movement = Vector3.ProjectOnPlane(movement, transform.up).normalized;

        // 移动玩家
        rb.AddForce(movement * moveSpeed);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

        // 只旋转玩家角色
        transform.Rotate(Vector3.up * mouseX);
    }
}
