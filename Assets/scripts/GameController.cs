using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallColorType
{
    Type1 = 0,
    Type2 = 1,
    Type3 = 2,
}

public class GameController : MonoBehaviour
{
    public Material ballMaterial;

    public Color Type1;
    public Color Type2;
    public Color Type3;

    Road road;
    public BadBall badBall;
    public Transform ballRoot;
    SegmentCurve curve;

    int[][] colorSeq =
        {
        new int[] { 0, 1, 2 },
        new int[] { 0, 2, 1 },
        new int[] { 1, 0, 2 },
        new int[] { 1, 2, 0 },
        new int[] { 2, 1, 0 },
        new int[] { 2, 0, 1 },
        };

    class TreeLine
    {
        public BadBall[] balls = new BadBall[3];
        public void SetColorSeq(int[] colorseq)
        {
            balls[0].ColorType = (BallColorType)colorseq[0];
            balls[1].ColorType = (BallColorType)colorseq[1];
            balls[2].ColorType = (BallColorType)colorseq[2];
        }
    }

    SortedDictionary<int, TreeLine> objs = new SortedDictionary<int, TreeLine> ();

    public static GameController Inst;

    Material[] colorsMat = new Material[3];


    public Color GetColorForType(BallColorType colortype)
    {
        if (colortype == BallColorType.Type1)
            return Type1;
        else if (colortype == BallColorType.Type2)
            return Type2;
        else if (colortype == BallColorType.Type3)
            return Type3;

        throw new System.Exception();
    }


    private void Init()
    {
        Inst = this;
        road = GameObject.FindGameObjectWithTag("Road").GetComponent<Road>();
        curve = road.GetCurve();
    }

    private void Awake()
    {
        Init();
    }


    public void FillRoadRandom()
    {
        Init();

        Debug.Log($"len curve={curve.Len}, segcount={curve.SegCount}");

        objs.Clear();
        Utils.DeleteAllChildren(ballRoot.transform, true);


        for(int i = 0; i < 100; i++)
        {
            int len = (int)Random.Range(30, curve.Len);
            //Debug.Log($"len={len}");
            if (objs.ContainsKey(len))
            {
                i--;
                continue;
            }

            float time = curve.FindTimeByLen(len);
            //Debug.Log($"time={time} for len={len}");
            var pos = curve.Interpolate(time);
            var forward = curve.Forward(time);

            var left = SegmentCurve.LeftByForward(forward);
            var up = Vector3.Cross(forward, left).normalized * 0.15f;

            var part = road.Width / 4;

            var posleft = pos + left * part + up;
            var poscenter = pos + up;
            var posright = pos - left * part + up;

            var rot = Quaternion.LookRotation(forward);

            TreeLine line = new TreeLine();

            void AddObj(Vector3 position, int index)
            {
                var obj = Instantiate<BadBall>(badBall, position, rot, ballRoot);
                line.balls[index] = obj;
            }

            AddObj(posleft, 0);
            AddObj(poscenter, 1);
            AddObj(posright, 2);

            objs.Add(len, line);
        }
        System.Random rnd = new System.Random();
        int prevlen = 0;
        var curColorSeq = colorSeq[0];
        foreach (var len in objs.Keys)
        {
            if(len - prevlen > 10)
            {
                prevlen = len;
                curColorSeq = colorSeq[rnd.Next(0, colorSeq.Length)];
            }
            objs[len].SetColorSeq(curColorSeq);

            prevlen = len;
        }
    }
}
