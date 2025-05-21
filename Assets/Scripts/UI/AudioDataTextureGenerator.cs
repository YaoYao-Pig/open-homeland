using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDataTextureGenerator : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioAnalyzer audioAnalyzer;
    [SerializeField] private int textureWidth = 64; // 纹理宽度，应与频谱条数匹配
    [SerializeField] private int textureHeight = 1; // 高度为1，因为我们只需要一行数据
    
    [Header("Smoothing")]
    [SerializeField] private float smoothingSpeed = 5.0f;
    [SerializeField] private float peakFalloff = 0.8f;
    
    // 纹理和像素数据
    private Texture2D audioDataTexture;
    private Color[] pixelData;
    private float[] currentValues;
    private float[] peakValues;
    
    // 公开纹理，供其他脚本访问
    public Texture2D AudioDataTexture => audioDataTexture;
    
    private void Awake()
    {
        // 创建纹理
        audioDataTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RFloat, false);
        audioDataTexture.wrapMode = TextureWrapMode.Clamp;
        audioDataTexture.filterMode = FilterMode.Bilinear;
        
        // 初始化像素数据
        pixelData = new Color[textureWidth * textureHeight];
        currentValues = new float[textureWidth];
        peakValues = new float[textureWidth];
        
        // 清除纹理
        ClearTexture();
    }
    
    private void Start()
    {
        // 如果没有指定音频分析器，尝试在场景中查找
        if (audioAnalyzer == null)
        {
            audioAnalyzer = FindObjectOfType<AudioAnalyzer>();
            if (audioAnalyzer == null)
            {
                Debug.LogWarning("AudioDataTextureGenerator: No AudioAnalyzer found.");
            }
        }
    }
    
    private void Update()
    {
        if (audioAnalyzer != null)
        {
            UpdateAudioData();
        }
    }
    
    private void UpdateAudioData()
    {
        // 获取音频数据
        for (int i = 0; i < textureWidth; i++)
        {
            // 将索引映射到音频分析器的频段
            float bandValue = 0;
            
            if (textureWidth <= 8)
            {
                // 如果纹理宽度小于等于8，直接使用音频分析器的8个频段
                bandValue = audioAnalyzer.GetBufferedBandValue(i % 8);
            }
            else
            {
                // 否则，插值获取更多频段
                float bandIndex = (float)i / textureWidth * 8;
                int lowerBand = Mathf.FloorToInt(bandIndex);
                int upperBand = Mathf.CeilToInt(bandIndex);
                float t = bandIndex - lowerBand;
                
                if (lowerBand == upperBand)
                {
                    bandValue = audioAnalyzer.GetBufferedBandValue(lowerBand);
                }
                else
                {
                    float lowerValue = audioAnalyzer.GetBufferedBandValue(lowerBand);
                    float upperValue = audioAnalyzer.GetBufferedBandValue(upperBand);
                    bandValue = Mathf.Lerp(lowerValue, upperValue, t);
                }
            }
            
            // 平滑过渡
            currentValues[i] = Mathf.Lerp(currentValues[i], bandValue, Time.deltaTime * smoothingSpeed);
            
            // 更新峰值
            if (currentValues[i] > peakValues[i])
            {
                peakValues[i] = currentValues[i];
            }
            else
            {
                peakValues[i] *= peakFalloff;
            }
            
            // 设置像素颜色（使用红色通道存储音频值）
            pixelData[i] = new Color(currentValues[i], 0, 0, 1);
        }
        
        // 更新纹理
        audioDataTexture.SetPixels(pixelData);
        audioDataTexture.Apply();
    }
    
    private void ClearTexture()
    {
        for (int i = 0; i < pixelData.Length; i++)
        {
            pixelData[i] = Color.black;
        }
        
        audioDataTexture.SetPixels(pixelData);
        audioDataTexture.Apply();
    }
    
    // 公开方法，用于获取特定索引的音频值
    public float GetAudioValue(int index)
    {
        if (index >= 0 && index < textureWidth)
        {
            return currentValues[index];
        }
        return 0;
    }
    
    // 公开方法，用于获取特定索引的峰值
    public float GetPeakValue(int index)
    {
        if (index >= 0 && index < textureWidth)
        {
            return peakValues[index];
        }
        return 0;
    }
}
