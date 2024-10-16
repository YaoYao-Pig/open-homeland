using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public static CameraController Instance { get; private set; } // 单例实例

    public float moveSpeed = 10f;     // 相机移动速度
    public float lookSpeed = 2f;      // 相机旋转速度
    public float scrollSpeed = 2f;    // 相机缩放速度
    private float rotationX = 0f;     // 记录旋转角度



    public float moveDuration = 2f; // 移动时间
    private bool canMove = true; // 控制输入的开关
    public float offset = 10f;
    public float elapsedTime=0;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    public SceneTransition sceneTransition;

    private void Awake()
    {
        // 确保只存在一个实例
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




        canMove = false; // 禁用输入
        startPosition = transform.position;
        targetPosition = spherePosition + new Vector3(0, 1, -3); // 根据需要调整摄像机位置

        Vector3 dir = Vector3.Normalize(startPosition - targetPosition);
        targetPosition += dir * (offset+radius);
        startRotation = transform.rotation;
        targetRotation = Quaternion.LookRotation(spherePosition - startPosition); // 目标旋转朝向球体
        elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            // 平滑移动
            transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / moveDuration));

            // 平滑旋转
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, (elapsedTime / moveDuration));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // 确保最终位置正确
        transform.rotation = targetRotation; // 确保最终旋转正确




        //加载场景
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
        transform.position = startPosition; // 确保最终位置正确
        transform.rotation = targetRotation;
        canMove = true; // 恢复输入
        Cursor.lockState = CursorLockMode.None;
    }
}
