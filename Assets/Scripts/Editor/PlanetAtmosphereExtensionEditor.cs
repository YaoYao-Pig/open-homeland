using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlanetAtmosphereExtension))]
public class PlanetAtmosphereExtensionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PlanetAtmosphereExtension atmosphereExtension = (PlanetAtmosphereExtension)target;
        
        // Draw default inspector
        DrawDefaultInspector();
        
        // Add buttons for common operations
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Update Atmosphere"))
        {
            atmosphereExtension.UpdateAtmosphere();
        }
        
        // Apply changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
