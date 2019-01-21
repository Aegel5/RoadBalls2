using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [SerializeField]
    public BallColorType ColorType = BallColorType.Type1;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material.color = GameBase.Inst.GetColorForType(ColorType);
    }
}
