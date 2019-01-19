using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Road road;
    public BadBall badBall;
    SegmentCurve curve;

    float sideDist = 0;
    int curIndexSeg = 0;

    const float startTime = 0.05f;
    float curTime = startTime;
    float waitTime = 0;

    bool isFinish;

    private void Awake()
    {
        road = GameObject.FindGameObjectWithTag("Road").GetComponent<Road>();
    }

    void Start()
    {
        curve = road.GetCurve();

        if (curve.SegCount == 0)
        {
            isFinish = true;
            return;
        }
    }

    bool IsEnd()
    {
        return isFinish;
    }

    void HandleInput()
    {
        float pointer_x = 0;
        if (Input.touchCount > 0)
        {
            pointer_x = Input.touches[0].deltaPosition.x;
            pointer_x /= 4;
        }
        else
        {
            pointer_x = Input.GetAxis("Horizontal");
            pointer_x *= 2;
        }


        if (pointer_x == 0)
            return;

        sideDist += pointer_x * Time.deltaTime * 2;
        sideDist = Mathf.Clamp(sideDist, -.65f, .65f);

    }

    public void Collision (BadBall ball)
    {
        if(badBall.ColorType != ball.ColorType)
        {
            curTime = startTime;
            waitTime = Time.time+2;
        }
    }



    void Update()
    {
        if (IsEnd())
            return;

        if (Time.time < waitTime)
            return;

        HandleInput();

        float distForFrame = Time.deltaTime * 20f;

        var res = curve.FindPointByMagnitude(curTime, distForFrame);
        var pos = res.pos;
        if (res.isend)
        {
            curTime = startTime;
            pos = curve.Interpolate(curTime);
        }
        else
        {
            curTime = res.time;
        }

        var circle = 2 * Mathf.PI * badBall.Radius;
        var period = distForFrame / circle;
        var grad = 360f * period / 10f;
        badBall.transform.Rotate(400f * Time.deltaTime, 0, 0, Space.Self);

        var forward = curve.Forward(curTime);
        var left = SegmentCurve.LeftByForward(forward);
        var up = Vector3.Cross(forward, left).normalized * (badBall.Radius);
        transform.position = pos + up + left * sideDist;
        transform.rotation = Quaternion.LookRotation(forward);
    }
}
