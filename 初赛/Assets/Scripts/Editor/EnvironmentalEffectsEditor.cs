using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnvironmentalEffectsManager))]
public class EnvironmentalEffectsEditor : Editor
{
    SerializedProperty meteorSystem;
    SerializedProperty northAurora;
    SerializedProperty southAurora;
    SerializedProperty planetTransform;
    SerializedProperty dataUpdateInterval;
    SerializedProperty useSimulatedData;
    
    private bool showTestControls = false;
    
    private void OnEnable()
    {
        meteorSystem = serializedObject.FindProperty("meteorSystem");
        northAurora = serializedObject.FindProperty("northAurora");
        southAurora = serializedObject.FindProperty("southAurora");
        planetTransform = serializedObject.FindProperty("planetTransform");
        dataUpdateInterval = serializedObject.FindProperty("dataUpdateInterval");
        useSimulatedData = serializedObject.FindProperty("useSimulatedData");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.LabelField("Environmental Effects Manager", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.PropertyField(planetTransform);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Effects References", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(meteorSystem);
        EditorGUILayout.PropertyField(northAurora);
        EditorGUILayout.PropertyField(southAurora);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Data Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(dataUpdateInterval);
        EditorGUILayout.PropertyField(useSimulatedData);
        
        serializedObject.ApplyModifiedProperties();
        
        EditorGUILayout.Space();
        showTestControls = EditorGUILayout.Foldout(showTestControls, "Test Controls");
        
        if (showTestControls)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            if (GUILayout.Button("Trigger Small Meteor Shower (1-3 meteors)"))
            {
                EnvironmentalEffectsManager manager = (EnvironmentalEffectsManager)target;
                manager.TriggerCommitEffect(1);
            }
            
            if (GUILayout.Button("Trigger Medium Meteor Shower (4-6 meteors)"))
            {
                EnvironmentalEffectsManager manager = (EnvironmentalEffectsManager)target;
                manager.TriggerCommitEffect(2);
            }
            
            if (GUILayout.Button("Trigger Large Meteor Shower (7-10 meteors)"))
            {
                EnvironmentalEffectsManager manager = (EnvironmentalEffectsManager)target;
                manager.TriggerCommitEffect(3);
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Aurora Activity Level");
            float activityLevel = EditorGUILayout.Slider(0f, 0f, 1f);
            
            if (GUILayout.Button("Set Aurora Activity"))
            {
                EnvironmentalEffectsManager manager = (EnvironmentalEffectsManager)target;
                manager.SetActivityLevel(activityLevel);
            }
            
            EditorGUILayout.EndVertical();
        }
    }
}
