using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Part : MonoBehaviour
{
    public string title;
    public List<GameObject> snapPoints = new List<GameObject>();
    public bool snapPointSelected1 = false;
    public bool snapPointSelected2 = false;
    public GameObject selectedSnapPoint;
    public Player player;
    public List<PartConnection> connections = new List<PartConnection>();
    public List<GameObject> collidingGameObjects = new List<GameObject>();

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        foreach (SnapPoint snapPoint in transform.GetComponentsInChildren<SnapPoint>())
        {
            snapPoints.Add(snapPoint.gameObject);
        }
    }

    public void Update()
    {
        snapPointSelected1 = snapPoints.Contains(player.selectedSnapPoint1);
        snapPointSelected2 = snapPoints.Contains(player.selectedSnapPoint2);
        selectedSnapPoint = snapPointSelected1 ? player.selectedSnapPoint1 : (snapPointSelected2 ? player.selectedSnapPoint2 : null);
        foreach (GameObject snapPoint in snapPoints)
        {
            SnapPoint snapPointScript = snapPoint.GetComponent<SnapPoint>();
            if (snapPoint == selectedSnapPoint)
            {
                snapPointScript.SetView(true, snapPointSelected1 ? SnapPoint.SnapPointMaterials.selected1Material : SnapPoint.SnapPointMaterials.selected2Material);
            } else if (!snapPointSelected1 && !snapPointSelected2)
            {
                snapPointScript.SetView(snapPointScript.lookingAt, SnapPoint.SnapPointMaterials.baseMaterial);
            } else
            {
                snapPointScript.SetView(false, SnapPoint.SnapPointMaterials.baseMaterial);
            }
        }
    }

    public ContiguousParts GetContiguousParts()
    {
        return transform.GetComponentInParent<ContiguousParts>();
    }

    public bool Intersects(ContiguousParts parts)
    {
        return collidingGameObjects.Contains(parts.gameObject) || parts.collidingGameObjects.Contains(gameObject);
    }

    public bool Intersects (Part part)
    {
        return collidingGameObjects.Contains(part.gameObject) || part.collidingGameObjects.Contains(gameObject);
    }

    void OnCollisionEnter(Collision col)
    {
        collidingGameObjects.Add(col.gameObject);
    }

    void OnCollisionExit(Collision col)
    {
        collidingGameObjects.Remove(col.gameObject);
    }

    public abstract void OnMove(Vector2 axis);
}

public class PartConnection
{
    public GameObject snapPoint1;
    public GameObject SnapPoint2;
    public Vector3 angle;

    public PartConnection(GameObject snapPoint1, GameObject snapPoint2, Vector3 angle)
    {
        this.snapPoint1 = snapPoint1;
        this.SnapPoint2 = snapPoint2;
    }
}