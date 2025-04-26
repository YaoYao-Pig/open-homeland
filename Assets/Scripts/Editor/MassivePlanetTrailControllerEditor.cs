using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MassivePlanetTrailController))]
public class MassivePlanetTrailControllerEditor : Editor
{
    SerializedProperty trailMaterial;
    SerializedProperty trailTime;
    SerializedProperty startWidth;
    SerializedProperty endWidth;
    SerializedProperty widthCurve;
    SerializedProperty colorGradient;
    
    SerializedProperty useSecondaryGlow;
    SerializedProperty glowMaterial;
    SerializedProperty glowMultiplier;
    
    SerializedProperty animateWidth;
    SerializedProperty pulseSpeed;
    SerializedProperty pulseAmount;
    
    SerializedProperty addPointLight;
    SerializedProperty lightColor;
    SerializedProperty lightRange;
    SerializedProperty lightIntensity;
    
    private void OnEnable()
    {
        // Get serialized properties
        trailMaterial = serializedObject.FindProperty("trailMaterial");
        trailTime = serializedObject.FindProperty("trailTime");
        startWidth = serializedObject.FindProperty("startWidth");
        endWidth = serializedObject.FindProperty("endWidth");
        widthCurve = serializedObject.FindProperty("widthCurve");
        colorGradient = serializedObject.FindProperty("colorGradient");
        
        useSecondaryGlow = serializedObject.FindProperty("useSecondaryGlow");
        glowMaterial = serializedObject.FindProperty("glowMaterial");
        glowMultiplier = serializedObject.FindProperty("glowMultiplier");
        
        animateWidth = serializedObject.FindProperty("animateWidth");
        pulseSpeed = serializedObject.FindProperty("pulseSpeed");
        pulseAmount = serializedObject.FindProperty("pulseAmount");
        
        addPointLight = serializedObject.FindProperty("addPointLight");
        lightColor = serializedObject.FindProperty("lightColor");
        lightRange = serializedObject.FindProperty("lightRange");
        lightIntensity = serializedObject.FindProperty("lightIntensity");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.HelpBox("This controller is specially designed for extremely large planets (200x scale).", MessageType.Info);
        
        EditorGUILayout.Space();
        
        // Ultra Massive Presets
        EditorGUILayout.LabelField("Ultra Massive Presets", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Ultra Massive Blue", GUILayout.Height(30)))
        {
            MassivePlanetTrailController controller = (MassivePlanetTrailController)target;
            controller.ApplyUltraMassivePreset();
        }
        if (GUILayout.Button("Galactic Flame", GUILayout.Height(30)))
        {
            MassivePlanetTrailController controller = (MassivePlanetTrailController)target;
            controller.ApplyGalacticFlamePreset();
        }
        if (GUILayout.Button("Cosmic Energy", GUILayout.Height(30)))
        {
            MassivePlanetTrailController controller = (MassivePlanetTrailController)target;
            controller.ApplyCosmicEnergyPreset();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Main Trail Settings
        EditorGUILayout.LabelField("Main Trail Settings", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(trailMaterial);
        EditorGUILayout.PropertyField(trailTime);
        EditorGUILayout.PropertyField(startWidth);
        EditorGUILayout.PropertyField(endWidth);
        EditorGUILayout.PropertyField(widthCurve);
        EditorGUILayout.PropertyField(colorGradient);
        EditorGUI.indentLevel--;
        
        EditorGUILayout.Space();
        
        // Glow Settings
        EditorGUILayout.LabelField("Glow Effect", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(useSecondaryGlow);
        if (useSecondaryGlow.boolValue)
        {
            EditorGUILayout.PropertyField(glowMaterial);
            EditorGUILayout.PropertyField(glowMultiplier);
        }
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
        
        // Light Settings
        EditorGUILayout.LabelField("Light Effect", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(addPointLight);
        if (addPointLight.boolValue)
        {
            EditorGUILayout.PropertyField(lightColor);
            EditorGUILayout.PropertyField(lightRange);
            EditorGUILayout.PropertyField(lightIntensity);
        }
        EditorGUI.indentLevel--;
        
        serializedObject.ApplyModifiedProperties();
    }
}
