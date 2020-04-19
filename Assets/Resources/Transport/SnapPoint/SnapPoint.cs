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

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        renderer = gameObject.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        bool lookingAt = player.lookingAtSnapPoint == gameObject;
        bool selected1 = player.selectedSnapPoint1 == gameObject;
        bool selected2 = player.selectedSnapPoint2 == gameObject;
        renderer.enabled = (lookingAt || selected1 || selected2);
        renderer.material = selected1 ? selected1Material : (selected2 ? selected2Material : baseMaterial);
    }
}
