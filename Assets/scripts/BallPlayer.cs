using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPlayer : MonoBehaviour
{
    Road road;
    public Transform ball;
    SegmentCurve curve;

    float radius = 0f;
    float sideDist = 0;
    int curIndexSeg = 0;

    float curTime = 0.05f;

    bool isFinish;

    private void Awake()
    {
        road = GameObject.FindGameObjectWithTag("Road").GetComponent<Road>();
    }

    void Start()
    {
        curve = road.GetSegmentList();

        var sphereCollider = ball.GetComponent<SphereCollider>();
        radius = Mathf.Max(sphereCollider.transform.lossyScale.x, sphereCollider.transform.lossyScale.x, sphereCollider.transform.lossyScale.x) * sphereCollider.radius;
        Debug.Log($"radius is: {radius}, segcount: {curve.SegCount}");

        if (curve.SegCount == 0)
        {
            isFinish = true;
            return;
        }

        GameController.Inst.FillRoadRandom();

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

        var res = curve.FindPointByMagnitude(curTime, distForFrame);
        var pos = res.pos;
        if (res.isend)
        {
            curTime = 0.05f;
            pos = curve.Interpolate(curTime);
        }
        else
        {
            curTime = res.time;
        }

        var circle = 2 * Mathf.PI * radius;
        var period = distForFrame / circle;
        var grad = 360f * period / 10f;
        ball.Rotate(400f * Time.deltaTime, 0, 0, Space.Self);

        var forward = curve.Forward(curTime);
        var left = SegmentCurve.LeftByForward(forward);
        var up = Vector3.Cross(forward, left).normalized * (radius - 0.05f);
        transform.position = pos + up + left * sideDist;
        transform.rotation = Quaternion.LookRotation(forward);
    }
}
