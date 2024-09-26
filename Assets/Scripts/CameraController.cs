using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;     // ����ƶ��ٶ�
    public float lookSpeed = 2f;      // �����ת�ٶ�
    public float scrollSpeed = 2f;    // ��������ٶ�
    private float rotationX = 0f;     // ��¼��ת�Ƕ�

    private void RotateCamera()
    {
        // ��ȡ��������
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        // ��ת���
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // ����������ת�Ƕ�
        transform.localRotation = Quaternion.Euler(rotationX, transform.localEulerAngles.y + mouseX, 0);

    }

    private void MoveCamera()
    {
        // ��ȡ WASD ��������
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        // �ƶ����
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        transform.position += move;
    }

    private void ScaleCamera()
    {
        // ���ţ���ǰ�����
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
