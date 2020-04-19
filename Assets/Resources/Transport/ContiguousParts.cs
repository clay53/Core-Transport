using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContiguousParts : MonoBehaviour
{
    public List<GameObject> collidingGameObjects = new List<GameObject>();
    void OnCollisionEnter(Collision col)
    {
        collidingGameObjects.Add(col.gameObject);
    }

    void OnCollisionExit(Collision col)
    {
        collidingGameObjects.Remove(col.gameObject);
    }

    public Part[] GetChildren ()
    {
        return transform.GetComponentsInChildren<Part>();
    }

    public bool HasCore()
    {
        foreach (Part child in GetChildren())
        {
            if (child.title == "Core")
            {
                return true;
            }
        }
        return false;
    }

    public bool Intersects(ContiguousParts parts)
    {
        return collidingGameObjects.Contains(parts.gameObject) || parts.collidingGameObjects.Contains(gameObject);
    }

    public bool Intersects(Part part)
    {
        return collidingGameObjects.Contains(part.gameObject) || part.collidingGameObjects.Contains(gameObject);
    }

    public void Merge(ContiguousParts contiguousParts)
    {
        foreach (Part part in contiguousParts.GetChildren())
        {
            part.transform.parent = gameObject.transform;
        }
        GameObject.Destroy(contiguousParts.gameObject);
    }
}
