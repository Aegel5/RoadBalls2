using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    Road road;
    Curve curve;
    SortedDictionary<int, object> objs = new SortedDictionary<int, object>();
    GameBase gameBase;
    Transform objRoot;

    class ElementThreeLine
    {
        public BadBall[] balls = new BadBall[3];
        public void SetColorSeq(int[] colorseq)
        {
            balls[0].ColorType = (BallColorType)colorseq[0];
            balls[1].ColorType = (BallColorType)colorseq[1];
            balls[2].ColorType = (BallColorType)colorseq[2];
        }
    }
    class ElementColorChanger
    {
        public ColorChanger changer;
    }

    int[][] colorSeq =
    {
        new int[] { 0, 1, 2 },
        new int[] { 0, 2, 1 },
        new int[] { 1, 0, 2 },
        new int[] { 1, 2, 0 },
        new int[] { 2, 1, 0 },
        new int[] { 2, 0, 1 },
        };

    public void FillRoadRandom()
    {
        road = GameObject.FindGameObjectWithTag("Road").GetComponent<Road>();
        curve = road.GetCurve();
        gameBase = GameObject.FindGameObjectWithTag("GameBase").GetComponent<GameBase>();
        objRoot = Utils.RecursiveFindChild(transform, "objRoot");

        Debug.Log($"len curve={curve.Len}, segcount={curve.SegCount}");

        objs.Clear();
        Utils.DeleteAllChildren(objRoot.transform, true);

        System.Random rnd = new System.Random();

        for (int i = 0; i < 110; i++)
        {
            int len = (int)Random.Range(20, curve.Len);
            if (objs.ContainsKey(len))
            {
                //i--;
                continue;
            }

            float time = curve.CalcTimeByLen(len);
            var pos = curve.Interpolate(time);
            var forward = curve.Forward(time);

            var left = Curve.LeftByForward(forward);
            var up = Vector3.Cross(forward, left).normalized * 0.15f;

            var part = road.Width / 4;

            var posleft = pos + left * part + up;
            var poscenter = pos + up;
            var posright = pos - left * part + up;

            var rot = Quaternion.LookRotation(forward);

            if (i < 6)
            {
                ElementColorChanger changer = new ElementColorChanger();
                var obj = Instantiate<ColorChanger>(gameBase.colorChangerPrefab, poscenter, rot, objRoot);
                var seq = colorSeq[0];
                obj.ColorType = (BallColorType)seq[rnd.Next(0, seq.Length)];
                objs.Add(len, changer);
            }
            else
            {
                ElementThreeLine line = new ElementThreeLine();

                void AddObj(Vector3 position, int index)
                {
                    var obj = Instantiate<BadBall>(gameBase.badBallPrefab, position, rot, objRoot);
                    line.balls[index] = obj;
                }

                AddObj(posleft, 0);
                AddObj(poscenter, 1);
                AddObj(posright, 2);

                objs.Add(len, line);
            }
        }

        int prevlen = 0;
        var curColorSeq = colorSeq[0];
        foreach (var len in objs.Keys)
        {
            var threeline = objs[len] as ElementThreeLine;
            if (threeline != null)
            {
                if (len - prevlen > 9)
                {
                    curColorSeq = colorSeq[rnd.Next(0, colorSeq.Length)];
                }
                threeline.SetColorSeq(curColorSeq);
            }
            prevlen = len;
        }
    }
}
