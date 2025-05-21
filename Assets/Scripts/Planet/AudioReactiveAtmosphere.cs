using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AtmosphereGlowController))]
public class AudioReactiveAtmosphere : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioAnalyzer audioAnalyzer;
    
    [Header("Reaction Settings")]
    [SerializeField] private bool enableAudioReaction = true;
    [SerializeField] private float reactionIntensity = 1.0f;
    [SerializeField] private float smoothingSpeed = 5.0f;
    
    [Header("Glow Settings")]
    [SerializeField] private bool reactGlowFactor = true;
    [SerializeField] private float baseGlowFactor = 5.0f;
    [SerializeField] private float maxGlowFactorChange = 3.0f;
    
    [Header("Color Settings")]
    [SerializeField] private bool reactColor = true;
    [SerializeField] private Color baseColor = new Color(0.5f, 0.7f, 1.0f, 1.0f);
    [SerializeField] private Color pulseColor = new Color(0.7f, 0.9f, 1.0f, 1.0f);
    [SerializeField] private float colorReactionIntensity = 0.5f;
    
    [Header("Size Settings")]
    [SerializeField] private bool reactSize = true;
    [SerializeField] private float baseSizeMultiplier = 1.0f;
    [SerializeField] private float maxSizeChange = 0.1f;
    
    [Header("Frequency Bands")]
    [SerializeField] private int primaryBand = 1; // 低频段，通常对应节拍
    [SerializeField] private int secondaryBand = 3; // 中频段
    [SerializeField] private int tertiaryBand = 6; // 高频段
    
    // 当前反应值
    private float currentReaction = 0f;
    private float targetReaction = 0f;
    private float currentSizeMultiplier = 1.0f;
    private Color currentColor;
    
    // 组件引用
    private AtmosphereGlowController atmosphereController;
    
    private void Awake()
    {
        atmosphereController = GetComponent<AtmosphereGlowController>();
        currentColor = baseColor;
    }
    
    private void Start()
    {
        // 如果没有指定音频分析器，尝试在场景中查找
        if (audioAnalyzer == null)
        {
            audioAnalyzer = FindObjectOfType<AudioAnalyzer>();
            if (audioAnalyzer == null)
            {
                Debug.LogWarning("AudioReactiveAtmosphere: No AudioAnalyzer found. Audio reaction disabled.");
                enableAudioReaction = false;
            }
        }
    }
    
    private void Update()
    {
        if (!enableAudioReaction || audioAnalyzer == null)
            return;
        
        // 获取音频反应值
        UpdateAudioReaction();
        
        // 应用效果
        if (reactGlowFactor)
            UpdateGlowFactor();
        
        if (reactColor)
            UpdateColor();
        
        if (reactSize)
            UpdateSize();
    }
    
    private void UpdateAudioReaction()
    {
        // 获取主要频段的值
        float primaryValue = audioAnalyzer.GetBufferedBandValue(primaryBand);
        
        // 获取次要频段的值，用于添加一些变化
        float secondaryValue = audioAnalyzer.GetBufferedBandValue(secondaryBand) * 0.5f;
        float tertiaryValue = audioAnalyzer.GetBufferedBandValue(tertiaryBand) * 0.25f;
        
        // 计算目标反应值
        targetReaction = (primaryValue + secondaryValue + tertiaryValue) * reactionIntensity;
        
        // 平滑过渡到目标值
        currentReaction = Mathf.Lerp(currentReaction, targetReaction, Time.deltaTime * smoothingSpeed);
        
        // 检测节拍，用于更强烈的脉冲效果
        if (audioAnalyzer.IsBeat)
        {
            // 节拍时增加反应强度
            currentReaction = Mathf.Min(currentReaction + 0.3f, 1.0f);
        }
    }
    
    private void UpdateGlowFactor()
    {
        // 计算新的发光因子
        float newGlowFactor = baseGlowFactor + (currentReaction * maxGlowFactorChange);
        
        // 通过反射设置私有字段
        var glowFactorField = atmosphereController.GetType().GetField("glowFactor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (glowFactorField != null)
        {
            glowFactorField.SetValue(atmosphereController, newGlowFactor);
        }
        else
        {
            // 尝试设置公共属性（如果存在）
            var property = atmosphereController.GetType().GetProperty("GlowFactor");
            if (property != null && property.CanWrite)
            {
                property.SetValue(atmosphereController, newGlowFactor);
            }
        }
    }
    
    private void UpdateColor()
    {
        // 在基础颜色和脉冲颜色之间插值
        Color targetColor = Color.Lerp(baseColor, pulseColor, currentReaction * colorReactionIntensity);
        
        // 平滑过渡颜色
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * smoothingSpeed);
        
        // 通过反射设置私有字段
        var colorField = atmosphereController.GetType().GetField("atmosphereColor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (colorField != null)
        {
            colorField.SetValue(atmosphereController, currentColor);
        }
        else
        {
            // 尝试设置公共属性（如果存在）
            var property = atmosphereController.GetType().GetProperty("AtmosphereColor");
            if (property != null && property.CanWrite)
            {
                property.SetValue(atmosphereController, currentColor);
            }
        }
    }
    
    private void UpdateSize()
    {
        // 计算新的大小乘数
        float targetSizeMultiplier = baseSizeMultiplier + (currentReaction * maxSizeChange);
        
        // 平滑过渡大小
        currentSizeMultiplier = Mathf.Lerp(currentSizeMultiplier, targetSizeMultiplier, Time.deltaTime * smoothingSpeed);
        
        // 应用大小变化
        transform.localScale = Vector3.one * currentSizeMultiplier;
    }
    
    // 公开方法，用于从外部启用/禁用音频反应
    public void SetAudioReaction(bool enable)
    {
        enableAudioReaction = enable;
        
        // 如果禁用，重置到基础值
        if (!enable)
        {
            ResetToBaseValues();
        }
    }
    
    // 重置到基础值
    private void ResetToBaseValues()
    {
        currentReaction = 0f;
        targetReaction = 0f;
        currentSizeMultiplier = baseSizeMultiplier;
        currentColor = baseColor;
        
        // 重置发光因子
        var glowFactorField = atmosphereController.GetType().GetField("glowFactor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (glowFactorField != null)
        {
            glowFactorField.SetValue(atmosphereController, baseGlowFactor);
        }
        
        // 重置颜色
        var colorField = atmosphereController.GetType().GetField("atmosphereColor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (colorField != null)
        {
            colorField.SetValue(atmosphereController, baseColor);
        }
        
        // 重置大小
        transform.localScale = Vector3.one * baseSizeMultiplier;
    }
}
