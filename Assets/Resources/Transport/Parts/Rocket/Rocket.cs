using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Part
{
    public float force = 1000;

    override public void OnMove(Vector2 moveAxis)
    {
        if (moveAxis.y > 0)
        {
            GetComponentInParent<Rigidbody>().AddForceAtPosition(-transform.up * force * Time.deltaTime, transform.position);
        }
    }
}
