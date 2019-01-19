using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    Road road;
    public Transform badBall;
    SegmentCurve segments;

    List<Transform> objs = new List<Transform>();

    public static GameController Inst;

    private void Awake()
    {
        Inst = this;
        road = GameObject.FindGameObjectWithTag("Road").GetComponent<Road>();
        segments = road.GetSegmentList();
    }

    public void FillRoadRandom()
    {
        Debug.Log($"len curve={segments.Len}, segcount={segments.SegCount}");

        SortedSet<int> added = new SortedSet<int>();
        for(int i = 0; i < 100; i++)
        {
            int len = (int)Random.Range(0, segments.Len);
            //Debug.Log($"len={len}");
            if (added.Contains(len))
                continue;

            float time = segments.FindTimeByLen(len);
            Debug.Log($"time={time} for len={len}");
            var pos = segments.Interpolate(time);
            var forward = segments.Forward(time);

            var left = SegmentCurve.LeftByForward(forward);
            var up = Vector3.Cross(forward, left).normalized * 0.15f;

            var part = road.MovableWidth / 4;

            var posleft = pos + left * part + up;
            var poscenter = pos + up;
            var posright = pos - left * part + up;

            var rot = Quaternion.LookRotation(forward);

            objs.Add(Instantiate(badBall, posleft, rot));
            objs.Add(Instantiate(badBall, poscenter, rot));
            objs.Add(Instantiate(badBall, posright, rot));

            added.Add(len);
        }
    }
}
