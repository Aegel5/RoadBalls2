using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadBall : MonoBehaviour
{
    public float Radius { get; private set; }

    [SerializeField]
    public BallColorType ColorType = BallColorType.Type1;

    // Start is called before the first frame update
    void Start()
    {
        var sphereCollider = GetComponent<SphereCollider>();
        Radius = Mathf.Max(sphereCollider.transform.lossyScale.x, sphereCollider.transform.lossyScale.x, sphereCollider.transform.lossyScale.x) * sphereCollider.radius;
        UpdateColor();
    }

    public void UpdateColor()
    {
        GetComponent<MeshRenderer>().material.color = GameBase.Inst.GetColorForType(ColorType);
    }
}
