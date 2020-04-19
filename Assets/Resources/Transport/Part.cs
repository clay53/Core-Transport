using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : MonoBehaviour
{
    public string title;
    public GameObject[] snapPoints;
    public bool snapPointSelected1 = false;
    public bool snapPointSelected2 = false;
    public GameObject selectedSnapPoint;
    private Player player;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
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
                Debug.Log(snapPointScript.lookingAt);
                snapPointScript.SetView(snapPointScript.lookingAt, SnapPoint.SnapPointMaterials.baseMaterial);
            } else
            {
                snapPointScript.SetView(false, SnapPoint.SnapPointMaterials.baseMaterial);
            }
        }
    }
}