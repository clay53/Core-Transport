using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    private Player player;
    private new MeshRenderer renderer;
    public Material baseMaterial;
    public Material selected1Material;
    public Material selected2Material;
    public enum SnapPointMaterials
    {
        baseMaterial,
        selected1Material,
        selected2Material
    }
    public bool lookingAt = false;
    public bool selected1 = false;
    public bool selected2 = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        renderer = gameObject.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lookingAt = player.lookingAtSnapPoint == gameObject;
        selected1 = player.selectedSnapPoint1 == gameObject;
        selected2 = player.selectedSnapPoint2 == gameObject;
    }

    public void SetView(bool visible, SnapPointMaterials material)
    {
        renderer.enabled = visible;
        switch (material)
        {
            case SnapPointMaterials.baseMaterial:
                renderer.material = baseMaterial;
                break;
            case SnapPointMaterials.selected1Material:
                renderer.material = selected1Material;
                break;
            case SnapPointMaterials.selected2Material:
                renderer.material = selected2Material;
                break;
            default:
                throw new System.ArgumentException();
        }
    }
}
