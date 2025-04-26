using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlanetEnvironmentExtension))]
public class PlanetEnvironmentExtensionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PlanetEnvironmentExtension extension = (PlanetEnvironmentExtension)target;
        
        // 绘制默认检查器
        DrawDefaultInspector();
        
        // 添加常用操作的按钮
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Update Environment Effects"))
        {
            extension.UpdateEnvironmentEffects();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Test Controls", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Trigger Small Meteor Shower"))
        {
            extension.TriggerMeteorShower(1);
        }
        
        if (GUILayout.Button("Trigger Medium Meteor Shower"))
        {
            extension.TriggerMeteorShower(2);
        }
        
        if (GUILayout.Button("Trigger Large Meteor Shower"))
        {
            extension.TriggerMeteorShower(3);
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Aurora Activity Level");
        float activityLevel = EditorGUILayout.Slider(0.5f, 0f, 1f);
        
        if (GUILayout.Button("Set Aurora Activity"))
        {
            extension.SetAuroraActivity(activityLevel);
        }
        
        // 应用更改
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
