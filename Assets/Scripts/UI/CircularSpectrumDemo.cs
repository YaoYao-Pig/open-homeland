using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CircularSpectrumDemo : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AudioAnalyzer audioAnalyzer;
    [SerializeField] private CircularSpectrumController spectrumController;
    
    [Header("UI Controls")]
    [SerializeField] private Button loadAudioButton;
    [SerializeField] private Slider barCountSlider;
    [SerializeField] private Slider minHeightSlider;
    [SerializeField] private Slider maxHeightSlider;
    [SerializeField] private Slider barWidthSlider;
    [SerializeField] private Slider innerRadiusSlider;
    [SerializeField] private Slider rotationSpeedSlider;
    [SerializeField] private Text statusText;
    
    // 组件引用
    private AudioSource audioSource;
    
    private void Start()
    {
        // 获取音频源
        if (audioAnalyzer != null)
        {
            audioSource = audioAnalyzer.GetComponent<AudioSource>();
        }
        
        // 设置UI事件
        if (loadAudioButton != null)
        {
            loadAudioButton.onClick.AddListener(OpenAudioFile);
        }
        
        if (barCountSlider != null)
        {
            barCountSlider.onValueChanged.AddListener(SetBarCount);
        }
        
        if (minHeightSlider != null && maxHeightSlider != null)
        {
            minHeightSlider.onValueChanged.AddListener(SetMinHeight);
            maxHeightSlider.onValueChanged.AddListener(SetMaxHeight);
        }
        
        if (barWidthSlider != null)
        {
            barWidthSlider.onValueChanged.AddListener(SetBarWidth);
        }
        
        if (innerRadiusSlider != null)
        {
            innerRadiusSlider.onValueChanged.AddListener(SetInnerRadius);
        }
        
        if (rotationSpeedSlider != null)
        {
            rotationSpeedSlider.onValueChanged.AddListener(SetRotationSpeed);
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
    
    // 设置条形数量
    private void SetBarCount(float value)
    {
        if (spectrumController != null)
        {
            spectrumController.SetBarCount(Mathf.RoundToInt(value));
        }
    }
    
    // 设置最小高度
    private void SetMinHeight(float value)
    {
        if (spectrumController != null)
        {
            float maxHeight = maxHeightSlider != null ? maxHeightSlider.value : 0.5f;
            spectrumController.SetBarHeightRange(value, maxHeight);
        }
    }
    
    // 设置最大高度
    private void SetMaxHeight(float value)
    {
        if (spectrumController != null)
        {
            float minHeight = minHeightSlider != null ? minHeightSlider.value : 0.1f;
            spectrumController.SetBarHeightRange(minHeight, value);
        }
    }
    
    // 设置条形宽度
    private void SetBarWidth(float value)
    {
        if (spectrumController != null)
        {
            spectrumController.SetBarWidth(value);
        }
    }
    
    // 设置内半径
    private void SetInnerRadius(float value)
    {
        if (spectrumController != null)
        {
            spectrumController.SetInnerRadius(value);
        }
    }
    
    // 设置旋转速度
    private void SetRotationSpeed(float value)
    {
        if (spectrumController != null)
        {
            spectrumController.SetRotationSpeed(value);
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
}
