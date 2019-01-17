using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Road road;
    //public float speed = 0.01f;
    List<Vector3> path = null;

    int curIndexPath = 2;
    // Start is called before the first frame update
    void Start()
    {
        path = road.GetPath();
        Debug.Log($"count={path.Count}");

        if (IsEnd())
            return;

        transform.position = path[curIndexPath];
        SetupPoint();


    }

    bool IsEnd()
    {
        return curIndexPath + 2 >= path.Count;
    }

    Quaternion start;
    Quaternion end;
    void SetupPoint()
    {
        if (IsEnd())
            return;

        var pos = transform.position;
        var rot = transform.rotation;

        transform.position = path[curIndexPath];
        transform.LookAt(path[curIndexPath + 1]);
        start = transform.rotation;

        transform.position = path[curIndexPath + 1];
        transform.LookAt(path[curIndexPath + 2]);
        end = transform.rotation;

        transform.position = pos;
        transform.rotation = start;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsEnd())
            return;

        transform.LookAt(path[curIndexPath + 1]);
        var prevRotAFterLook = transform.rotation;
        transform.position += transform.forward * Time.deltaTime * 25f;
        transform.LookAt(path[curIndexPath + 1]);

        if (prevRotAFterLook != transform.rotation)
        {
            curIndexPath += 1;
            transform.position = path[curIndexPath];
            Debug.Log($"new point: {curIndexPath}");
            SetupPoint();
        }

        if (IsEnd())
            return;

        var destPos = path[curIndexPath + 1];
        var fulldist = (path[curIndexPath + 1] - path[curIndexPath]).magnitude;
        var dist = (destPos - transform.position).magnitude;
        float finished = 1- dist / fulldist;
        Debug.Log($"finished: {finished}, dist={dist}, fulldist={fulldist}");

        transform.rotation = Quaternion.Lerp(start, end, finished);



    }
}
