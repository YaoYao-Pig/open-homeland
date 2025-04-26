using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DualTrailController))]
public class DualTrailControllerEditor : Editor
{
    SerializedProperty primaryTrailMaterial;
    SerializedProperty primaryTrailTime;
    SerializedProperty primaryStartWidth;
    SerializedProperty primaryEndWidth;
    SerializedProperty primaryColorGradient;
    
    SerializedProperty secondaryTrailMaterial;
    SerializedProperty secondaryTrailTime;
    SerializedProperty secondaryStartWidth;
    SerializedProperty secondaryEndWidth;
    SerializedProperty secondaryColorGradient;
    
    SerializedProperty animateWidth;
    SerializedProperty pulseSpeed;
    SerializedProperty pulseAmount;
    
    private bool showPresets = false;
    
    private void OnEnable()
    {
        // Get serialized properties
        primaryTrailMaterial = serializedObject.FindProperty("primaryTrailMaterial");
        primaryTrailTime = serializedObject.FindProperty("primaryTrailTime");
        primaryStartWidth = serializedObject.FindProperty("primaryStartWidth");
        primaryEndWidth = serializedObject.FindProperty("primaryEndWidth");
        primaryColorGradient = serializedObject.FindProperty("primaryColorGradient");
        
        secondaryTrailMaterial = serializedObject.FindProperty("secondaryTrailMaterial");
        secondaryTrailTime = serializedObject.FindProperty("secondaryTrailTime");
        secondaryStartWidth = serializedObject.FindProperty("secondaryStartWidth");
        secondaryEndWidth = serializedObject.FindProperty("secondaryEndWidth");
        secondaryColorGradient = serializedObject.FindProperty("secondaryColorGradient");
        
        animateWidth = serializedObject.FindProperty("animateWidth");
        pulseSpeed = serializedObject.FindProperty("pulseSpeed");
        pulseAmount = serializedObject.FindProperty("pulseAmount");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.LabelField("Dual Trail Settings", EditorStyles.boldLabel);
        
        // Primary Trail Settings
        EditorGUILayout.LabelField("Primary Trail", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(primaryTrailMaterial);
        EditorGUILayout.PropertyField(primaryTrailTime);
        EditorGUILayout.PropertyField(primaryStartWidth);
        EditorGUILayout.PropertyField(primaryEndWidth);
        EditorGUILayout.PropertyField(primaryColorGradient);
        EditorGUI.indentLevel--;
        
        EditorGUILayout.Space();
        
        // Secondary Trail Settings
        EditorGUILayout.LabelField("Secondary Trail (Glow)", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(secondaryTrailMaterial);
        EditorGUILayout.PropertyField(secondaryTrailTime);
        EditorGUILayout.PropertyField(secondaryStartWidth);
        EditorGUILayout.PropertyField(secondaryEndWidth);
        EditorGUILayout.PropertyField(secondaryColorGradient);
        EditorGUI.indentLevel--;
        
        EditorGUILayout.Space();
        
        // Animation Settings
        EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(animateWidth);
        if (animateWidth.boolValue)
        {
            EditorGUILayout.PropertyField(pulseSpeed);
            EditorGUILayout.PropertyField(pulseAmount);
        }
        EditorGUI.indentLevel--;
        
        EditorGUILayout.Space();
        
        // Presets
        showPresets = EditorGUILayout.Foldout(showPresets, "Presets for Large Planets");
        if (showPresets)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Massive Blue Comet"))
            {
                ApplyMassiveBlueComet();
            }
            if (GUILayout.Button("Giant Fire Comet"))
            {
                ApplyGiantFireComet();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Cosmic Energy"))
            {
                ApplyCosmicEnergy();
            }
            if (GUILayout.Button("Galactic Core"))
            {
                ApplyGalacticCore();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void ApplyMassiveBlueComet()
    {
        // Primary trail settings
        primaryStartWidth.floatValue = 1.0f;
        primaryEndWidth.floatValue = 0.4f;
        primaryTrailTime.floatValue = 4.0f;
        
        // Secondary trail settings
        secondaryStartWidth.floatValue = 1.5f;
        secondaryEndWidth.floatValue = 0.6f;
        secondaryTrailTime.floatValue = 4.5f;
        
        // Animation settings
        animateWidth.boolValue = true;
        pulseSpeed.floatValue = 1.2f;
        pulseAmount.floatValue = 0.3f;
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void ApplyGiantFireComet()
    {
        // Primary trail settings
        primaryStartWidth.floatValue = 1.2f;
        primaryEndWidth.floatValue = 0.5f;
        primaryTrailTime.floatValue = 3.5f;
        
        // Secondary trail settings
        secondaryStartWidth.floatValue = 1.8f;
        secondaryEndWidth.floatValue = 0.7f;
        secondaryTrailTime.floatValue = 4.0f;
        
        // Animation settings
        animateWidth.boolValue = true;
        pulseSpeed.floatValue = 1.5f;
        pulseAmount.floatValue = 0.4f;
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void ApplyCosmicEnergy()
    {
        // Primary trail settings
        primaryStartWidth.floatValue = 0.9f;
        primaryEndWidth.floatValue = 0.3f;
        primaryTrailTime.floatValue = 5.0f;
        
        // Secondary trail settings
        secondaryStartWidth.floatValue = 1.4f;
        secondaryEndWidth.floatValue = 0.5f;
        secondaryTrailTime.floatValue = 5.5f;
        
        // Animation settings
        animateWidth.boolValue = true;
        pulseSpeed.floatValue = 0.8f;
        pulseAmount.floatValue = 0.25f;
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void ApplyGalacticCore()
    {
        // Primary trail settings
        primaryStartWidth.floatValue = 1.5f;
        primaryEndWidth.floatValue = 0.6f;
        primaryTrailTime.floatValue = 4.0f;
        
        // Secondary trail settings
        secondaryStartWidth.floatValue = 2.2f;
        secondaryEndWidth.floatValue = 0.9f;
        secondaryTrailTime.floatValue = 4.5f;
        
        // Animation settings
        animateWidth.boolValue = true;
        pulseSpeed.floatValue = 1.0f;
        pulseAmount.floatValue = 0.35f;
        
        serializedObject.ApplyModifiedProperties();
    }
}
