using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class AudioAtmosphereLargeDemo : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AudioAnalyzer audioAnalyzer;
    [SerializeField] private GameObject planetObject;
    
    [Header("UI")]
    [SerializeField] private Button loadAudioButton;
    [SerializeField] private Slider intensitySlider;
    [SerializeField] private Slider amplitudeSlider;
    [SerializeField] private Slider frequencySlider;
    [SerializeField] private Toggle enableToggle;
    [SerializeField] private Text statusText;
    
    // 组件引用
    private AudioReactiveAtmosphereLargeExtension atmosphereExtension;
    private AudioSource audioSource;
    
    private void Start()
    {
        // 获取组件引用
        if (planetObject != null)
        {
            atmosphereExtension = planetObject.GetComponent<AudioReactiveAtmosphereLargeExtension>();
            if (atmosphereExtension == null)
            {
                atmosphereExtension = planetObject.AddComponent<AudioReactiveAtmosphereLargeExtension>();
            }
        }
        
        if (audioAnalyzer != null)
        {
            audioSource = audioAnalyzer.GetComponent<AudioSource>();
        }
        
        // 设置UI事件
        if (loadAudioButton != null)
        {
            loadAudioButton.onClick.AddListener(OpenAudioFile);
        }
        
        if (intensitySlider != null)
        {
            intensitySlider.onValueChanged.AddListener(SetIntensity);
        }
        
        if (amplitudeSlider != null)
        {
            amplitudeSlider.onValueChanged.AddListener(SetAmplitude);
        }
        
        if (frequencySlider != null)
        {
            frequencySlider.onValueChanged.AddListener(SetFrequency);
        }
        
        if (enableToggle != null)
        {
            enableToggle.onValueChanged.AddListener(ToggleAtmosphere);
        }
        
        // 初始化
        if (atmosphereExtension != null && audioAnalyzer != null)
        {
            atmosphereExtension.SetAudioAnalyzer(audioAnalyzer);
        }
        
        UpdateStatusText("Ready. Load an audio file to begin.");
    }
    
    // 打开音频文件
    private void OpenAudioFile()
    {
        #if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.OpenFilePanel("Select Audio File", "", "mp3,wav,ogg");
        if (!string.IsNullOrEmpty(path))
        {
            StartCoroutine(LoadAudioFile(path));
        }
        #else
        UpdateStatusText("File loading is only available in the Unity Editor.");
        #endif
    }
    
    // 加载音频文件
    private IEnumerator LoadAudioFile(string path)
    {
        UpdateStatusText("Loading audio file...");
        
        string extension = Path.GetExtension(path).ToLower();
        AudioType audioType = AudioType.UNKNOWN;
        
        // 确定音频类型
        if (extension == ".mp3")
        {
            audioType = AudioType.MPEG;
        }
        else if (extension == ".wav")
        {
            audioType = AudioType.WAV;
        }
        else if (extension == ".ogg")
        {
            audioType = AudioType.OGGVORBIS;
        }
        else
        {
            UpdateStatusText("Unsupported audio format. Please use MP3, WAV, or OGG.");
            yield break;
        }
        
        // 使用WWW加载音频文件
        using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip("file://" + path, audioType))
        {
            yield return www.SendWebRequest();
            
            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                AudioClip clip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(www);
                if (clip != null && audioSource != null)
                {
                    // 停止当前播放
                    audioSource.Stop();
                    
                    // 设置新的音频剪辑
                    audioSource.clip = clip;
                    audioSource.Play();
                    
                    UpdateStatusText("Playing: " + Path.GetFileName(path));
                }
                else
                {
                    UpdateStatusText("Failed to load audio clip.");
                }
            }
            else
            {
                UpdateStatusText("Error loading audio: " + www.error);
            }
        }
    }
    
    // 设置强度
    private void SetIntensity(float value)
    {
        if (atmosphereExtension != null)
        {
            atmosphereExtension.SetPulseIntensity(value);
        }
    }
    
    // 设置振幅
    private void SetAmplitude(float value)
    {
        if (atmosphereExtension != null)
        {
            atmosphereExtension.SetWaveAmplitude(value);
        }
    }
    
    // 设置频率
    private void SetFrequency(float value)
    {
        if (atmosphereExtension != null)
        {
            atmosphereExtension.SetWaveFrequency(value);
        }
    }
    
    // 切换大气层
    private void ToggleAtmosphere(bool enable)
    {
        if (atmosphereExtension != null)
        {
            atmosphereExtension.ToggleAtmosphere(enable);
        }
    }
    
    // 更新状态文本
    private void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        else
        {
            Debug.Log(message);
        }
    }
    
    // 在编辑器中显示频谱可视化
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || audioAnalyzer == null) return;
        
        float width = 5f;
        float height = 2f;
        float spacing = 0.1f;
        Vector3 startPos = transform.position + new Vector3(-width/2, 0, 0);
        
        // 绘制频段可视化
        for (int i = 0; i < 8; i++)
        {
            float bandValue = audioAnalyzer.GetBufferedBandValue(i);
            float bandHeight = bandValue * height;
            
            Gizmos.color = Color.Lerp(Color.blue, Color.red, bandValue);
            Gizmos.DrawCube(startPos + new Vector3(i * (width/8 + spacing), bandHeight/2, 0), 
                new Vector3(width/8, bandHeight, 0.1f));
        }
        
        // 绘制音频强度
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(startPos + new Vector3(-1f, audioAnalyzer.AudioProfile * height, 0), 0.2f);
    }
}
