using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlLogic : MonoBehaviour
{
    public static CameraControlLogic instance;

    public Transform cameraTransform;

    public Transform LockedOnObject;

    public float movementSpeed;
    public float movementTime;

    public float rotationAmount;
    public float dragRotationDamping;

    public Vector3 zoomAmount;

    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newZoom;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    public void Move(float deltaTime, int forward, int right)
    {
        Vector3 direction = Vector3.Normalize(transform.forward * forward + transform.right * right);
        newPosition += direction * movementSpeed * deltaTime;
    }
    public void Rotate(float deltaTime, int direction)
    {
        newRotation *= Quaternion.Euler(Vector3.up * rotationAmount * deltaTime * direction);
    }
    public void Zoom(float deltaTime, float deltaMagnifier)
    {
        newZoom += zoomAmount * deltaTime * deltaMagnifier;
    }

    public void DragRotation(Vector3 deltaPosition)
    {
        newRotation *= Quaternion.Euler(Vector3.up * (deltaPosition.x/ dragRotationDamping));
    }

    public void UpdateCameraPosition()
    {
        if (LockedOnObject != null)
        {
            transform.position = Vector3.Lerp(transform.position, LockedOnObject.transform.position, movementTime * Time.unscaledDeltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, movementTime * Time.unscaledDeltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.unscaledDeltaTime * movementTime);
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.unscaledDeltaTime * movementTime);
        }
    }

    void Update()
    {
        UpdateCameraPosition();
    }
}
