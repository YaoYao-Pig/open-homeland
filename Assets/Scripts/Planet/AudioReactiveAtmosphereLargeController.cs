using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AudioReactiveAtmosphereLargeController : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioAnalyzer audioAnalyzer;
    [SerializeField] private bool enableAudioReaction = true;
    
    [Header("Atmosphere Settings")]
    [SerializeField] private Color atmosphereColor = new Color(0.5f, 0.7f, 1.0f, 1.0f);
    [SerializeField] private Color pulseColor = new Color(0.7f, 0.9f, 1.0f, 1.0f);
    [Range(0, 1)]
    [SerializeField] private float atmosphereAlpha = 0.5f;
    [Range(0, 0.5f)]
    [SerializeField] private float atmosphereHeight = 0.1f;
    [Range(0, 10)]
    [SerializeField] private float atmosphereFalloff = 3f;
    [Range(0, 20)]
    [SerializeField] private float atmosphereRimPower = 5f;
    
    [Header("Light Settings")]
    [SerializeField] private Transform sunTransform;
    [SerializeField] private bool useSunPosition = true;
    [SerializeField] private Vector3 customLightDirection = new Vector3(0, 1, 0);
    [Range(0, 10)]
    [SerializeField] private float sunIntensity = 1f;
    
    [Header("Scattering Settings")]
    [Range(0, 10)]
    [SerializeField] private float rayleighCoefficient = 1f;
    [Range(0, 10)]
    [SerializeField] private float mieCoefficient = 0.5f;
    [Range(0, 1)]
    [SerializeField] private float mieDirectionalG = 0.8f;
    
    [Header("Audio Reaction Settings")]
    [Range(0, 5)]
    [SerializeField] private float pulseIntensity = 1f;
    [Range(0, 10)]
    [SerializeField] private float waveSpeed = 2f;
    [Range(0, 20)]
    [SerializeField] private float waveFrequency = 10f;
    [Range(0, 10)]
    [SerializeField] private float waveAmplitude = 2.0f;
    [SerializeField] private float smoothingSpeed = 5.0f;
    
    [Header("Scale Settings")]
    [Range(1, 2000)]
    [SerializeField] private float scaleFactor = 1000f; // 默认为1000，适合大型星球
    
    [Header("Frequency Bands")]
    [SerializeField] private int primaryBand = 1; // 低频段，通常对应节拍
    [SerializeField] private int secondaryBand = 3; // 中频段
    [SerializeField] private int tertiaryBand = 6; // 高频段
    
    // 当前音频反应值
    private float currentAudioPulse = 0f;
    private float targetAudioPulse = 0f;
    
    // 材质引用
    private Material atmosphereMaterial;
    private MeshRenderer meshRenderer;
    private Planet planet;
    
    private void Awake()
    {
        // 获取引用
        meshRenderer = GetComponent<MeshRenderer>();
        planet = GetComponentInParent<Planet>();
        
        // 创建新的材质实例
        atmosphereMaterial = new Material(Shader.Find("Planet/AudioReactiveAtmosphereLarge"));
        meshRenderer.material = atmosphereMaterial;
        
        // 如果没有指定太阳，尝试查找一个
        if (sunTransform == null && useSunPosition)
        {
            GameObject sunObject = GameObject.Find("Directional Light");
            if (sunObject != null)
            {
                sunTransform = sunObject.transform;
            }
        }
    }
    
    private void Start()
    {
        // 如果没有指定音频分析器，尝试在场景中查找
        if (audioAnalyzer == null)
        {
            audioAnalyzer = FindObjectOfType<AudioAnalyzer>();
            if (audioAnalyzer == null)
            {
                Debug.LogWarning("AudioReactiveAtmosphereLargeController: No AudioAnalyzer found. Audio reaction disabled.");
                enableAudioReaction = false;
            }
        }
        
        // 创建大气层网格（如果需要）
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter.sharedMesh == null)
        {
            meshFilter.sharedMesh = CreateSphereMesh(1f, 128, 128); // 使用更高的分辨率以获得更好的波动效果
        }
        
        // 设置初始材质属性
        UpdateMaterialProperties();
    }
    
    private void Update()
    {
        // 更新音频反应
        if (enableAudioReaction && audioAnalyzer != null)
        {
            UpdateAudioReaction();
        }
        
        // 更新材质属性
        UpdateMaterialProperties();
    }
    
    private void UpdateAudioReaction()
    {
        // 获取主要频段的值
        float primaryValue = audioAnalyzer.GetBufferedBandValue(primaryBand);
        
        // 获取次要频段的值，用于添加一些变化
        float secondaryValue = audioAnalyzer.GetBufferedBandValue(secondaryBand) * 0.5f;
        float tertiaryValue = audioAnalyzer.GetBufferedBandValue(tertiaryBand) * 0.25f;
        
        // 计算目标反应值
        targetAudioPulse = (primaryValue + secondaryValue + tertiaryValue);
        
        // 平滑过渡到目标值
        currentAudioPulse = Mathf.Lerp(currentAudioPulse, targetAudioPulse, Time.deltaTime * smoothingSpeed);
        
        // 检测节拍，用于更强烈的脉冲效果
        if (audioAnalyzer.IsBeat)
        {
            // 节拍时增加反应强度
            currentAudioPulse = Mathf.Min(currentAudioPulse + 0.3f, 1.0f);
        }
    }
    
    private void UpdateMaterialProperties()
    {
        if (atmosphereMaterial != null)
        {
            // 设置基本属性
            atmosphereMaterial.SetColor("_AtmoColor", atmosphereColor);
            atmosphereMaterial.SetColor("_PulseColor", pulseColor);
            atmosphereMaterial.SetFloat("_ColorBlend", enableAudioReaction ? currentAudioPulse * 0.5f : 0);
            
            // 设置大气层属性
            atmosphereMaterial.SetFloat("_AtmosphereAlpha", atmosphereAlpha);
            atmosphereMaterial.SetFloat("_AtmosphereHeight", atmosphereHeight);
            atmosphereMaterial.SetFloat("_AtmosphereFalloff", atmosphereFalloff);
            atmosphereMaterial.SetFloat("_AtmosphereRimPower", atmosphereRimPower);
            
            // 设置光照属性
            if (useSunPosition && sunTransform != null)
            {
                atmosphereMaterial.SetVector("_SunDir", sunTransform.forward);
            }
            else
            {
                atmosphereMaterial.SetVector("_SunDir", customLightDirection.normalized);
            }
            atmosphereMaterial.SetFloat("_SunIntensity", sunIntensity);
            
            // 设置散射属性
            atmosphereMaterial.SetFloat("_RayleighCoefficient", rayleighCoefficient);
            atmosphereMaterial.SetFloat("_MieCoefficient", mieCoefficient);
            atmosphereMaterial.SetFloat("_MieDirectionalG", mieDirectionalG);
            
            // 设置音频反应属性
            atmosphereMaterial.SetFloat("_AudioPulse", enableAudioReaction ? currentAudioPulse : 0);
            atmosphereMaterial.SetFloat("_PulseIntensity", pulseIntensity);
            atmosphereMaterial.SetFloat("_WaveSpeed", waveSpeed);
            atmosphereMaterial.SetFloat("_WaveFrequency", waveFrequency);
            atmosphereMaterial.SetFloat("_WaveAmplitude", waveAmplitude);
            
            // 设置缩放因子
            atmosphereMaterial.SetFloat("_ScaleFactor", scaleFactor);
        }
    }
    
    // 创建球体网格的辅助方法
    private Mesh CreateSphereMesh(float radius, int latitudeSegments, int longitudeSegments)
    {
        Mesh mesh = new Mesh();
        
        // 创建顶点
        Vector3[] vertices = new Vector3[(latitudeSegments + 1) * (longitudeSegments + 1)];
        Vector2[] uv = new Vector2[(latitudeSegments + 1) * (longitudeSegments + 1)];
        
        for (int lat = 0; lat <= latitudeSegments; lat++)
        {
            float theta = lat * Mathf.PI / latitudeSegments;
            float sinTheta = Mathf.Sin(theta);
            float cosTheta = Mathf.Cos(theta);
            
            for (int lon = 0; lon <= longitudeSegments; lon++)
            {
                float phi = lon * 2 * Mathf.PI / longitudeSegments;
                float sinPhi = Mathf.Sin(phi);
                float cosPhi = Mathf.Cos(phi);
                
                float x = cosPhi * sinTheta;
                float y = cosTheta;
                float z = sinPhi * sinTheta;
                
                int index2 = lat * (longitudeSegments + 1) + lon;
                vertices[index2] = new Vector3(x, y, z) * radius;
                uv[index2] = new Vector2((float)lon / longitudeSegments, (float)lat / latitudeSegments);
            }
        }
        
        // 创建三角形
        int[] triangles = new int[latitudeSegments * longitudeSegments * 6];
        int index = 0;
        for (int lat = 0; lat < latitudeSegments; lat++)
        {
            for (int lon = 0; lon < longitudeSegments; lon++)
            {
                int current = lat * (longitudeSegments + 1) + lon;
                int next = current + 1;
                int bottom = current + (longitudeSegments + 1);
                int bottomNext = bottom + 1;
                
                triangles[index++] = current;
                triangles[index++] = bottomNext;
                triangles[index++] = bottom;
                
                triangles[index++] = current;
                triangles[index++] = next;
                triangles[index++] = bottomNext;
            }
        }
        
        // 创建法线（从中心向外指）
        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            normals[i] = vertices[i].normalized;
        }
        
        // 分配给网格
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;
        
        return mesh;
    }
    
    // 根据行星属性调整大气层的方法
    public void AdjustToMatchPlanet()
    {
        if (planet != null)
        {
            if (planet.shapeSettings == null)
            {
                if (PlanetManager.Instance.currentScence == ScenceType.Main)
                {
                    planet.shapeSettings = Resources.Load<ShapeSettings>("Settings/Shape_main");
                }
                else if (PlanetManager.Instance.currentScence == ScenceType.Start)
                {
                    planet.shapeSettings = Resources.Load<ShapeSettings>("Settings/Shape_start");
                }
            }
            // 根据行星半径缩放大气层
            float planetRadius = planet.shapeSettings.planetRadius;
            transform.localScale = Vector3.one * planetRadius;
            
            // 更新缩放因子以匹配行星大小
            scaleFactor = planetRadius;
            UpdateMaterialProperties();
        }
    }
    
    // 公开方法，用于从外部启用/禁用音频反应
    public void SetAudioReaction(bool enable)
    {
        enableAudioReaction = enable;
        
        // 如果禁用，重置音频脉冲
        if (!enable)
        {
            currentAudioPulse = 0f;
            targetAudioPulse = 0f;
            UpdateMaterialProperties();
        }
    }
    
    // 公开方法，用于设置波动振幅
    public void SetWaveAmplitude(float amplitude)
    {
        waveAmplitude = amplitude;
        UpdateMaterialProperties();
    }
    
    // 公开方法，用于设置波动频率
    public void SetWaveFrequency(float frequency)
    {
        waveFrequency = frequency;
        UpdateMaterialProperties();
    }
    
    // 公开方法，用于设置脉冲强度
    public void SetPulseIntensity(float intensity)
    {
        pulseIntensity = intensity;
        UpdateMaterialProperties();
    }
}
