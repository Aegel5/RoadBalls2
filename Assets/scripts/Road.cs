using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
[Serializable]
public class Road : MonoBehaviour
{
    [SerializeField, HideInInspector]
    List<Vector3> lastPath = new List<Vector3>();

    [SerializeField, HideInInspector]
    Curve curve = new Curve();

    public void GeneratePoints(int count)
    {
        Vector3 last = GetMainPoints().LastOrDefault();

        for (int i = 0; i < count; i++)
        {
            last.x += UnityEngine.Random.Range(0, 100);
            last.y += UnityEngine.Random.Range(0, 100);
            last.z += UnityEngine.Random.Range(0, 100);

            AddPoint(last);
        }

    }

    public float Width { get { return 1f * 2; } }

    public void AddPoint(Vector3? pos = null)
    {
        var obj = new GameObject();
        obj.name = "point";
        if (pos != null)
        {
            obj.transform.localPosition = pos.Value;
        }
        else if (transform.childCount != 0)
        {
            obj.transform.localPosition = transform.GetChild(transform.childCount - 1).transform.position;
        }
        obj.transform.SetParent(transform);
    }

    public void UpdateMesh()
    {
        GetComponent<MeshFilter>().mesh = GenereateMesh();

        //int textureRepeat = Mathf.RoundToInt(tiling * points.Count * .05f);
        //GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(1, textureRepeat);
    }

    Mesh GenereateMesh()
    {
        int count = lastPath.Count;
        if (count == 0)
            return null;
        Vector3[] verts = new Vector3[count * 2];
        Vector2[] uvs = new Vector2[verts.Length];
        int numTris = 2 * (count - 1);
        int[] tris = new int[numTris * 3];
        int vertIndex = 0;
        int triIndex = 0;

        Vector3 p1, p2, p3, p4;
        for (int i = 1; i < lastPath.Count; i++)
        {
            Vector3 forward = lastPath[i] - lastPath[i - 1];

            var left = Curve.LeftByForward(forward);
            var right = -left;

            p3 = lastPath[i] + left;
            p4 = lastPath[i] + right;

            if (i == 1)
            {
                p1 = lastPath[i - 1] + left;
                p2 = lastPath[i - 1] + right;
                verts[vertIndex++] = p1;
                verts[vertIndex++] = p2;
                uvs[vertIndex - 2] = new Vector2(0, 0);
                uvs[vertIndex - 1] = new Vector2(1, 0);
            }

            verts[vertIndex++] = p3;
            verts[vertIndex++] = p4;

            tris[triIndex++] = vertIndex - 4; // p1
            tris[triIndex++] = vertIndex - 3; // p2
            tris[triIndex++] = vertIndex - 2; // p3

            tris[triIndex++] = vertIndex - 3; // p2
            tris[triIndex++] = vertIndex - 1; // p4
            tris[triIndex++] = vertIndex - 2; // p3

            float completionPercent = i / (float)(lastPath.Count - 1);
            //float v = 1 - Mathf.Abs(2 * completionPercent - 1);
            uvs[vertIndex - 2] = new Vector2(0, completionPercent); // p3
            uvs[vertIndex - 1] = new Vector2(1, completionPercent); // p4
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;

        return mesh;
    }

    public void UpdateRoad()
    {
        curve = Curve.GenerateFromPointsQuadratic(GetMainPoints());
        lastPath = curve.GeneratePath();

        UpdateMesh();
    }

    public List<Vector3> GetMainPoints()
    {
        List<Vector3> res = new List<Vector3>();
        for (int i = 0; i < transform.childCount; i++)
        {
            res.Add(transform.GetChild(i).position);
        }
        return res;
    }


    public List<Vector3> GetPath()
    {
        return lastPath;
    }

    public Curve GetCurve()
    {
        return curve;
    }



}
