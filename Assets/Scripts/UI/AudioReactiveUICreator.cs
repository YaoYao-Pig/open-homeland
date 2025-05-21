using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AudioReactiveUICreator : MonoBehaviour
{
    #if UNITY_EDITOR
    [MenuItem("GameObject/UI/Audio Reactive Circle", false, 10)]
    static void CreateAudioReactiveCircle(MenuCommand menuCommand)
    {
        // 创建一个新的游戏对象
        GameObject go = new GameObject("AudioReactiveCircle");
        
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
        AudioReactiveUIController controller = go.AddComponent<AudioReactiveUIController>();
        
        // 设置RectTransform
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(200, 200);
        
        // 尝试查找或创建AudioAnalyzer
        AudioAnalyzer analyzer = FindObjectOfType<AudioAnalyzer>();
        if (analyzer == null)
        {
            GameObject analyzerGO = new GameObject("AudioAnalyzer");
            analyzer = analyzerGO.AddComponent<AudioAnalyzer>();
            analyzerGO.AddComponent<AudioSource>();
        }
        
        // 设置控制器的音频分析器
        controller.SetAudioReaction(true);
        
        // 注册撤销
        Undo.RegisterCreatedObjectUndo(go, "Create Audio Reactive Circle");
        
        // 选择新创建的对象
        Selection.activeObject = go;
    }
    #endif
}
