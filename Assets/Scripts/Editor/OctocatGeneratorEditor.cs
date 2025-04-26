using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OctocatGenerator))]
public class OctocatGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        OctocatGenerator generator = (OctocatGenerator)target;
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Generate Octocat", GUILayout.Height(30)))
        {
            generator.GenerateOctocat();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("This generates a simplified Octocat shape. For a more detailed model, consider importing a 3D model from external sources.", MessageType.Info);
    }
}
