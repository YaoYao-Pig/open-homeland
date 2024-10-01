using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanEffect : MonoBehaviour
{

    public float startScanRange = 0;
    public float maxScanRange = 20;
    public float scanWidth = 3;
    public float scanSpeed = 1;
    public Color headColor;
    public Color trailColor;

    private Material material;


    private bool isInScane = false;
    private Vector3 centerPos;
    private float scanRadius;

    private Camera mCamera;
    public Shader shader;

    private IEnumerator scanHandler = null;
    private void OnEnable()
    {
        mCamera = GetComponent<Camera>();
        material = new Material(shader);

        scanRadius = startScanRange;
        mCamera.depthTextureMode = DepthTextureMode.Depth;
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                centerPos = hit.point;
                Scan();
            }
        }



    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material != null&&isInScane)
        {
            material.SetVector("_ScanCenterPos", centerPos);//定义扫描线中心点
            material.SetFloat("_CurrentScanRadius", scanRadius);//当前扫到哪里了
            material.SetFloat("_ScanWidth", scanWidth);
            material.SetColor("_HeadColor", headColor);

            RaycastCornerBlit(source,destination,material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    private void RaycastCornerBlit(RenderTexture source,RenderTexture dest,Material mat)
    {
        float cameraFar = mCamera.farClipPlane;
        float camerFov = mCamera.fieldOfView;
        float camerAspect = mCamera.aspect;

        float fovWHalf = camerFov / 2.0f;

        //从相机位置向右或者向上（都是走半程）
        Vector3 toRight = mCamera.transform.right * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camerAspect;
        Vector3 toTop = mCamera.transform.up * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);


        //求四个顶点的向量
        Vector3 topLeft = mCamera.transform.forward - toRight + toTop;
        float cameraScale = topLeft.magnitude * cameraFar;
        topLeft.Normalize();  
        topLeft *= cameraScale;

        Vector3 topRight = (mCamera.transform.forward + toRight + toTop);
        topRight.Normalize();
        topRight *= cameraScale;

        Vector3 bottomRight = (mCamera.transform.forward + toRight - toTop);
        bottomRight.Normalize();
        bottomRight *= cameraScale;

        Vector3 bottomLeft = (mCamera.transform.forward - toRight - toTop);
        bottomLeft.Normalize();
        bottomLeft *= cameraScale;


        RenderTexture.active = dest;

        mat.SetTexture("_MainTex", source);

        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetPass(0);

        GL.Begin(GL.QUADS);

        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.MultiTexCoord(1, bottomLeft);
        GL.Vertex3(0.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.MultiTexCoord(1, bottomRight);
        GL.Vertex3(1.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.MultiTexCoord(1, topRight);
        GL.Vertex3(1.0f, 1.0f, 0.0f);

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.MultiTexCoord(1, topLeft);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();
    }


    private IEnumerator ScanCoroutine()
    {
        isInScane = true;
        while (scanRadius <= maxScanRange)
        {
            scanRadius += scanSpeed;
            yield return new WaitForSecondsRealtime(0.01f);
        }
        scanRadius = maxScanRange;
        isInScane = false;
    }


    private void Scan()
    {
        scanHandler = ScanCoroutine();
        StartCoroutine(scanHandler);
    }
}
