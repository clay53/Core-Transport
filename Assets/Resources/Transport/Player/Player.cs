using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public InputAction switchModeAction;

    public InputAction moveAction;
    public float editorMoveSpeed = 2f;
    public float editorAccelerationSpeed = 1f;
    public float editorStandingAccelerationSpeed = 0.5f;
    public Vector2 moveAxis = new Vector2(0, 0);

    public InputAction jumpAction;
    public float jumpPower = 5;

    public InputAction elevateAction;
    public float elevateSpeed;

    public enum Tool
    {
        Select,
        Place,
        Move
    }
    public Tool currentTool = Tool.Select;

    public InputAction cancelPlaceAction;
    private Vector3 oldPosition1;
    private Vector3 oldRotation1;
    private Vector3 oldPosition2;
    private Vector3 oldRotation2;

    public InputAction placeRotateXAction;
    public InputAction placeRotateYAction;
    public InputAction placeRotateZAction;
    public Vector3 placeRotation = new Vector3(0, 0, 0);

    public InputAction placeAction;

    public InputAction lookAction;
    public float lookSpeed = 0.2f;
    public GameObject lookingAt;

    public InputAction selectAction;
    public GameObject selectedSnapPoint1;
    public GameObject selectedSnapPoint2;

    public GameObject editorPlayer;
    Rigidbody editorPlayerBody;

    public Transport transport;
    public Camera cam;
    public bool editMode = false;

    public Text modeText;
    public Text controlHintsText;
    public Text infoText;

    void Start()
    {
        editorPlayerBody = editorPlayer.GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;

        switchModeAction.performed += _ => SwitchMode();
        switchModeAction.Enable();
        moveAction.Enable();
        jumpAction.Enable();
        elevateAction.Enable();
        lookAction.Enable();
        selectAction.Enable();
        placeRotateXAction.performed += _ =>
        {
            if (editMode && currentTool == Tool.Place)
            {
                placeRotation.x += placeRotateXAction.ReadValue<float>() * 90;
            }
        };
        placeRotateXAction.Enable();
        placeRotateYAction.performed += _ =>
        {
            if (editMode && currentTool == Tool.Place)
            {
                placeRotation.y += placeRotateYAction.ReadValue<float>() * 90;
            }
        };
        placeRotateYAction.Enable();
        placeRotateZAction.performed += _ =>
        {
            if (editMode && currentTool == Tool.Place)
            {
                placeRotation.z += placeRotateZAction.ReadValue<float>() * 90;
            }
        };
        placeRotateZAction.Enable();
        cancelPlaceAction.Enable();
        placeAction.Enable();
    }

    void Update()
    {
        infoText.text = "";
        infoText.color = Color.grey;
        if (editMode)
        {
            modeText.text = "Mode: Edit";
            controlHintsText.text = "TAB - Edit Mode; WASD - Move; Space - Jump; UP/DOWN - Elevate Transport";

            // Rotate camera
            Vector2 value = lookAction.ReadValue<Vector2>() * lookSpeed;
            cam.transform.rotation = Quaternion.Euler(cam.transform.rotation.eulerAngles.x - value.y, cam.transform.rotation.eulerAngles.y + value.x, 0);

            // Rotate player to camera
            Vector3 editorPlayerPos = editorPlayer.transform.position;
            cam.transform.position = new Vector3(editorPlayerPos.x, editorPlayerPos.y + 0.5f, editorPlayerPos.z);
            editorPlayer.transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);

            // Player walk / jump
            moveAxis = moveAction.ReadValue<Vector2>();
            Vector3 cV = editorPlayerBody.velocity;
            Vector3 tV = editorPlayer.transform.forward*moveAxis.y*editorMoveSpeed+ editorPlayer.transform.right*moveAxis.x*editorMoveSpeed;
            if (Physics.Raycast(editorPlayer.transform.position, -Vector3.up, editorPlayer.GetComponent<CapsuleCollider>().bounds.extents.y+0.1f) && jumpAction.ReadValue<float>() != 0)
            {
                tV.y = jumpPower;
            } else
            {
                tV.y = cV.y;
            }
            Vector3 nV = tV;
            editorPlayerBody.velocity = nV;

            // Elevate Transport
            transport.transform.position += new Vector3(0, elevateAction.ReadValue<float>() * elevateSpeed*Time.deltaTime, 0);

            // Select SnapPoints
            if (currentTool == Tool.Select)
            {
                if (selectedSnapPoint1 && cancelPlaceAction.ReadValue<float>() > 0)
                {
                    selectedSnapPoint1 = null;
                }

                if (selectedSnapPoint1)
                {
                    infoText.text = "Select 1 more point to start placement!";
                    infoText.color = Color.blue;
                } else
                {
                    infoText.text = "Select 2 points to start placement";
                    infoText.color = Color.gray;
                }
                modeText.text += " - Select";
                controlHintsText.text += "; LEFT BUTTON - Select Snap Point";
                int layerMask = 1 << 9;
                RaycastHit hitInfo;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, Mathf.Infinity, layerMask))
                {
                    lookingAt = hitInfo.collider.gameObject;

                    if (selectAction.ReadValue<float>() > 0 &&  lookingAt.tag == "SnapPoint" && selectedSnapPoint1 == null && lookingAt.GetComponent<MeshRenderer>().enabled)
                    {
                        selectedSnapPoint1 = lookingAt;
                    }
                    else if (selectAction.ReadValue<float>() > 0 && lookingAt.tag == "SnapPoint" && selectedSnapPoint1 != lookingAt && selectedSnapPoint2 == null && lookingAt.GetComponent<MeshRenderer>().enabled)
                    {
                        selectedSnapPoint2 = lookingAt;
                        currentTool = Tool.Place;
                        oldPosition1 = selectedSnapPoint1.transform.position;
                        oldRotation1 = selectedSnapPoint1.transform.rotation.eulerAngles;
                        oldPosition2 = selectedSnapPoint2.transform.position;
                        oldRotation2 = selectedSnapPoint2.transform.rotation.eulerAngles;
                    }
                }
                else
                {
                    lookingAt = null;
                }
            }

            // Place
            if (currentTool == Tool.Place && selectedSnapPoint1 && selectedSnapPoint2)
            {
                modeText.text += " - Place";
                controlHintsText.text += "; RTYFGH - Rotate Part";
                Part snap1Part = selectedSnapPoint1.GetComponent<SnapPoint>().GetPart();
                ContiguousParts snap1ContiguousParts = snap1Part.GetContiguousParts();
                Part snap2Part = selectedSnapPoint2.GetComponent<SnapPoint>().GetPart();
                ContiguousParts snap2ContiguousParts = snap2Part.GetContiguousParts();

                if ((snap1ContiguousParts && !snap2ContiguousParts) || (!snap1ContiguousParts && snap2ContiguousParts))
                {
                    bool snap1IsParent = snap1ContiguousParts;
                    ContiguousParts parentParts = snap1IsParent ? snap1ContiguousParts : snap2ContiguousParts;
                    Part parentPart = snap1IsParent ? snap1Part : snap2Part;
                    GameObject parentSnap = snap1IsParent ? selectedSnapPoint1 : selectedSnapPoint2;
                    Part childPart = snap1IsParent ? snap2Part : snap1Part;
                    Rigidbody childPartRigidBody = childPart.GetComponent<Rigidbody>();
                    GameObject childSnap = snap1IsParent ? selectedSnapPoint2 : selectedSnapPoint1;

                    childPartRigidBody.constraints = RigidbodyConstraints.FreezeAll;

                    childPart.transform.rotation = Quaternion.Euler(parentPart.transform.rotation.eulerAngles);
                    childPart.transform.Rotate(placeRotation);
                    childPart.transform.position += (parentSnap.transform.position - childSnap.transform.position);

                    if (parentParts.Intersects(childPart))
                    {
                        infoText.text = "Invalid placement!";
                        infoText.color = Color.red;
                    } else
                    {
                        infoText.text = "Press Enter to place";
                        infoText.color = Color.blue;
                        if (placeAction.ReadValue<float>() > 0)
                        {
                            childPart.connections.Add(new PartConnection(childSnap, parentSnap, placeRotation));
                            parentPart.connections.Add(new PartConnection(parentSnap, childSnap, placeRotation));
                            childPart.transform.parent = parentParts.transform;
                            Destroy(childPartRigidBody);
                            currentTool = Tool.Select;
                            selectedSnapPoint1 = null;
                            selectedSnapPoint2 = null;
                        }
                    }

                    if (cancelPlaceAction.ReadValue<float>() > 0)
                    {
                        childPartRigidBody.constraints = RigidbodyConstraints.None;
                        currentTool = Tool.Select;
                        selectedSnapPoint1 = null;
                        selectedSnapPoint2 = null;
                    }
                }
                else if (!snap1ContiguousParts && !snap2ContiguousParts)
                {
                    Part parentPart = snap1Part;
                    Rigidbody parentPartRigidBody = parentPart.GetComponent<Rigidbody>();
                    GameObject parentSnap = selectedSnapPoint1;
                    Part childPart = snap2Part;
                    Rigidbody childPartRigidBody = childPart.GetComponent<Rigidbody>();
                    GameObject childSnap = selectedSnapPoint2;

                    childPartRigidBody.constraints = RigidbodyConstraints.FreezeAll;

                    childPart.transform.rotation = Quaternion.Euler(parentPart.transform.rotation.eulerAngles);
                    childPart.transform.Rotate(placeRotation);
                    childPart.transform.position += (parentSnap.transform.position - childSnap.transform.position);

                    if (parentPart.Intersects(childPart))
                    {
                        infoText.text = "Invalid placement!";
                        infoText.color = Color.red;
                    }
                    else
                    {
                        infoText.text = "Press Enter to place";
                        infoText.color = Color.blue;
                        if (placeAction.ReadValue<float>() > 0)
                        {
                            childPart.connections.Add(new PartConnection(childSnap, parentSnap, placeRotation));
                            parentPart.connections.Add(new PartConnection(parentSnap, childSnap, placeRotation));
                            GameObject contiguousParts = Instantiate(new GameObject("Contiguous Parts"));
                            contiguousParts.AddComponent<ContiguousParts>();
                            contiguousParts.AddComponent<Rigidbody>();
                            contiguousParts.transform.rotation = parentPart.transform.rotation;
                            contiguousParts.transform.position = parentPart.transform.position;
                            childPart.transform.parent = contiguousParts.transform;
                            parentPart.transform.parent = contiguousParts.transform;
                            Destroy(parentPartRigidBody);
                            Destroy(childPartRigidBody);
                            currentTool = Tool.Select;
                            selectedSnapPoint1 = null;
                            selectedSnapPoint2 = null;
                        }
                    }

                    if (cancelPlaceAction.ReadValue<float>() > 0)
                    {
                        childPartRigidBody.constraints = RigidbodyConstraints.None;
                        currentTool = Tool.Select;
                        selectedSnapPoint1 = null;
                        selectedSnapPoint2 = null;
                    }
                }
                else if (snap1ContiguousParts && snap2ContiguousParts && snap1ContiguousParts != snap2ContiguousParts)
                {
                    bool snap1IsParent = snap2ContiguousParts.HasCore() ? false : true;
                    ContiguousParts parentParts = snap1IsParent ? snap1ContiguousParts : snap2ContiguousParts;
                    Part parentPart = snap1IsParent ? snap1Part : snap2Part;
                    GameObject parentSnap = snap1IsParent ? selectedSnapPoint1 : selectedSnapPoint2;
                    ContiguousParts childParts = snap1IsParent ? snap2ContiguousParts : snap1ContiguousParts;
                    Part childPart = snap1IsParent ? snap2Part : snap1Part;
                    Rigidbody childPartsRigidBody = childParts.GetComponent<Rigidbody>();
                    GameObject childSnap = snap1IsParent ? selectedSnapPoint2 : selectedSnapPoint1;

                    childPartsRigidBody.constraints = RigidbodyConstraints.FreezeAll;

                    childParts.transform.rotation = Quaternion.Euler(parentPart.transform.rotation.eulerAngles);
                    childParts.transform.Rotate(placeRotation);
                    childParts.transform.position += (parentSnap.transform.position - childSnap.transform.position);

                    if (parentParts.Intersects(childPart))
                    {
                        infoText.text = "Invalid placement!";
                        infoText.color = Color.red;
                    }
                    else
                    {
                        infoText.text = "Press Enter to place";
                        infoText.color = Color.blue;
                        if (placeAction.ReadValue<float>() > 0)
                        {
                            childPart.connections.Add(new PartConnection(childSnap, parentSnap, placeRotation));
                            parentPart.connections.Add(new PartConnection(parentSnap, childSnap, placeRotation));
                            parentParts.Merge(childParts);
                            Destroy(childPartsRigidBody);
                            currentTool = Tool.Select;
                            selectedSnapPoint1 = null;
                            selectedSnapPoint2 = null;
                        }
                    }

                    if (cancelPlaceAction.ReadValue<float>() > 0)
                    {
                        childPartsRigidBody.constraints = RigidbodyConstraints.None;
                        currentTool = Tool.Select;
                        selectedSnapPoint1 = null;
                        selectedSnapPoint2 = null;
                    }
                } else
                {
                    Debug.LogWarning("Unkown Place Condition");
                    if (cancelPlaceAction.ReadValue<float>() > 0)
                    {
                        currentTool = Tool.Select;
                        selectedSnapPoint1 = null;
                        selectedSnapPoint2 = null;
                    }
                }
            }
        }
        else
        {
            modeText.text = "Mode: Play";
            controlHintsText.text = "TAB - Exit Edit Mode; WASD - Move";
            Vector3 corePos = transport.core.transform.position;
            cam.transform.position = new Vector3(corePos.x, corePos.y + 2, corePos.z - 5);
            cam.transform.LookAt(corePos);

            transport.OnMoveAxis(moveAction.ReadValue<Vector2>());
        }
    }

    public void SwitchMode()
    {
        if (editMode) // Switching to play mode
        {
            editorPlayer.SetActive(false);
            transport.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
        else
        {
            transport.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            Vector3 corePos = transport.core.transform.position;
            editorPlayer.transform.position = new Vector3(corePos.x, corePos.y + 10, corePos.z);
            editorPlayer.transform.rotation = Quaternion.Euler(0, 0, 0);
            editorPlayer.SetActive(true);
        }
        editMode = !editMode;
    }
}
