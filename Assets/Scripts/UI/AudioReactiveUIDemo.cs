using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class AudioReactiveUIDemo : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AudioAnalyzer audioAnalyzer;
    [SerializeField] private AudioReactiveUIController circleController;
    
    [Header("UI Controls")]
    [SerializeField] private Button loadAudioButton;
    [SerializeField] private Slider intensitySlider;
    [SerializeField] private Slider amplitudeSlider;
    [SerializeField] private Slider waveCountSlider;
    [SerializeField] private Slider rotationSlider;
    [SerializeField] private Toggle enableToggle;
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
        
        if (intensitySlider != null)
        {
            intensitySlider.onValueChanged.AddListener(SetIntensity);
        }
        
        if (amplitudeSlider != null)
        {
            amplitudeSlider.onValueChanged.AddListener(SetAmplitude);
        }
        
        if (waveCountSlider != null)
        {
            waveCountSlider.onValueChanged.AddListener(SetWaveCount);
        }
        
        if (rotationSlider != null)
        {
            rotationSlider.onValueChanged.AddListener(SetRotation);
        }
        
        if (enableToggle != null)
        {
            enableToggle.onValueChanged.AddListener(ToggleAudioReaction);
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
        if (circleController != null)
        {
            circleController.SetPulseIntensity(value);
        }
    }
    
    // 设置振幅
    private void SetAmplitude(float value)
    {
        if (circleController != null)
        {
            circleController.SetWaveAmplitude(value);
        }
    }
    
    // 设置波数
    private void SetWaveCount(float value)
    {
        if (circleController != null)
        {
            circleController.SetWaveCount(value);
        }
    }
    
    // 设置旋转速度
    private void SetRotation(float value)
    {
        if (circleController != null)
        {
            circleController.SetRotationSpeed(value);
        }
    }
    
    // 切换音频反应
    private void ToggleAudioReaction(bool enable)
    {
        if (circleController != null)
        {
            circleController.SetAudioReaction(enable);
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
