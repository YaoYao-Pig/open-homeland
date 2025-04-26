using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlanetGlowAtmosphereExtension))]
public class PlanetGlowAtmosphereExtensionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PlanetGlowAtmosphereExtension extension = (PlanetGlowAtmosphereExtension)target;
        
        // Draw default inspector
        DrawDefaultInspector();
        
        // Add buttons for common operations
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Update Atmosphere"))
        {
            extension.UpdateAtmosphere();
        }
        
        if (GUILayout.Button("Toggle Atmosphere"))
        {
            extension.ToggleAtmosphere(!extension.enabled);
        }
        
        // Apply changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
