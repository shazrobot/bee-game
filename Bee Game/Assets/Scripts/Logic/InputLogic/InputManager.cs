using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    

    private Vector3 rotateStartPosition;
    private Vector3 rotateCurrentPosition;


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


    // Update is called once per frame
    void Update()
    {
        HandleMovementKeyboardInput();
        HandleMovementMouseInput();
        SelectionManager.instance.HandleMouseSelectionInput();
    }


}
