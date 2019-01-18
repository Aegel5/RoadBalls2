using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SegmentList 
{
    List<Segment> segments = new List<Segment>();

    public int Size { get { return segments.Count; } }
    public Segment GetSegment(int index)
    {
        return segments[index];
    }

    static public SegmentList GenerateFromPoints(IList<Vector3> points)
    {
        SegmentList segList = new SegmentList();

        if (!points.Any())
            return segList;

        int count = points.Count;
        Vector3 p1 = points[0];
        Vector3 p2;
        for (int i = 2; i < count; i++)
        {
            bool isLast = i == count - 1;
            var prevPos = points[i - 1];

            var curPos = points[i];

            p2 = prevPos + (curPos - prevPos) / 2;


            Segment seg = new Segment(p1, prevPos, p2);
            segList.segments.Add(seg);

            p1 = p2;
        }

        return segList;
    }
}

public class Segment
{
    Vector3 start;
    Vector3 end;

    Vector3 control;

    public Segment(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        start = p1;

        control = p2;

        end = p3;

        Debug.Log($"create seg {start} {control} {end}");

    }

    public Vector3 Interpolate(float t)
    {
        return Bezier.EvaluateQuadratic(start, control, end, t);
    }
}
