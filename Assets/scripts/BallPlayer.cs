using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPlayer : MonoBehaviour
{
    Road road;
    public Transform ball;

    SegmentCurve segments;
    Segment curSeg;
    bool isFinish = false;
    float radius = 0f;
    float sideDist = 0;
    int curIndexSeg = 0;
    float curSegTime = 0.05f;

    private void Awake()
    {
        road = GameObject.FindGameObjectWithTag("Road").GetComponent<Road>();
    }

    void Start()
    {
        segments = road.GetSegmentList();

        var sphereCollider = ball.GetComponent<SphereCollider>();
        radius = Mathf.Max(sphereCollider.transform.lossyScale.x, sphereCollider.transform.lossyScale.x, sphereCollider.transform.lossyScale.x) * sphereCollider.radius;
        Debug.Log($"radius is: {radius}, segcount: {segments.SegCount}");

        if (segments.SegCount == 0)
        {
            isFinish = true;
            return;
        }

        SetupSegment();

        GameController.Inst.FillRoadRandom();

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

        if (curSeg == null)
            return;

        HandleInput();

        float distForFrame = Time.deltaTime * 20f;
        var res = curSeg.FindPointByMagnitude(curSegTime, distForFrame);
        if (res.time == 1)
        {
            // конец сегмента переходим на следующий

            curIndexSeg += 1;
            Debug.Log($"go to seg {curIndexSeg}");

            if (curIndexSeg == segments.SegCount)
            {
                Debug.Log("finish trace");
                curIndexSeg = 0;
            }
            SetupSegment();
            var actualMagn = Mathf.Sqrt(res.actualSqrMagnitude);
            if (actualMagn < distForFrame)
            {
                distForFrame -= actualMagn;
                res = curSeg.FindPointByMagnitude(curSegTime, distForFrame);
            }
        }

        var circle = 2 * Mathf.PI * radius;
        var period = distForFrame / circle;
        var grad = 360f * period / 10f;
        ball.Rotate(400f * Time.deltaTime, 0, 0, Space.Self);

        var forward = curSeg.Tangent(res.time);
        var left = SegmentCurve.LeftByForward(forward);
        var up = Vector3.Cross(forward, left).normalized * (radius - 0.05f);
        var delt = up + left * sideDist;
        transform.position = res.pos + delt;
        transform.rotation = Quaternion.LookRotation(forward);

        curSegTime = res.time;
    }
}
