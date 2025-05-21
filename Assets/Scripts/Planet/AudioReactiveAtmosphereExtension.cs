using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Planet))]
public class AudioReactiveAtmosphereExtension : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioAnalyzer audioAnalyzer;
    [SerializeField] private bool enableAtmosphere = true;
    
    [Header("Atmosphere Settings")]
    [SerializeField] private Color atmosphereColor = new Color(0.5f, 0.7f, 1.0f, 1.0f);
    [SerializeField] private Color pulseColor = new Color(0.7f, 0.9f, 1.0f, 1.0f);
    [Range(0, 1)]
    [SerializeField] private float atmosphereAlpha = 0.5f;
    [Range(0, 0.5f)]
    [SerializeField] private float atmosphereHeight = 0.1f;
    
    [Header("Audio Reaction Settings")]
    [Range(0, 2)]
    [SerializeField] private float pulseIntensity = 1f;
    [Range(0, 10)]
    [SerializeField] private float waveSpeed = 2f;
    [Range(0, 0.1f)]
    [SerializeField] private float waveAmplitude = 0.02f;
    
    private Planet planet;
    private GameObject atmosphereObject;
    private AudioReactiveAtmosphereController atmosphereController;
    
    private void Awake()
    {
        planet = GetComponent<Planet>();
    }
    
    private void Start()
    {
        // 等待行星完全初始化
        StartCoroutine(SetupAtmosphereDelayed());
    }
    
    private IEnumerator SetupAtmosphereDelayed()
    {
        // 等待一帧，确保行星完全初始化
        yield return null;
        
        if (enableAtmosphere)
        {
            SetupAtmosphere();
        }
    }
    
    private void SetupAtmosphere()
    {
        // 如果大气层对象不存在，则创建
        if (atmosphereObject == null)
        {
            atmosphereObject = new GameObject("AudioReactiveAtmosphere");
            atmosphereObject.transform.parent = transform;
            atmosphereObject.transform.localPosition = Vector3.zero;
            atmosphereObject.transform.localRotation = Quaternion.identity;
            
            // 添加所需组件
            atmosphereController = atmosphereObject.AddComponent<AudioReactiveAtmosphereController>();
            
            // 设置音频分析器
            if (audioAnalyzer != null)
            {
                var controllerField = atmosphereController.GetType().GetField("audioAnalyzer", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (controllerField != null) controllerField.SetValue(atmosphereController, audioAnalyzer);
            }
            
            // 应用设置
            ApplySettings();
            
            // 调整大气层以匹配行星
            atmosphereController.AdjustToMatchPlanet();
        }
    }
    
    private void ApplySettings()
    {
        if (atmosphereController != null)
        {
            // 使用反射设置私有字段
            var colorField = atmosphereController.GetType().GetField("atmosphereColor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (colorField != null) colorField.SetValue(atmosphereController, atmosphereColor);
            
            var pulseColorField = atmosphereController.GetType().GetField("pulseColor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (pulseColorField != null) pulseColorField.SetValue(atmosphereController, pulseColor);
            
            var alphaField = atmosphereController.GetType().GetField("atmosphereAlpha", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (alphaField != null) alphaField.SetValue(atmosphereController, atmosphereAlpha);
            
            var heightField = atmosphereController.GetType().GetField("atmosphereHeight", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (heightField != null) heightField.SetValue(atmosphereController, atmosphereHeight);
            
            var pulseIntensityField = atmosphereController.GetType().GetField("pulseIntensity", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (pulseIntensityField != null) pulseIntensityField.SetValue(atmosphereController, pulseIntensity);
            
            var waveSpeedField = atmosphereController.GetType().GetField("waveSpeed", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (waveSpeedField != null) waveSpeedField.SetValue(atmosphereController, waveSpeed);
            
            var waveAmplitudeField = atmosphereController.GetType().GetField("waveAmplitude", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (waveAmplitudeField != null) waveAmplitudeField.SetValue(atmosphereController, waveAmplitude);
        }
    }
    
    public void ToggleAtmosphere(bool enable)
    {
        enableAtmosphere = enable;
        
        if (enable && atmosphereObject == null)
        {
            SetupAtmosphere();
        }
        else if (!enable && atmosphereObject != null)
        {
            Destroy(atmosphereObject);
            atmosphereObject = null;
            atmosphereController = null;
        }
    }
    
    // 当行星更新时应调用此方法
    public void UpdateAtmosphere()
    {
        if (enableAtmosphere && atmosphereController != null)
        {
            ApplySettings();
            atmosphereController.AdjustToMatchPlanet();
        }
    }
    
    // 设置音频分析器
    public void SetAudioAnalyzer(AudioAnalyzer analyzer)
    {
        audioAnalyzer = analyzer;
        
        if (atmosphereController != null)
        {
            var controllerField = atmosphereController.GetType().GetField("audioAnalyzer", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (controllerField != null) controllerField.SetValue(atmosphereController, audioAnalyzer);
        }
    }
}
