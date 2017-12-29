using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameController script = (GameController)target;
        if(GUILayout.Button("New Game"))
        {
            script.NewGame();
        }
    }
}
