using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transport : MonoBehaviour
{
    public GameObject core;
    Vector2 moveAxis = new Vector2(0, 0);

    void Start ()
    {
        core = GameObject.Instantiate(Resources.Load<GameObject>("Transport/Parts/Core/CorePrefab"), gameObject.transform);
    }

    public void SetMoveAxis(Vector2 moveAxis)
    {
        this.moveAxis = moveAxis;
    }
}