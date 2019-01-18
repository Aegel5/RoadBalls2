using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPlayer : MonoBehaviour
{
    public Road road;
    SegmentList segments;
    Segment curSeg;
    bool isFinish = false;
    float radius = 0f;
    public Transform ball;
    float sideDist = 0;

    int curIndexSeg = 0;
    float curSegTime = 0.05f;

    void Start()
    {
        segments = road.GenerateSegmentList();

        var sphereCollider = ball.GetComponent<SphereCollider>();
        radius = Mathf.Max(sphereCollider.transform.lossyScale.x, sphereCollider.transform.lossyScale.x, sphereCollider.transform.lossyScale.x) * sphereCollider.radius;
        Debug.Log($"radius is: {radius}");

        if (segments.Size == 0)
        {
            isFinish = true;
            return;
        }

        SetupSegment();

    }

    void SetupSegment()
    {
        curSegTime = 0;
        curSeg = segments.GetSegment(curIndexSeg);
        transform.position = curSeg.Interpolate(curSegTime);
    }


    bool IsEnd()
    {
        return isFinish;
    }

    void SetPos(Vector3 pos)
    {

    }

    void HandleInput()
    {
        var move = Input.GetAxis("Horizontal");
        if (move == 0)
            return;
        sideDist += move * Time.deltaTime * 2;
        sideDist = Mathf.Clamp(sideDist, -.7f, .7f);

    }



    void Update()
    {
        if (IsEnd())
            return;

        HandleInput();

        float distForFrame = Time.deltaTime * 20f;
        var res = curSeg.FindPointByMagnitude(curSegTime, distForFrame);
        if (res.time == 1)
        {
            Debug.Log("finish1");
            curIndexSeg += 1;
            // конец сегмента переходим на следующий
            if (curIndexSeg == segments.Size)
            {
                Debug.Log("finish2");
                curIndexSeg = 0;
                //isFinish = true;
                //return;
            }
            SetupSegment();
            if (res.actualMagnitude < distForFrame)
            {
                distForFrame -= res.actualMagnitude;
                res = curSeg.FindPointByMagnitude(curSegTime, distForFrame);
            }
        }

        var circle = 2 * Mathf.PI * radius;
        var period = distForFrame / circle;
        var grad = 360f * period / 10f;
        ball.Rotate(20f*Time.deltaTime*20, 0, 0, Space.Self);

        float nextTimePoint = curSegTime + (res.time - curSegTime) * 2;
        if (nextTimePoint > 1)
            nextTimePoint = 1;
        var nextPoint = curSeg.Interpolate(nextTimePoint);
        var forward = nextPoint - res.pos;
        var left = Vector3.Cross(Vector3.up, forward).normalized;
        var up = Vector3.Cross(forward, left).normalized * (radius - 0.05f);
        var delt = up + left * sideDist;
        transform.position = res.pos + delt;
        transform.LookAt(nextPoint + delt);
        curSegTime = res.time;
    }
}
