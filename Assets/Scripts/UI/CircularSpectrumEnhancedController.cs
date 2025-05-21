using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CircularSpectrumEnhancedController : MonoBehaviour
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
    [SerializeField] private Color glowColor = new Color(1.0f, 0.8f, 0.4f, 1.0f);
    [Range(0, 2)]
    [SerializeField] private float glowIntensity = 0.5f;
    
    [Header("Animation Settings")]
    [Range(-1, 1)]
    [SerializeField] private float rotationSpeed = 0.1f;
    [Range(0, 10)]
    [SerializeField] private float pulseSpeed = 5f;
    [Range(0, 0.2f)]
    [SerializeField] private float pulseAmount = 0.05f;
    
    [Header("Visual Enhancement")]
    [Range(0, 1)]
    [SerializeField] private float barRounding = 0.2f;
    [Range(0, 0.5f)]
    [SerializeField] private float barGap = 0.1f;
    [Range(0, 0.2f)]
    [SerializeField] private float peakWidth = 0.05f;
    [Range(0, 0.2f)]
    [SerializeField] private float peakHeight = 0.05f;
    [Range(1, 3)]
    [SerializeField] private float responsiveness = 1.5f;
    
    [Header("Beat Detection")]
    [SerializeField] private bool reactToBeat = true;
    [SerializeField] private float beatIntensityMultiplier = 1.5f;
    [SerializeField] private float beatGlowMultiplier = 2.0f;
    [SerializeField] private float beatDecaySpeed = 5.0f;
    
    // 组件引用
    private Image image;
    private Material material;
    
    // 节拍反应
    private float currentBeatIntensity = 0f;
    
    private void Awake()
    {
        // 获取组件引用
        image = GetComponent<Image>();
        
        // 创建材质实例
        material = new Material(Shader.Find("UI/CircularSpectrumEnhanced"));
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
        // 更新节拍反应
        if (reactToBeat && audioAnalyzer != null)
        {
            UpdateBeatReaction();
        }
        
        // 更新材质属性
        UpdateMaterialProperties();
    }
    
    private void UpdateBeatReaction()
    {
        // 检测节拍
        if (audioAnalyzer.IsBeat)
        {
            // 节拍时增加强度
            currentBeatIntensity = beatIntensityMultiplier;
        }
        else
        {
            // 逐渐衰减
            currentBeatIntensity = Mathf.Lerp(currentBeatIntensity, 0, Time.deltaTime * beatDecaySpeed);
        }
    }
    
    private void UpdateMaterialProperties()
    {
        if (material != null)
        {
            // 设置频谱属性
            material.SetFloat("_BarCount", barCount);
            material.SetFloat("_MinBarHeight", minBarHeight);
            material.SetFloat("_MaxBarHeight", maxBarHeight + (currentBeatIntensity * 0.1f)); // 节拍时增加高度
            material.SetFloat("_BarWidth", barWidth);
            material.SetFloat("_InnerRadius", innerRadius);
            
            // 设置颜色属性
            material.SetColor("_BaseColor", baseColor);
            material.SetColor("_PeakColor", peakColor);
            material.SetColor("_BgColor", bgColor);
            material.SetColor("_GlowColor", glowColor);
            material.SetFloat("_GlowIntensity", glowIntensity + (currentBeatIntensity * beatGlowMultiplier)); // 节拍时增加发光
            
            // 设置动画属性
            material.SetFloat("_RotationSpeed", rotationSpeed);
            material.SetFloat("_PulseSpeed", pulseSpeed);
            material.SetFloat("_PulseAmount", pulseAmount + (currentBeatIntensity * 0.05f)); // 节拍时增加脉冲
            
            // 设置视觉增强属性
            material.SetFloat("_BarRounding", barRounding);
            material.SetFloat("_BarGap", barGap);
            material.SetFloat("_PeakWidth", peakWidth);
            material.SetFloat("_PeakHeight", peakHeight);
            material.SetFloat("_Responsiveness", responsiveness);
            
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
    public void SetColors(Color baseCol, Color peakCol, Color bgCol, Color glowCol)
    {
        baseColor = baseCol;
        peakColor = peakCol;
        bgColor = bgCol;
        glowColor = glowCol;
        UpdateMaterialProperties();
    }
    
    // 公开方法，用于设置发光强度
    public void SetGlowIntensity(float intensity)
    {
        glowIntensity = Mathf.Clamp(intensity, 0f, 2f);
        UpdateMaterialProperties();
    }
    
    // 公开方法，用于设置旋转速度
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = Mathf.Clamp(speed, -1f, 1f);
        UpdateMaterialProperties();
    }
    
    // 公开方法，用于设置脉冲参数
    public void SetPulseParameters(float speed, float amount)
    {
        pulseSpeed = Mathf.Clamp(speed, 0f, 10f);
        pulseAmount = Mathf.Clamp(amount, 0f, 0.2f);
        UpdateMaterialProperties();
    }
    
    // 公开方法，用于设置视觉增强参数
    public void SetVisualEnhancement(float rounding, float gap, float peakW, float peakH)
    {
        barRounding = Mathf.Clamp01(rounding);
        barGap = Mathf.Clamp(gap, 0f, 0.5f);
        peakWidth = Mathf.Clamp(peakW, 0f, 0.2f);
        peakHeight = Mathf.Clamp(peakH, 0f, 0.2f);
        UpdateMaterialProperties();
    }
    
    // 公开方法，用于设置响应性
    public void SetResponsiveness(float value)
    {
        responsiveness = Mathf.Clamp(value, 1f, 3f);
        UpdateMaterialProperties();
    }
    
    // 公开方法，用于设置节拍反应参数
    public void SetBeatReaction(bool enable, float intensityMult, float glowMult, float decaySpeed)
    {
        reactToBeat = enable;
        beatIntensityMultiplier = Mathf.Max(0f, intensityMult);
        beatGlowMultiplier = Mathf.Max(0f, glowMult);
        beatDecaySpeed = Mathf.Max(0.1f, decaySpeed);
    }
}
