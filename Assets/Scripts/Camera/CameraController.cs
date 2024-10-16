using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public static CameraController Instance { get; private set; } // ����ʵ��

    public float moveSpeed = 10f;     // ����ƶ��ٶ�
    public float lookSpeed = 2f;      // �����ת�ٶ�
    public float scrollSpeed = 2f;    // ��������ٶ�
    private float rotationX = 0f;     // ��¼��ת�Ƕ�



    public float moveDuration = 2f; // �ƶ�ʱ��
    private bool canMove = true; // ��������Ŀ���
    public float offset = 10f;
    public float elapsedTime=0;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    public SceneTransition sceneTransition;

    private void Awake()
    {
        // ȷ��ֻ����һ��ʵ��
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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


    private void CameraControl()
    {
        if (canMove)
        {
            RotateCamera();
            MoveCamera();
            ScaleCamera();
        }

    }

    void Update()
    {

        CameraControl();

    }



    
    public void MoveCameraToSphereAndLoadScence(Vector3 spherePosition,float radius)
    {
        if (canMove)
        {
            StartCoroutine(MoveCameraAndLoadScence(spherePosition,WorldInfo.detailScenceName, radius));
        }
    }
    private IEnumerator MoveCameraAndLoadScence(Vector3 spherePosition,string scenceName,float radius)
    {




        canMove = false; // ��������
        startPosition = transform.position;
        targetPosition = spherePosition + new Vector3(0, 1, -3); // ������Ҫ���������λ��

        Vector3 dir = Vector3.Normalize(startPosition - targetPosition);
        targetPosition += dir * (offset+radius);
        startRotation = transform.rotation;
        targetRotation = Quaternion.LookRotation(spherePosition - startPosition); // Ŀ����ת��������
        elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            // ƽ���ƶ�
            transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / moveDuration));

            // ƽ����ת
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, (elapsedTime / moveDuration));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // ȷ������λ����ȷ
        transform.rotation = targetRotation; // ȷ��������ת��ȷ




        //���س���
        sceneTransition = FindObjectOfType<SceneTransition>();
        sceneTransition.FadeToScene(scenceName);
        //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenceName);

    }
    public IEnumerator backDetail()
    {
        Cursor.lockState = CursorLockMode.Locked;
        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(targetPosition, startPosition, (elapsedTime)/moveDuration);
            transform.rotation = Quaternion.Lerp(targetRotation, startRotation, (elapsedTime) / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = startPosition; // ȷ������λ����ȷ
        transform.rotation = targetRotation;
        canMove = true; // �ָ�����
        Cursor.lockState = CursorLockMode.None;
    }
}
