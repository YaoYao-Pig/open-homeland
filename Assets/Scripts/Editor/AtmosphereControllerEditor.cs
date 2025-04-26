using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AtmosphereController))]
public class AtmosphereControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AtmosphereController atmosphereController = (AtmosphereController)target;
        
        // Draw default inspector
        DrawDefaultInspector();
        
        // Add a button to adjust atmosphere to match planet
        EditorGUILayout.Space();
        if (GUILayout.Button("Adjust to Match Planet"))
        {
            atmosphereController.AdjustToMatchPlanet();
        }
        
        // Apply changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
