using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
public class Road : MonoBehaviour
{
    [SerializeField, HideInInspector]
    List<Vector3> lastPath = new List<Vector3>();

    public void CreatePoint()
    {

    }

    public void AddPoint()
    {
        var obj = new GameObject();
        obj.name = "point";
        if(transform.childCount != 0)
        {
            obj.transform.localPosition = transform.GetChild(transform.childCount - 1).transform.position;
        }
        obj.transform.SetParent(transform);
        //chil
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
        int numTris = 2 * (count - 1);
        int[] tris = new int[numTris * 3];
        int vertIndex = 0;
        int triIndex = 0;

        Vector3 p1, p2, p3, p4;
        for (int i = 1; i < lastPath.Count; i++)
        {
            Vector3 forward = lastPath[i] - lastPath[i - 1];
            //forward.Normalize();

            var left = Vector3.Cross(Vector3.up, forward).normalized;
            var right = Vector3.Cross(forward, Vector3.up).normalized;

            p3 = lastPath[i] + left;
            p4 = lastPath[i] + right;

            if (i == 1)
            {
                p1 = lastPath[i - 1] + left;
                p2 = lastPath[i - 1] + right;
                verts[vertIndex++] = p1;
                verts[vertIndex++] = p2;
            }

            verts[vertIndex++] = p3;
            verts[vertIndex++] = p4;

            tris[triIndex++] = vertIndex-4; // p1
            tris[triIndex++] = vertIndex-3; // p2
            tris[triIndex++] = vertIndex-2; // p3

            tris[triIndex++] = vertIndex -3; // p2
            tris[triIndex++] = vertIndex-1; // p4
            tris[triIndex++] = vertIndex -2; // p3
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;

        return mesh;
    }

    public void UpdateRoad()
    {
        RegenPath();
        UpdateMesh();

    }

    IEnumerable<Vector3> Interpolate(Vector3 p1, Vector3 p2, Vector3 p3, bool addLast)
    {
        int count = 61;
        double step = 1d / (count-1);
        for (int j = 0; j < count; j++)
        {
            float t = (float)(j * step);
            if (j == count - 1)
                t = 1;
            Debug.Log($"t={t}");
            var point = Bezier.EvaluateQuadratic(p1, p2, p3, t);

            if (t >= 1)
            {
                if (addLast)
                    yield return point;
            }
            else
            {
                yield return point;
                //Debug.Log($"add pint {point}");
            }
        }
        yield break;
    }

    void RegenPath()
    {
        lastPath = GeneratePath();
        //var path = GeneratePath();
        //for(int i = 0; i < path.Count; i++)
        //{
        //    Gizmos.DrawSphere(path[i], 1f);
        //    if(i > 0)
        //        Gizmos.DrawLine(path[i - 1], path[i]);
        //}
    }

    public List<Vector3> Points()
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


    public List<Vector3> GeneratePath()
    {
        List<Vector3> res = new List<Vector3>();
        int count = transform.childCount;

        Vector3 p1 = transform.GetChild(0).position;
        Vector3 p2;
        for (int i = 2; i < count; i++)
        {
            bool isLast = i == count - 1;
            var prevChild = transform.GetChild(i-1);
            var prevPos = prevChild.position;

            var curChild = transform.GetChild(i);
            var curPos = curChild.position;

            p2 = prevPos + (curPos - prevPos) / 2;

            foreach(var p in Interpolate(p1, prevPos, p2, isLast))
            {
                res.Add(p);
            }

            p1 = p2;
        }

        return res;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
