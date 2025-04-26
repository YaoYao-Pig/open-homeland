using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AuroraController : MonoBehaviour
{
    [Header("Aurora Settings")]
    [SerializeField] private Material auroraMaterial;
    [SerializeField] private float minIntensity = 0.2f;
    [SerializeField] private float maxIntensity = 2.0f;
    [SerializeField] private Color lowActivityColor = new Color(0, 0.5f, 1f);
    [SerializeField] private Color highActivityColor = new Color(0, 1f, 0.5f);
    [SerializeField] private float updateInterval = 1f; // 更新频率（秒）
    
    [Header("Activity Data")]
    [SerializeField, Range(0f, 1f)] private float activityLevel = 0.5f; // 当前活跃度
    [SerializeField] private bool useRealData = false; // 是否使用真实数据
    [SerializeField] private bool animateInEditor = true; // 在编辑器中是否动画
    
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private float lastUpdateTime = 0f;
    
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        
        // 创建默认材质
        if (auroraMaterial == null)
        {
            auroraMaterial = new Material(Shader.Find("Planet/AuroraEffect"));
        }
        
        // 应用材质
        meshRenderer.material = auroraMaterial;
    }
    
    private void Start()
    {
        // 创建极光网格
        if (meshFilter.sharedMesh == null)
        {
            meshFilter.sharedMesh = CreateAuroraMesh();
        }
        
        // 初始更新
        UpdateAurora(activityLevel);
    }
    
    private void Update()
    {
        // 在编辑器中模拟活跃度变化
        if (!Application.isPlaying || (Application.isPlaying && !useRealData))
        {
            if (animateInEditor && Time.time - lastUpdateTime > updateInterval)
            {
                lastUpdateTime = Time.time;
                float newActivity = Mathf.PingPong(Time.time * 0.1f, 1f);
                UpdateAurora(newActivity);
            }
        }
    }
    
    // 更新极光基于活跃度
    public void UpdateAurora(float activityLevel) // 0-1范围
    {
        this.activityLevel = Mathf.Clamp01(activityLevel);
        
        if (auroraMaterial != null)
        {
            // 设置强度
            float intensity = Mathf.Lerp(minIntensity, maxIntensity, activityLevel);
            auroraMaterial.SetFloat("_Intensity", intensity);
            
            // 设置颜色
            Color mainColor = Color.Lerp(lowActivityColor, highActivityColor, activityLevel);
            auroraMaterial.SetColor("_Color", mainColor);
            
            // 设置速度
            float speed = Mathf.Lerp(0.5f, 2.0f, activityLevel);
            auroraMaterial.SetFloat("_Speed", speed);
        }
    }
    
    // 创建极光网格（半球形）
    private Mesh CreateAuroraMesh()
    {
        Mesh mesh = new Mesh();
        
        int segments = 32;
        int rings = 16;
        float radius = 1.05f; // 略大于行星半径
        
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        
        // 添加顶点
        for (int ring = 0; ring <= rings; ring++)
        {
            float phi = Mathf.PI * 0.5f * ring / rings; // 从0到PI/2（半球）
            float y = Mathf.Cos(phi);
            float ringRadius = Mathf.Sin(phi);
            
            for (int segment = 0; segment <= segments; segment++)
            {
                float theta = 2 * Mathf.PI * segment / segments;
                float x = ringRadius * Mathf.Cos(theta);
                float z = ringRadius * Mathf.Sin(theta);
                
                vertices.Add(new Vector3(x, y, z) * radius);
                uvs.Add(new Vector2((float)segment / segments, (float)ring / rings));
            }
        }
        
        // 添加三角形
        for (int ring = 0; ring < rings; ring++)
        {
            int ringVertexOffset = ring * (segments + 1);
            int nextRingVertexOffset = (ring + 1) * (segments + 1);
            
            for (int segment = 0; segment < segments; segment++)
            {
                triangles.Add(ringVertexOffset + segment);
                triangles.Add(nextRingVertexOffset + segment);
                triangles.Add(ringVertexOffset + segment + 1);
                
                triangles.Add(ringVertexOffset + segment + 1);
                triangles.Add(nextRingVertexOffset + segment);
                triangles.Add(nextRingVertexOffset + segment + 1);
            }
        }
        
        // 设置网格数据
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        
        return mesh;
    }
    
    // 调整极光大小以匹配行星
    public void AdjustToMatchPlanet(Transform planetTransform)
    {
        if (planetTransform != null)
        {
            transform.localScale = planetTransform.localScale;
        }
    }
}
