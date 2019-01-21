using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier
{

    public static Vector3 EvaluateQuadratic(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 p0 = Vector3.Lerp(a, b, t);
        Vector3 p1 = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(p0, p1, t);
    }

    public static Vector3 EvaluateCubic(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 p0 = EvaluateQuadratic(a, b, c, t);
        Vector3 p1 = EvaluateQuadratic(b, c, d, t);
        return Vector3.Lerp(p0, p1, t);
    }

    public static Vector3 TangentQuadratic(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        return 2 * (1 - t) * (b - a) + 2 * t * (c - b);
    }

    public static Vector3 TangentCubic(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        return 3 * (1 - t) * (1 - t) * (b - a) + 6 * (1 - t) * t * (c - b) + 3 * t * t * (d - c);
    }

    public static (Vector3 p1, Vector3 p2) CalcControlPointsForCubic(Vector3 point, Vector3? prevPoint, Vector3? nextPoint)
    {
        var p1 = point;
        var p2 = point;

        if(prevPoint.HasValue && nextPoint.HasValue)
        {
            var left = prevPoint.Value - point;
            var right = nextPoint.Value - point;

            var leftnorm = left.normalized;
            var rightnorm = right.normalized;

            var orto = Vector3.Cross(rightnorm, leftnorm).normalized;
            var mediana = (leftnorm + rightnorm).normalized;

            var vect1 = Vector3.Cross(orto, mediana);
            var vect2 = -vect1;

            p1 = point + vect1.normalized * left.magnitude / 2;
            p2 = point + vect2.normalized * right.magnitude / 2;

            //Debug.Log($"p={point}, prev={prevPoint}, next={nextPoint}, left={left}, right={right}, orto={orto}, med={mediana}, vect1={vect1}, vect2={vect2}, p1={p1}, p2={p2}");
        }

        return (p1, p2);
    }
}
