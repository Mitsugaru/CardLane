using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ArenaManager))]
public class ArenaManagerEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ArenaManager script = (ArenaManager)target;
        if (GUILayout.Button("Demo"))
        {
            script.DemoArena();
        }
    }
}
