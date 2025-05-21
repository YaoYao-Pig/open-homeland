using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CircularSpectrumCreator : MonoBehaviour
{
    #if UNITY_EDITOR
    [MenuItem("GameObject/UI/Circular Audio Spectrum", false, 10)]
    static void CreateCircularSpectrum(MenuCommand menuCommand)
    {
        // 创建一个新的游戏对象
        GameObject go = new GameObject("CircularAudioSpectrum");
        
        // 确保它在Canvas下
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            // 如果没有Canvas，创建一个
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        go.transform.SetParent(canvas.transform, false);
        
        // 添加必要的组件
        Image image = go.AddComponent<Image>();
        CircularSpectrumController controller = go.AddComponent<CircularSpectrumController>();
        
        // 设置RectTransform
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(300, 300);
        
        // 尝试查找或创建AudioAnalyzer
        AudioAnalyzer analyzer = FindObjectOfType<AudioAnalyzer>();
        if (analyzer == null)
        {
            GameObject analyzerGO = new GameObject("AudioAnalyzer");
            analyzer = analyzerGO.AddComponent<AudioAnalyzer>();
            analyzerGO.AddComponent<AudioSource>();
        }
        
        // 尝试查找或创建AudioDataTextureGenerator
        AudioDataTextureGenerator generator = FindObjectOfType<AudioDataTextureGenerator>();
        if (generator == null)
        {
            GameObject generatorGO = new GameObject("AudioDataGenerator");
            generator = generatorGO.AddComponent<AudioDataTextureGenerator>();
            generatorGO.transform.SetParent(analyzer.transform);
        }
        
        // 设置控制器的引用
        SerializedObject serializedController = new SerializedObject(controller);
        SerializedProperty analyzerProp = serializedController.FindProperty("audioAnalyzer");
        SerializedProperty generatorProp = serializedController.FindProperty("audioDataGenerator");
        
        analyzerProp.objectReferenceValue = analyzer;
        generatorProp.objectReferenceValue = generator;
        
        serializedController.ApplyModifiedProperties();
        
        // 注册撤销
        Undo.RegisterCreatedObjectUndo(go, "Create Circular Audio Spectrum");
        
        // 选择新创建的对象
        Selection.activeObject = go;
    }
    #endif
}
