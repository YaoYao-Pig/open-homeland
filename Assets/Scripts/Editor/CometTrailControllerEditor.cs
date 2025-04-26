using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CometTrailController))]
public class CometTrailControllerEditor : Editor
{
    SerializedProperty trailMaterial;
    SerializedProperty trailTime;
    SerializedProperty startWidth;
    SerializedProperty endWidth;
    SerializedProperty widthCurve;
    SerializedProperty colorGradient;
    SerializedProperty animateWidth;
    SerializedProperty pulseSpeed;
    SerializedProperty pulseAmount;
    SerializedProperty emitParticles;
    SerializedProperty particlePrefab;
    SerializedProperty particleEmissionRate;
    
    private bool showPresets = false;
    
    private void OnEnable()
    {
        // Get serialized properties
        trailMaterial = serializedObject.FindProperty("trailMaterial");
        trailTime = serializedObject.FindProperty("trailTime");
        startWidth = serializedObject.FindProperty("startWidth");
        endWidth = serializedObject.FindProperty("endWidth");
        widthCurve = serializedObject.FindProperty("widthCurve");
        colorGradient = serializedObject.FindProperty("colorGradient");
        animateWidth = serializedObject.FindProperty("animateWidth");
        pulseSpeed = serializedObject.FindProperty("pulseSpeed");
        pulseAmount = serializedObject.FindProperty("pulseAmount");
        emitParticles = serializedObject.FindProperty("emitParticles");
        particlePrefab = serializedObject.FindProperty("particlePrefab");
        particleEmissionRate = serializedObject.FindProperty("particleEmissionRate");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.LabelField("Comet Trail Settings", EditorStyles.boldLabel);
        
        // Trail Settings
        EditorGUILayout.PropertyField(trailMaterial);
        EditorGUILayout.PropertyField(trailTime);
        EditorGUILayout.PropertyField(startWidth);
        EditorGUILayout.PropertyField(endWidth);
        EditorGUILayout.PropertyField(widthCurve);
        EditorGUILayout.PropertyField(colorGradient);
        
        EditorGUILayout.Space();
        
        // Animation Settings
        EditorGUILayout.PropertyField(animateWidth);
        if (animateWidth.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(pulseSpeed);
            EditorGUILayout.PropertyField(pulseAmount);
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Space();
        
        // Particle Settings
        EditorGUILayout.PropertyField(emitParticles);
        if (emitParticles.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(particlePrefab);
            EditorGUILayout.PropertyField(particleEmissionRate);
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Space();
        
        // Presets
        showPresets = EditorGUILayout.Foldout(showPresets, "Presets");
        if (showPresets)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Blue Comet"))
            {
                ApplyBlueComet();
            }
            if (GUILayout.Button("Fire Comet"))
            {
                ApplyFireComet();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Green Energy"))
            {
                ApplyGreenEnergy();
            }
            if (GUILayout.Button("Purple Mystic"))
            {
                ApplyPurpleMystic();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void ApplyBlueComet()
    {
        CometTrailController controller = (CometTrailController)target;
        
        // Create blue gradient
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0].color = Color.white;
        colorKeys[0].time = 0f;
        colorKeys[1].color = new Color(0.5f, 0.7f, 1f);
        colorKeys[1].time = 1f;
        
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0].alpha = 1f;
        alphaKeys[0].time = 0f;
        alphaKeys[1].alpha = 0f;
        alphaKeys[1].time = 1f;
        
        gradient.SetKeys(colorKeys, alphaKeys);
        
        // Apply settings
        colorGradient.gradientValue = gradient;
        trailTime.floatValue = 2f;
        startWidth.floatValue = 0.2f;
        endWidth.floatValue = 0.05f;
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void ApplyFireComet()
    {
        CometTrailController controller = (CometTrailController)target;
        
        // Create fire gradient
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[3];
        colorKeys[0].color = Color.yellow;
        colorKeys[0].time = 0f;
        colorKeys[1].color = new Color(1f, 0.5f, 0f);
        colorKeys[1].time = 0.5f;
        colorKeys[2].color = new Color(1f, 0.3f, 0.1f);
        colorKeys[2].time = 1f;
        
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0].alpha = 1f;
        alphaKeys[0].time = 0f;
        alphaKeys[1].alpha = 0f;
        alphaKeys[1].time = 1f;
        
        gradient.SetKeys(colorKeys, alphaKeys);
        
        // Apply settings
        colorGradient.gradientValue = gradient;
        trailTime.floatValue = 1.5f;
        startWidth.floatValue = 0.25f;
        endWidth.floatValue = 0.05f;
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void ApplyGreenEnergy()
    {
        CometTrailController controller = (CometTrailController)target;
        
        // Create green gradient
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0].color = new Color(0.7f, 1f, 0.7f);
        colorKeys[0].time = 0f;
        colorKeys[1].color = new Color(0f, 0.8f, 0.4f);
        colorKeys[1].time = 1f;
        
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0].alpha = 1f;
        alphaKeys[0].time = 0f;
        alphaKeys[1].alpha = 0f;
        alphaKeys[1].time = 1f;
        
        gradient.SetKeys(colorKeys, alphaKeys);
        
        // Apply settings
        colorGradient.gradientValue = gradient;
        trailTime.floatValue = 2.5f;
        startWidth.floatValue = 0.15f;
        endWidth.floatValue = 0.03f;
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void ApplyPurpleMystic()
    {
        CometTrailController controller = (CometTrailController)target;
        
        // Create purple gradient
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0].color = new Color(0.9f, 0.7f, 1f);
        colorKeys[0].time = 0f;
        colorKeys[1].color = new Color(0.5f, 0f, 0.8f);
        colorKeys[1].time = 1f;
        
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0].alpha = 1f;
        alphaKeys[0].time = 0f;
        alphaKeys[1].alpha = 0f;
        alphaKeys[1].time = 1f;
        
        gradient.SetKeys(colorKeys, alphaKeys);
        
        // Apply settings
        colorGradient.gradientValue = gradient;
        trailTime.floatValue = 3f;
        startWidth.floatValue = 0.18f;
        endWidth.floatValue = 0.02f;
        
        serializedObject.ApplyModifiedProperties();
    }
}
