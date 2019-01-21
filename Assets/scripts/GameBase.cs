using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallColorType
{
    Type1 = 0,
    Type2 = 1,
    Type3 = 2,
}

public class GameBase : MonoBehaviour
{
    public Material ballMaterial;

    public Color Type1;
    public Color Type2;
    public Color Type3;

    public BadBall badBallPrefab;
    public ColorChanger colorChangerPrefab;

    //Material[] colorsMat = new Material[3];

    public static GameBase Inst;

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

    private void Awake()
    {
        Inst = this;
    }



}
