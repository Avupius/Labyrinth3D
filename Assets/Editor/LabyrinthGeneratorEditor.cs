using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LabyrinthGenerator))]
public class LabyrinthGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        LabyrinthGenerator generator = (LabyrinthGenerator)target;
        
        GUILayout.Space(20);
        GUILayout.Label("Generator-Kontrolle", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Labyrinth generieren", GUILayout.Height(40)))
        {
            generator.GenerateMaze();
            EditorUtility.DisplayDialog("Erfolg", "Labyrinth wurde generiert!", "OK");
        }
        
        GUILayout.Space(10);
        if (GUILayout.Button("Clear", GUILayout.Height(30)))
        {
            // Setzt die Szene zurueck
            EditorUtility.DisplayDialog("Info", "Loeschen Sie die Wand- und Floor-Objekte manuell.", "OK");
        }
    }
}