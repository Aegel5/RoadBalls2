using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPlayer : MonoBehaviour
{
    public Road road;
    public Transform ball;

    SegmentList segments;
    Segment curSeg;
    bool isFinish = false;
    float radius = 0f;
    float sideDist = 0;
    int curIndexSeg = 0;
    float curSegTime = 0.05f;

    void Start()
    {
        segments = road.GenerateSegmentList();

        var sphereCollider = ball.GetComponent<SphereCollider>();
        radius = Mathf.Max(sphereCollider.transform.lossyScale.x, sphereCollider.transform.lossyScale.x, sphereCollider.transform.lossyScale.x) * sphereCollider.radius;
        Debug.Log($"radius is: {radius}, segcount: {segments.Size}");

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

    void HandleInput()
    {
        float pointer_x = Input.GetAxis("Mouse X");
        float pointer_y = Input.GetAxis("Mouse Y");
        if (Input.touchCount > 0)
        {
            pointer_x = Input.touches[0].deltaPosition.x;
            pointer_y = Input.touches[0].deltaPosition.y;

            pointer_x /= 4;
            pointer_y /= 4;
        }

        if (pointer_x == 0)
            return;
        sideDist += pointer_x * Time.deltaTime * 2;
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
            // конец сегмента переходим на следующий

            curIndexSeg += 1;
            Debug.Log($"go to seg {curIndexSeg}");

            if (curIndexSeg == segments.Size)
            {
                Debug.Log("finish trace");
                curIndexSeg = 0;
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
        ball.Rotate(400f * Time.deltaTime, 0, 0, Space.Self);

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
