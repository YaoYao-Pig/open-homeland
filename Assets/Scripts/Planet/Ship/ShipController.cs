using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public static ShipController Instance { get; private set; } // ����ʵ��
    public GameObject centerPlanet;
    public ShapeSettings shapeSettings;

    public float moveSpeed = 1f;     
    public float lookSpeed = 2f;      
    public float scrollSpeed = 2f;    
    private float rotationX = 0f;
    private enum TravelStage
    {
        View, Travel
    }
    TravelStage stage = TravelStage.View;

    private Camera mainCamera;

    public float moveDuration = 2f; // �ƶ�ʱ��
    private bool canMove = true; // ��������Ŀ���
    public float offset = 10f;
    public float elapsedTime = 0;
    private float distance = 0.0f;
    private float preDistance = 0.0f;
    private float maxDistance = 0.0f;


    private int currentScale=0;
    private float[] scaleFactors = new float[] { 0.0f, 0.05f, 0.1f, 0.15f, 0.2f, 0.25f, 0.3f, 0.35f, 0.4f, 0.45f, 0.5f };
    private float CaculateDistance()
    {
        Vector3 radiusOffset = Vector3.Normalize(gameObject.transform.position - centerPlanet.transform.position);
        return Vector3.Distance(centerPlanet.transform.position + radiusOffset * shapeSettings.planetRadius,
              gameObject.transform.position);
    }



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

        distance = CaculateDistance();
        maxDistance = distance;
        preDistance = distance;
        //shapeSettings.planetRadius = 1.7f;
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
          RotateCamera();
          MoveCamera();
          ScaleCamera();

    }


    int t = 0;
    private void SwitchToTravell()
    {
        distance = CaculateDistance();
        maxDistance = maxDistance > distance ? maxDistance : distance;
        Debug.Log("Distance:" + distance);
        if (stage == TravelStage.View && distance <= 2000)
        {
            if (mainCamera == null) mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

            //������
            gameObject.transform.SetParent(centerPlanet.transform);

            moveSpeed = 200;
            
            stage = TravelStage.Travel;
        }

        else if (stage == TravelStage.Travel&& distance > 1200)
        {
            gameObject.transform.SetParent(null);

            stage = TravelStage.View;
        }
        else if(t==0&&stage == TravelStage.Travel&& distance <= 1000)
        {
            moveSpeed = 100f;
            Debug.Log("Less Than 500");
            float rayDistance = 1000f;
            // �������ߵ���ʼ��ͷ���
            Vector3 origin = transform.position; // �������λ�ÿ�ʼ
            Vector3 direction = transform.forward; // ���ߵķ���Ϊ�����ǰ��
                t++;
                Ray ray = new Ray(origin, direction);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, rayDistance))
                {
                    // ��⵽��ײ
                    if (hit.collider.CompareTag("Face"))
                    {
                        string name = hit.collider.name;
                        Debug.Log("Hit target: " + name);
                        // �����������������߼������������廥��
                        Debug.DrawLine(origin, hit.point, Color.red); // �ڳ����л�������

                        int index =int.Parse(name.Split("_")[1]);

                        centerPlanet.GetComponent<Planet>().GeneratePlanet(index,256);
                        centerPlanet.GetComponent<Planet>().GenerateMeshCollider();


                }

                }


            //centerPlanet.GetComponent<Planet>().GeneratePlanet(resolutions);
            //centerPlanet.GetComponent<Planet>().GenerateMeshCollider();
        }
        
    }

    private float maxRadius = 100.0f;
    private void CaculateRadius()
    {
        int preScale = currentScale;
        if (distance >= 8) currentScale =0;
        else if (distance >= 7.5) currentScale = 1;
        else if (distance >= 7) currentScale = 2;
        else if (distance >= 6.5) currentScale = 3;
        else if (distance >= 6) currentScale = 4;
        else if (distance >= 5.5) currentScale = 5;
        else if (distance >= 5) currentScale = 6;
        else if (distance >= 4.5) currentScale = 7;
        else if (distance >= 4) currentScale = 8;

        if (preScale != currentScale)
        {
            Debug.Log("Distance:" + distance);
            Debug.Log("Radius:" + shapeSettings.planetRadius);
            shapeSettings.planetRadius = Mathf.Lerp(shapeSettings.planetRadius,maxRadius, scaleFactors[currentScale]);
            Vector3 bias = Vector3.Normalize(centerPlanet.transform.position - gameObject.transform.position);
            centerPlanet.transform.localPosition += bias * shapeSettings.planetRadius;
            centerPlanet.GetComponent<Planet>().GeneratePlanet();
        }

    }

    void Update()
    {

        CameraControl();
        SwitchToTravell();
        //CaculateRadius();

    }
}
