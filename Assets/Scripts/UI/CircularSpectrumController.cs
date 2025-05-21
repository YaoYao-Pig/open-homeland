using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CircularSpectrumController : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioAnalyzer audioAnalyzer;
    [SerializeField] private AudioDataTextureGenerator audioDataGenerator;
    
    [Header("Spectrum Settings")]
    [Range(8, 128)]
    [SerializeField] private int barCount = 64;
    [Range(0, 1)]
    [SerializeField] private float minBarHeight = 0.1f;
    [Range(0, 1)]
    [SerializeField] private float maxBarHeight = 0.5f;
    [Range(0, 1)]
    [SerializeField] private float barWidth = 0.7f;
    [Range(0, 1)]
    [SerializeField] private float innerRadius = 0.3f;
    
    [Header("Color Settings")]
    [SerializeField] private Color baseColor = new Color(0.2f, 0.4f, 1.0f, 1.0f);
    [SerializeField] private Color peakColor = new Color(1.0f, 0.5f, 0.2f, 1.0f);
    [SerializeField] private Color bgColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
    
    [Header("Animation Settings")]
    [Range(-1, 1)]
    [SerializeField] private float rotationSpeed = 0.1f;
    
    // 组件引用
    private Image image;
    private Material material;
    
    private void Awake()
    {
        // 获取组件引用
        image = GetComponent<Image>();
        
        // 创建材质实例
        material = new Material(Shader.Find("UI/CircularSpectrum"));
        image.material = material;
    }
    
    private void Start()
    {
        // 如果没有指定音频分析器，尝试在场景中查找
        if (audioAnalyzer == null)
        {
            audioAnalyzer = FindObjectOfType<AudioAnalyzer>();
        }
        
        // 如果没有指定音频数据生成器，尝试在场景中查找或创建
        if (audioDataGenerator == null)
        {
            audioDataGenerator = FindObjectOfType<AudioDataTextureGenerator>();
            if (audioDataGenerator == null && audioAnalyzer != null)
            {
                GameObject generatorObj = new GameObject("AudioDataGenerator");
                audioDataGenerator = generatorObj.AddComponent<AudioDataTextureGenerator>();
                audioDataGenerator.transform.SetParent(audioAnalyzer.transform);
            }
        }
        
        // 设置初始材质属性
        UpdateMaterialProperties();
    }
    
    private void Update()
    {
        // 更新材质属性
        UpdateMaterialProperties();
    }
    
    private void UpdateMaterialProperties()
    {
        if (material != null)
        {
            // 设置频谱属性
            material.SetFloat("_BarCount", barCount);
            material.SetFloat("_MinBarHeight", minBarHeight);
            material.SetFloat("_MaxBarHeight", maxBarHeight);
            material.SetFloat("_BarWidth", barWidth);
            material.SetFloat("_InnerRadius", innerRadius);
            
            // 设置颜色属性
            material.SetColor("_BaseColor", baseColor);
            material.SetColor("_PeakColor", peakColor);
            material.SetColor("_BgColor", bgColor);
            
            // 设置动画属性
            material.SetFloat("_RotationSpeed", rotationSpeed);
            
            // 设置音频数据纹理
            if (audioDataGenerator != null && audioDataGenerator.AudioDataTexture != null)
            {
                material.SetTexture("_AudioData", audioDataGenerator.AudioDataTexture);
            }
        }
    }
    
    // 公开方法，用于设置条形数量
    public void SetBarCount(int count)
    {
        barCount = Mathf.Clamp(count, 8, 128);
        UpdateMaterialProperties();
    }
    
    // 公开方法，用于设置条形高度范围
    public void SetBarHeightRange(float min, float max)
    {
        minBarHeight = Mathf.Clamp01(min);
        maxBarHeight = Mathf.Clamp01(max);
        UpdateMaterialProperties();
    }
    
    // 公开方法，用于设置条形宽度
    public void SetBarWidth(float width)
    {
        barWidth = Mathf.Clamp01(width);
        UpdateMaterialProperties();
    }
    
    // 公开方法，用于设置内半径
    public void SetInnerRadius(float radius)
    {
        innerRadius = Mathf.Clamp01(radius);
        UpdateMaterialProperties();
    }
    
    // 公开方法，用于设置颜色
    public void SetColors(Color baseCol, Color peakCol, Color bgCol)
    {
        baseColor = baseCol;
        peakColor = peakCol;
        bgColor = bgCol;
        UpdateMaterialProperties();
    }
    
    // 公开方法，用于设置旋转速度
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = Mathf.Clamp(speed, -1f, 1f);
        UpdateMaterialProperties();
    }
}
