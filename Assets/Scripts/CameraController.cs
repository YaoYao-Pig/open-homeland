using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;     // 相机移动速度
    public float lookSpeed = 2f;      // 相机旋转速度
    public float scrollSpeed = 2f;    // 相机缩放速度
    private float rotationX = 0f;     // 记录旋转角度

    private void RotateCamera()
    {
        // 获取鼠标的输入
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        // 旋转相机
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // 限制上下旋转角度
        transform.localRotation = Quaternion.Euler(rotationX, transform.localEulerAngles.y + mouseX, 0);

    }

    private void MoveCamera()
    {
        // 获取 WASD 键的输入
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        // 移动相机
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        transform.position += move;
    }

    private void ScaleCamera()
    {
        // 缩放（向前或向后）
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            transform.position += transform.forward * Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        }
    }

    void Update()
    {
        RotateCamera();
        MoveCamera();
        ScaleCamera();

    }
}
