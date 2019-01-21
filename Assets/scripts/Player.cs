using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Road road;
    BadBall playerBall;
    Curve curve;

    float sideDist;
    float curTime;
    float waitTime;
    float curLen;

    bool isFinish;

    private void Awake()
    {
        road = GameObject.FindGameObjectWithTag("Road").GetComponent<Road>();
        playerBall = Utils.RecursiveFindChild<BadBall>(transform, "PlayerBall");
        curve = road.GetCurve();
    }

    void Start()
    {
        if (curve.IsEmpty())
        {
            isFinish = true;
            return;
        }

        MoveToStart();
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

    void MoveToStart()
    {
        curTime = 0.05f;
        transform.position = curve.Interpolate(curTime);
        waitTime = 0;
        playerBall.ColorType = BallColorType.Type1;
        playerBall.UpdateColor();
        sideDist = 0;
        curLen = 5f;
    }

    public bool HandleCollision(Collider other)
    {
        var otherball = other.GetComponent<BadBall>();
        if (otherball != null)
        {
            if (playerBall.ColorType != otherball.ColorType)
            {
                waitTime = Time.time + 2;
                Handheld.Vibrate();
                return true;
            }
            else
            {
            }
        }
        else
        {
            var changer = other.GetComponent<ColorChanger>();
            if (changer != null)
            {
                playerBall.ColorType = changer.ColorType;
                playerBall.UpdateColor();
            }
        }

        return false;
    }


    void Update()
    {
        if (isFinish)
            return;

        if (waitTime != 0)
        {
            if (Time.time < waitTime)
            {
                // пауза 2 сек
                return;
            }
            else
            {
                MoveToStart();
            }
        }

        HandleInput();

        float distForFrame = Time.deltaTime * 20f;

        Vector3 pathPos;


        var findRes = curve.FindPointByMagnitude(curTime, distForFrame);
        pathPos = findRes.pos;
        if (findRes.isend)
        {
            MoveToStart();
        }
        else
        {
            curTime = findRes.time;
        }


        var circleLen = 2 * Mathf.PI * playerBall.Radius;
        var period = distForFrame / circleLen;
        var grad = 360f * period / 10f;
        playerBall.transform.Rotate(grad, 0, 0, Space.Self);

        var forward = curve.Forward(curTime);
        var left = Curve.LeftByForward(forward);
        var up = Vector3.Cross(forward, left).normalized * (playerBall.Radius);

        var oldPos = transform.position;
        var delta = up + left * sideDist;
        var newPos = pathPos + delta;
        var dir = newPos - oldPos;

        RaycastHit[] hitInfoList = Physics.SphereCastAll(oldPos, playerBall.Radius, dir, dir.magnitude);
        foreach (var hit in hitInfoList)
        {
            if (HandleCollision(hit.collider))
            {
                newPos = hit.point + hit.normal * playerBall.Radius;
                break;
            }
        }

        transform.rotation = Quaternion.LookRotation(forward);
        transform.position = newPos;
    }
}
