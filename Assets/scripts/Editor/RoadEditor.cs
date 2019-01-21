using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Road))]
public class RoadEditor : Editor
{
    Road road;
    void OnEnable()
    {
        road = (Road)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();
        if (GUILayout.Button("Create road quadratic"))
        {
            //Undo.RecordObject(road, "update_road");
            road.UpdateRoad(CurveType.BezierQuadratic);
        }
        if (GUILayout.Button("Create road cubic"))
        {
            //Undo.RecordObject(road, "update_road");
            road.UpdateRoad(CurveType.BezierCubic);
        }
        else if (GUILayout.Button("Generate points"))
        {
            //Undo.RecordObject(road, "generate_points");
            road.GeneratePoints(5);
        }
        else if (GUILayout.Button("Add point"))
        {
            //Undo.RecordObject(road, "add_point");
            road.AddPoint();
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

        Vector3 toGlobal(Vector3 point)
        {
            return road.transform.TransformPoint(point);
        }

        Handles.color = Color.blue;
        var points = road.GetMainPoints();
        for (int i = 0; i < points.Count; i++)
        {
            Handles.FreeMoveHandle(toGlobal(points[i]), Quaternion.identity, 0.5f, Vector3.zero, Handles.SphereHandleCap);
        }

        var path = road.GetPath();
        for (int i = 0; i < path.Count; i++)
        {
            Handles.color = Color.green;
            Handles.FreeMoveHandle(toGlobal(path[i]), Quaternion.identity, 0.1f, Vector3.zero, Handles.SphereHandleCap);

            if (i > 0)
            {
                Vector3 forward = path[i] - path[i - 1];

                var left = Curve.LeftByForward(forward);
                var right = -left;

                Handles.FreeMoveHandle(toGlobal(path[i]+left), Quaternion.identity, 0.1f, Vector3.zero, Handles.SphereHandleCap);
                Handles.FreeMoveHandle(toGlobal(path[i]+right), Quaternion.identity, 0.1f, Vector3.zero, Handles.SphereHandleCap);
            }
        }


        var curve = road.GetCurve();
        for (int i = 0; i < curve.SegCount; i++)
        {
            //if (i == 0)
            {
                var seg = curve.GetSegment(i);
                Handles.color = Color.red;
                //Debug.Log($"seg.Control1={seg.Control1}");
                if (i > 0)
                {
                    Handles.FreeMoveHandle(toGlobal(seg.Control1), Quaternion.identity, 0.4f, Vector3.zero, Handles.SphereHandleCap);
                }
                Handles.color = Color.cyan;
                if (i < curve.SegCount - 1)
                {
                    Handles.FreeMoveHandle(toGlobal(seg.Control2), Quaternion.identity, 0.4f, Vector3.zero, Handles.SphereHandleCap);
                }
            }


        }
    }
}
