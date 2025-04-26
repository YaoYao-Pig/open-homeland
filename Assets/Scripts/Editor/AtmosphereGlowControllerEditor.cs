using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AtmosphereGlowController))]
public class AtmosphereGlowControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AtmosphereGlowController controller = (AtmosphereGlowController)target;
        
        // Draw default inspector
        DrawDefaultInspector();
        
        // Add a button to adjust atmosphere to match planet
        EditorGUILayout.Space();
        if (GUILayout.Button("Adjust to Match Planet"))
        {
            controller.AdjustToMatchPlanet();
        }
        
        // Apply changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
