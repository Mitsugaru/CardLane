using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DeckScript))]
public class DeckScriptEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DeckScript script = (DeckScript)target;
        if (GUILayout.Button("Draw"))
        {
            script.drawCard();
        }
        if (GUILayout.Button("Spawn"))
        {
            script.spawnDeck();
        }
    }
}
