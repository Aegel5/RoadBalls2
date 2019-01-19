using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

//public class CurvePos
//{
//    public Segment seg;
//    public float t;
//}

public struct FindPointRes
{
    public Vector3 pos;
    public float time;
    public float actualSqrMagnitude;
    public bool isend;
}

[Serializable]
public class SegmentCurve 
{
    [SerializeField]
    List<Segment> segments = new List<Segment>();

    [SerializeField]
    public int SegCount { get { return segments.Count; } }
    public Segment GetSegment(int index)
    {
        return segments[index];
    }

    [SerializeField]
    public float Len; /*{ get; private set; }*/

    IEnumerable<Vector3> GeneratePathForSegment(Segment seg, bool addLast)
    {
        int count = 61;
        double step = 1d / (count - 1);
        for (int i = 0; i < count; i++)
        {
            float t = (float)(i * step);
            if (i == count - 1)
                t = 1;
            var point = seg.Interpolate(t);
            if (t >= 1)
            {
                if (addLast)
                    yield return point;
            }
            else
            {
                yield return point;
            }
        }
        yield break;
    }

    static public Vector3 LeftByForward(Vector3 forward)
    {
        return Vector3.Cross(Vector3.up, forward).normalized;
    }

    struct CurvePos
    {
        public Segment seg;
        public float t;
        public int segIndex;
    }

    CurvePos GetPos(float time)
    {
        CurvePos pos = new CurvePos();
        int segIndex = (int)time;
        float timeSeg = time - segIndex;

        if(segIndex >= segments.Count)
        {
            segIndex = segments.Count - 1;
            timeSeg = 1;
        }
        pos.seg = segments[segIndex];
        pos.t = timeSeg;
        pos.segIndex = segIndex;

        return pos;
    }

    public Vector3 Forward(float time)
    {
        var pos = GetPos(time);
        return pos.seg.Tangent(pos.t);
    }

    public Vector3 Interpolate(float time)
    {
        var pos = GetPos(time);
        return pos.seg.Interpolate(pos.t);
    }

    public FindPointRes FindPointByMagnitude(float fromTime, float magnitude)
    {
        var pos = GetPos(fromTime);
        int segIndex = pos.segIndex;
        var res = segments[segIndex].FindPointByMagnitude(pos.t, magnitude);
        if (res.time == 1)
        {
            if (pos.segIndex == segments.Count)
            {
                res.isend = true;
            }
            else
            {
                var actualMagn = Mathf.Sqrt(res.actualSqrMagnitude);
                if (actualMagn < magnitude)
                {
                    magnitude -= actualMagn;
                    segIndex += 1;
                    res = segments[segIndex].FindPointByMagnitude(0, magnitude);
                }
            }
        }

        res.time += segIndex;

        return res;
    }

    public float FindTimeByLen(float len)
    {
        float time = 0;
        float curlen = 0;
        for(int i = 0; i < segments.Count; i++)
        {
            var seg = segments[i];
            curlen += seg.Len;
            //Debug.Log($"curlen={curlen}");
            if(len <= curlen)
            {
                var t = 1 - (curlen - len) / seg.Len;
                if (t < 0 || t > 1)
                    throw new Exception($"bad time: {t}");
                time += t;
                break;
            }
            else
            {
                time += 1;
            }
        }

        return time;
    }

    public List<Vector3> GeneratePath()
    {
        var path = new List<Vector3>();
        float alllen = 0;
        for (int i = 0; i < segments.Count; i++)
        {
            var seg = segments[i];
            bool isLast = i == segments.Count - 1;

            float len = 0;
            Vector3? prev = null;
            foreach (var p in GeneratePathForSegment(seg, isLast))
            {
                if (prev != null)
                {
                    len += Vector3.Distance(prev.Value, p);
                }
                path.Add(p);
                prev = p;
            }

            seg.SetLen(len);

            alllen += len;
        }

        Debug.Log($"set alllen={alllen}");
        Len = alllen;

        return path;
    }

    static public SegmentCurve GenerateFromPoints(IList<Vector3> points)
    {
        SegmentCurve segList = new SegmentCurve();

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

[Serializable]
public class Segment
{
    [SerializeField]
    Vector3 start;
    [SerializeField]
    Vector3 end;
    [SerializeField]
    Vector3 control;

    [SerializeField]
    public float Len; /*{ get; private set; }*/

    public Segment(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        start = p1;
        control = p2;
        end = p3;

        //Debug.Log($"create seg {start} {control} {end}");

    }

    public void SetLen(float len)
    {
        Len = len;
    }

    public Vector3 Interpolate(float t)
    {
        return Bezier.EvaluateQuadratic(start, control, end, t);
    }


    public FindPointRes FindPointByMagnitude(float fromTime, float magnitude)
    {
        return FindPointByMagnitude(fromTime, Interpolate(fromTime), magnitude);
    }

    public Vector3 Tangent(float t)
    {
        return Bezier.TangentQuadratic(start, control, end, t);
    }

    public FindPointRes FindPointByMagnitude(float fromTime, Vector3 fromPos, float magnitude)
    {
        var sqrMag = magnitude * magnitude;
        FindPointRes res = new FindPointRes();
        if (fromTime >= 1)
        {
            res.time = 1;
            res.actualSqrMagnitude = 0;
            return res;
        }
        float curTimeDelta = 1.01f - fromTime;
        float curTime = fromTime + curTimeDelta;  // ~ 1.01f
        float minDeltaMagnitude = float.MaxValue;
        for (int i = 0; i < 15; i++)
        {
            float curTClipped = Mathf.Min(1f, curTime);
            var pos = Interpolate(curTClipped);
            var curSqrMagn = (pos - fromPos).sqrMagnitude;
            var curDelta = Mathf.Abs(curSqrMagn - sqrMag);

            if (curDelta < minDeltaMagnitude)
            {
                res.actualSqrMagnitude = curSqrMagn;
                res.pos = pos;
                res.time = curTClipped;
                minDeltaMagnitude = curDelta;
            }

            if (curSqrMagn == sqrMag)
                break;

            curTimeDelta /= 2;

            if (curSqrMagn > sqrMag)
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
                    res.actualSqrMagnitude = (res.pos - fromPos).sqrMagnitude;
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
        //Debug.Log($"aerr={SegmentHelper.AvgErr} ({SegmentHelper.countErr})");
        return res;
    }
}
