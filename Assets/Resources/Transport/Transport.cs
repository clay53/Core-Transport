using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transport : MonoBehaviour
{
    public GameObject core;
    Vector2 moveAxis = new Vector2(0, 0);
    public ContiguousParts contiguousParts;

    void Start ()
    {
        contiguousParts = gameObject.AddComponent<ContiguousParts>();
        core = GameObject.Instantiate(Resources.Load<GameObject>("Transport/Parts/Core/CorePrefab"), gameObject.transform);
        Destroy(core.GetComponent<Rigidbody>());
    }

    public void OnMoveAxis(Vector2 moveAxis)
    {
        foreach (Part part in contiguousParts.GetChildren())
        {
            part.OnMove(moveAxis);
        }
    }
}