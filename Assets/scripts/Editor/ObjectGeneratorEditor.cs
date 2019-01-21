using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectGenerator))]
public class ObjectGeneratorEditor : Editor
{
    ObjectGenerator obj;
    void OnEnable()
    {
        obj = (ObjectGenerator)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();

        if (GUILayout.Button("Random fill objects"))
        {
            obj.FillRoadRandom();
        }

        if (GUILayout.Button("Test"))
        {
        }

        if (EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll();
        }
    }

    void OnSceneGUI()
    {
        Draw();

    }

    void Draw()
    {
       
    }
}
