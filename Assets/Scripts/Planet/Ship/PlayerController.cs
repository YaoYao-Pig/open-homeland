using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f; // �ƶ��ٶ�
    public float mouseSensitivity = 2f; // ���������
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
        // ��ȡ����
        float moveHorizontal = Input.GetAxis("Horizontal"); // A/D �� ��/�Ҽ�ͷ
        float moveVertical = Input.GetAxis("Vertical"); // W/S �� ��/�¼�ͷ

        // �����ƶ�����
        Vector3 forward = transform.forward; // ����泯�ķ���
        Vector3 right = transform.right; // ����Ҳ෽��

        // ����ʵ�ʵ��ƶ�����
        Vector3 movement = (right * moveHorizontal + forward * moveVertical).normalized;

        // ���ƶ�����ͶӰ������
        movement = Vector3.ProjectOnPlane(movement, transform.up).normalized;

        // �ƶ����
        rb.AddForce(movement * moveSpeed);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

        // ֻ��ת��ҽ�ɫ
        transform.Rotate(Vector3.up * mouseX);
    }
}
