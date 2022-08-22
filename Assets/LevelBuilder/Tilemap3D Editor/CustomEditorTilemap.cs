using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RecreateTileMapInEditMode))]
public class CustomEditorTilemap : Editor
{
    public override void OnInspectorGUI()
    {
        RecreateTileMapInEditMode myScript = (RecreateTileMapInEditMode)target;

        if (GUILayout.Button("Reload Level"))
            myScript.LoadBinary();
    }
}
