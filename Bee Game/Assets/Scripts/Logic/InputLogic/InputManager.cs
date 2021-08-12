using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private Vector3 rotateStartPosition;
    private Vector3 rotateCurrentPosition;

    private Ray ray;
    private RaycastHit rayHit;
    private float maxRayDistance = 1000.0f;

    public bool buildMode = false;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    

    private void HandleMovementMouseInput()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            CameraControlLogic.instance.Zoom(Time.unscaledDeltaTime, Input.mouseScrollDelta.y);
        }
        if (Input.GetMouseButtonDown(2))
        {
            rotateStartPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            rotateCurrentPosition = Input.mousePosition;

            Vector3 difference = rotateStartPosition - rotateCurrentPosition;

            rotateStartPosition = rotateCurrentPosition;

            CameraControlLogic.instance.DragRotation(difference);
        }
    }

    private void HandleMovementKeyboardInput()
    {
        int forward = 0;
        int right = 0;
        int rotate = 0;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            forward += 1;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            forward -= 1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            right += 1;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            right -= 1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            rotate -= 1;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotate += 1;
        }
        CameraControlLogic.instance.Rotate(Time.unscaledDeltaTime, rotate);
        CameraControlLogic.instance.Move(Time.unscaledDeltaTime, forward, right);
    }

    private void HandleSelectableMouseOver()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out rayHit, maxRayDistance))
        {
            SelectableLogic selected = rayHit.collider.gameObject.GetComponent<SelectableLogic>();
            if (selected != null)
            {
                MouseOverSpriteLogic.instance.SetTarget(selected.GetUIPosition(), selected.GetUIScale());
                return;
            }
        }
        MouseOverSpriteLogic.instance.SetTarget();
    }


    // Update is called once per frame
    void Update()
    {
        HandleMovementKeyboardInput();
        HandleMovementMouseInput();
        HandleSelectableMouseOver();//this might be worth putting into selection manager
        if (!buildMode)
        {
            SelectionManager.instance.HandleIssueCommandsRightClick();
            SelectionManager.instance.HandleMouseSelectionInput();
        }
        else
        {
            BuildManager.instance.HandleMouseLeftClick();
            BuildManager.instance.HandleMouseRightClick();
        }
        
    }
}
