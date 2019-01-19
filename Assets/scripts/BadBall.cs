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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateColor()
    {
        GetComponent<MeshRenderer>().material.color = GameController.Inst.GetColorForType(ColorType);

    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        var player = transform.parent.GetComponent<Player>();
        if (player == null)
            return;
        var otherball = other.GetComponent<BadBall>();
        player.Collision(otherball);
    }
}
