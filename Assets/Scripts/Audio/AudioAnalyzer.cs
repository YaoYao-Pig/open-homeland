using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAnalyzer : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool useExternalAudio = false;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float updateInterval = 0.05f; // 更新频率，秒
    
    [Header("Analysis Settings")]
    [SerializeField] private int sampleSize = 1024; // 采样大小，必须是2的幂
    [SerializeField] private FFTWindow fftWindow = FFTWindow.BlackmanHarris;
    [SerializeField] private int bandCount = 8; // 频段数量
    
    [Header("Debug")]
    [SerializeField] private bool showDebug = false;
    
    // 分析结果
    private float[] samples; // 原始采样数据
    private float[] freqBands; // 频段数据
    private float[] bandBuffer; // 平滑后的频段数据
    private float[] bufferDecrease; // 缓冲减少速率
    
    // 归一化数据 (0-1范围)
    private float[] normalizedBands;
    private float[] normalizedBuffer;
    
    // 公开的属性，用于其他脚本访问
    public float[] NormalizedBands => normalizedBands;
    public float[] NormalizedBuffer => normalizedBuffer;
    
    // 音频强度
    private float audioProfile = 0f;
    public float AudioProfile => audioProfile;
    
    // 节拍检测
    private float beatThreshold = 0.6f;
    private float lastBeatTime = 0f;
    private float beatCooldown = 0.2f; // 节拍冷却时间
    private bool isBeat = false;
    public bool IsBeat => isBeat;
    
    private void Awake()
    {
        // 初始化数组
        samples = new float[sampleSize];
        freqBands = new float[bandCount];
        bandBuffer = new float[bandCount];
        bufferDecrease = new float[bandCount];
        normalizedBands = new float[bandCount];
        normalizedBuffer = new float[bandCount];
        
        // 设置音频源
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        if (!useExternalAudio && audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        // 开始定期分析
        StartCoroutine(AnalyzeAudioRoutine());
    }
    
    private IEnumerator AnalyzeAudioRoutine()
    {
        while (true)
        {
            AnalyzeAudio();
            yield return new WaitForSeconds(updateInterval);
        }
    }
    
    public void AnalyzeAudio()
    {
        // 获取音频频谱数据
        audioSource.GetSpectrumData(samples, 0, fftWindow);
        
        // 计算频段值
        CalculateFrequencyBands();
        
        // 创建平滑的频段缓冲
        CreateBandBuffer();
        
        // 归一化数据
        NormalizeData();
        
        // 计算音频整体强度
        CalculateAudioProfile();
        
        // 检测节拍
        DetectBeat();
    }
    
    private void CalculateFrequencyBands()
    {
        // 将频谱数据分成几个频段
        int sampleCount = sampleSize / 2; // 只使用一半的样本（另一半是镜像）
        int samplesPerBand = sampleCount / bandCount;
        
        for (int i = 0; i < bandCount; i++)
        {
            float average = 0;
            int startSample = i * samplesPerBand;
            
            for (int j = 0; j < samplesPerBand; j++)
            {
                average += samples[startSample + j];
            }
            
            average /= samplesPerBand;
            freqBands[i] = average * 10; // 放大值以便更好地可视化
        }
    }
    
    private void CreateBandBuffer()
    {
        for (int i = 0; i < bandCount; i++)
        {
            if (freqBands[i] > bandBuffer[i])
            {
                bandBuffer[i] = freqBands[i];
                bufferDecrease[i] = 0.005f;
            }
            else
            {
                bandBuffer[i] -= bufferDecrease[i];
                bufferDecrease[i] *= 1.2f; // 指数衰减
            }
            
            // 确保不会低于0
            if (bandBuffer[i] < 0)
            {
                bandBuffer[i] = 0;
            }
        }
    }
    
    private void NormalizeData()
    {
        // 找到最大值
        float maxBand = 0;
        float maxBuffer = 0;
        
        for (int i = 0; i < bandCount; i++)
        {
            if (freqBands[i] > maxBand)
            {
                maxBand = freqBands[i];
            }
            
            if (bandBuffer[i] > maxBuffer)
            {
                maxBuffer = bandBuffer[i];
            }
        }
        
        // 归一化
        for (int i = 0; i < bandCount; i++)
        {
            if (maxBand != 0)
            {
                normalizedBands[i] = freqBands[i] / maxBand;
            }
            else
            {
                normalizedBands[i] = 0;
            }
            
            if (maxBuffer != 0)
            {
                normalizedBuffer[i] = bandBuffer[i] / maxBuffer;
            }
            else
            {
                normalizedBuffer[i] = 0;
            }
        }
    }
    
    private void CalculateAudioProfile()
    {
        float sum = 0;
        for (int i = 0; i < bandCount; i++)
        {
            sum += normalizedBands[i];
        }
        
        audioProfile = sum / bandCount;
    }
    
    private void DetectBeat()
    {
        // 简单的节拍检测 - 当音频强度超过阈值且不在冷却时间内
        if (audioProfile > beatThreshold && Time.time > lastBeatTime + beatCooldown)
        {
            isBeat = true;
            lastBeatTime = Time.time;
        }
        else
        {
            isBeat = false;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!showDebug || !Application.isPlaying) return;
        
        // 绘制频段可视化
        float width = 10f;
        float height = 5f;
        float spacing = 0.1f;
        Vector3 startPos = transform.position;
        
        for (int i = 0; i < bandCount; i++)
        {
            float bandHeight = normalizedBands[i] * height;
            float bufferHeight = normalizedBuffer[i] * height;
            
            Gizmos.color = Color.green;
            Gizmos.DrawCube(startPos + new Vector3(i * (width / bandCount + spacing), bandHeight / 2, 0), 
                new Vector3(width / bandCount, bandHeight, 0.1f));
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(startPos + new Vector3(i * (width / bandCount + spacing), bufferHeight / 2, 0), 
                new Vector3(width / bandCount, bufferHeight, 0.1f));
        }
        
        // 绘制节拍指示器
        if (isBeat)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(startPos + new Vector3(-1f, 0, 0), 0.3f);
        }
    }
    
    // 公开方法，用于获取特定频段的值
    public float GetBandValue(int band)
    {
        if (band >= 0 && band < bandCount)
        {
            return normalizedBands[band];
        }
        return 0;
    }
    
    // 获取平滑后的频段值
    public float GetBufferedBandValue(int band)
    {
        if (band >= 0 && band < bandCount)
        {
            return normalizedBuffer[band];
        }
        return 0;
    }
    
    // 获取低频、中频、高频的平均值
    public float GetLowFrequency()
    {
        int lowBandCount = bandCount / 3;
        float sum = 0;
        for (int i = 0; i < lowBandCount; i++)
        {
            sum += normalizedBands[i];
        }
        return sum / lowBandCount;
    }
    
    public float GetMidFrequency()
    {
        int lowBandCount = bandCount / 3;
        int midBandCount = bandCount / 3;
        float sum = 0;
        for (int i = lowBandCount; i < lowBandCount + midBandCount; i++)
        {
            sum += normalizedBands[i];
        }
        return sum / midBandCount;
    }
    
    public float GetHighFrequency()
    {
        int lowBandCount = bandCount / 3;
        int midBandCount = bandCount / 3;
        int highBandCount = bandCount - lowBandCount - midBandCount;
        float sum = 0;
        for (int i = lowBandCount + midBandCount; i < bandCount; i++)
        {
            sum += normalizedBands[i];
        }
        return sum / highBandCount;
    }
}
