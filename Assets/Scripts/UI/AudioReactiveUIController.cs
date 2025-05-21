using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AudioReactiveUIController : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioAnalyzer audioAnalyzer;
    [SerializeField] private bool enableAudioReaction = true;
    
    [Header("Circle Settings")]
    [SerializeField] private Color circleColor = new Color(0.5f, 0.7f, 1.0f, 1.0f);
    [SerializeField] private Color pulseColor = new Color(0.7f, 0.9f, 1.0f, 1.0f);
    [Range(0, 1)]
    [SerializeField] private float radius = 0.8f;
    [Range(0, 0.5f)]
    [SerializeField] private float edgeSoftness = 0.05f;
    
    [Header("Wave Settings")]
    [Range(1, 20)]
    [SerializeField] private float waveCount = 8f;
    [Range(0, 0.2f)]
    [SerializeField] private float waveAmplitude = 0.05f;
    [Range(0, 10)]
    [SerializeField] private float waveSpeed = 2f;
    
    [Header("Audio Reaction Settings")]
    [Range(0, 5)]
    [SerializeField] private float pulseIntensity = 1f;
    [SerializeField] private float smoothingSpeed = 5.0f;
    
    [Header("Rotation Settings")]
    [Range(-10, 10)]
    [SerializeField] private float rotationSpeed = 1f;
    
    [Header("Frequency Bands")]
    [SerializeField] private int primaryBand = 1; // 低频段，通常对应节拍
    [SerializeField] private int secondaryBand = 3; // 中频段
    [SerializeField] private int tertiaryBand = 6; // 高频段
    
    // 当前音频反应值
    private float currentAudioPulse = 0f;
    private float targetAudioPulse = 0f;
    
    // 组件引用
    private Image image;
    private Material material;
    
    private void Awake()
    {
        // 获取组件引用
        image = GetComponent<Image>();
        
        // 创建材质实例
        material = new Material(Shader.Find("UI/AudioReactiveCircle"));
        image.material = material;
    }
    
    private void Start()
    {
        // 如果没有指定音频分析器，尝试在场景中查找
        if (audioAnalyzer == null)
        {
            audioAnalyzer = FindObjectOfType<AudioAnalyzer>();
            if (audioAnalyzer == null)
            {
                Debug.LogWarning("AudioReactiveUIController: No AudioAnalyzer found. Audio reaction disabled.");
                enableAudioReaction = false;
            }
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
        if (material != null)
        {
            // 设置基本属性
            material.SetColor("_CircleColor", circleColor);
            material.SetColor("_PulseColor", pulseColor);
            material.SetFloat("_ColorBlend", enableAudioReaction ? currentAudioPulse * 0.5f : 0);
            
            // 设置圆形属性
            material.SetFloat("_Radius", radius);
            material.SetFloat("_EdgeSoftness", edgeSoftness);
            
            // 设置波动属性
            material.SetFloat("_WaveCount", waveCount);
            material.SetFloat("_WaveAmplitude", waveAmplitude);
            material.SetFloat("_WaveSpeed", waveSpeed);
            
            // 设置音频反应属性
            material.SetFloat("_AudioPulse", enableAudioReaction ? currentAudioPulse : 0);
            material.SetFloat("_PulseIntensity", pulseIntensity);
            
            // 设置旋转属性
            material.SetFloat("_RotationSpeed", rotationSpeed);
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
    
    // 公开方法，用于设置波动数量
    public void SetWaveCount(float count)
    {
        waveCount = count;
        UpdateMaterialProperties();
    }
    
    // 公开方法，用于设置脉冲强度
    public void SetPulseIntensity(float intensity)
    {
        pulseIntensity = intensity;
        UpdateMaterialProperties();
    }
    
    // 公开方法，用于设置旋转速度
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
        UpdateMaterialProperties();
    }
}
