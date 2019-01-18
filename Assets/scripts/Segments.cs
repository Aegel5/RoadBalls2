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

static class SegmentHelper
{
    public static float sumErr;
    public static int countErr;
    public static float AvgErr { get { return sumErr / countErr; } }
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

        //Debug.Log($"create seg {start} {control} {end}");

    }

    public Vector3 Interpolate(float t)
    {
        return Bezier.EvaluateQuadratic(start, control, end, t);
    }

    public struct FindPointRes
    {
        public Vector3 pos;
        public float time;
        public float actualMagnitude;
    }
    public FindPointRes FindPointByMagnitude(float fromTime, float magnitude)
    {
        return FindPointByMagnitude(fromTime, Interpolate(fromTime), magnitude);
    }

    public FindPointRes FindPointByMagnitude(float fromTime, Vector3 fromPos, float magnitude)
    {
        FindPointRes res = new FindPointRes();
        if (fromTime >= 1)
        {
            res.time = 1;
            res.actualMagnitude = 0;
            return res;
        }
        float curTimeDelta = 1.01f - fromTime;
        float curTime = fromTime + curTimeDelta;  // ~ 1.01f
        float minDeltaMagnitude = float.MaxValue;
        for (int i = 0; i < 15; i++)
        {
            float curTClipped = Mathf.Min(1f, curTime);
            var pos = Interpolate(curTClipped);
            var curMagn = (pos - fromPos).magnitude;
            var curDelta = Mathf.Abs(curMagn - magnitude);

            if (curDelta < minDeltaMagnitude)
            {
                res.actualMagnitude = curMagn;
                res.pos = pos;
                res.time = curTClipped;
                minDeltaMagnitude = curDelta;
            }

            if (curMagn == magnitude)
                break;

            curTimeDelta /= 2;

            if (curMagn > magnitude)
            {
                curTime -= curTimeDelta;
            }
            else
            {
                if (curTime >= 1)
                {
                    // подошли к концу сегмента
                    Debug.Log("return end");
                    res.time = 1;
                    res.pos = Interpolate(1);
                    res.actualMagnitude = (res.pos - fromPos).magnitude;
                    return res;
                }
                if (i == 0)
                {
                    throw new System.Exception("internal error: алгоритм работает только на уменьшение magnitude");
                }
                curTime += curTimeDelta;
            }
        }

        //SegmentHelper.sumErr = Mathf.Abs(res.actualMagnitude - magnitude);
        //SegmentHelper.countErr += 1;
        //Debug.Log($"aerr={SegmentHelper.AvgErr} ({SegmentHelper.sumErr})");
        return res;
    }
}
