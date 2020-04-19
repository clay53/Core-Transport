using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    public InputAction moveAction;
    public float editorMoveSpeed = 2f;
    public float editorAccelerationSpeed = 1f;
    public float editorStandingAccelerationSpeed = 0.5f;
    public Vector2 moveAxis = new Vector2(0, 0);

    public InputAction jumpAction;
    public float jumpPower = 5;

    public enum Tool
    {
        Select,
        Place
    }
    public Tool currentTool = Tool.Select;

    public InputAction lookAction;
    public float lookSpeed = 0.2f;
    public GameObject lookingAtSnapPoint;

    public InputAction selectAction;
    public GameObject selectedSnapPoint1;
    public GameObject selectedSnapPoint2;

    public GameObject editorPlayer;
    Rigidbody editorPlayerBody;

    public Transport transport;
    public Camera cam;
    public bool editMode = false;

    void Start()
    {
        editorPlayerBody = editorPlayer.GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        moveAction.Enable();
        jumpAction.Enable();
        lookAction.Enable();
        selectAction.Enable();
    }

    void Update()
    {
        if (editMode)
        {
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

            // SnapPoints
            if (currentTool == Tool.Select)
            {
                int layerMask = 1 << 9;
                RaycastHit hitInfo;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, Mathf.Infinity, layerMask))
                {
                    lookingAtSnapPoint = hitInfo.collider.gameObject;
                    if (selectAction.ReadValue<float>() > 0 && selectedSnapPoint1 == null && lookingAtSnapPoint.GetComponent<MeshRenderer>().enabled)
                    {
                        selectedSnapPoint1 = lookingAtSnapPoint;
                    }
                    else if (selectAction.ReadValue<float>() > 0 && selectedSnapPoint1 != lookingAtSnapPoint && selectedSnapPoint2 == null && lookingAtSnapPoint.GetComponent<MeshRenderer>().enabled)
                    {
                        selectedSnapPoint2 = lookingAtSnapPoint;
                        currentTool = Tool.Place;
                    }
                }
                else
                {
                    lookingAtSnapPoint = null;
                }
            }
        }
        else
        {
            Vector3 corePos = transport.core.transform.position;
            cam.transform.position = new Vector3(corePos.x, corePos.y + 2, corePos.z - 5);
            cam.transform.LookAt(corePos);
        }
    }

    public void SwitchMode()
    {
        if (editMode)
        {
            editorPlayer.SetActive(false);
            transport.core.GetComponent<Rigidbody>().isKinematic = false;
        }
        else
        {
            transport.core.GetComponent<Rigidbody>().isKinematic = true;
            Vector3 corePos = transport.core.transform.position;
            editorPlayer.transform.position = new Vector3(corePos.x, corePos.y + 10, corePos.z);
            editorPlayer.transform.rotation = Quaternion.Euler(0, 0, 0);
            editorPlayer.SetActive(true);
        }
        editMode = !editMode;
    }

    public void SwitchMode(InputAction.CallbackContext context)
    {
        SwitchMode();
    }
}
