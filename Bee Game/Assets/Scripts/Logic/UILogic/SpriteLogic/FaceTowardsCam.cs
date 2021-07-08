using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTowardsCam : MonoBehaviour
{
    private void RotateX()
    {
        float camy = Camera.main.transform.position.y - transform.position.y;
        float camz = Camera.main.transform.position.z - transform.position.z;

        float camd = Vector3.Distance(Camera.main.transform.position, transform.position);

        float theta = (float)(Math.Acos(camz/camd)*180/Math.PI);

        //float theta = (float)(Math.Atan(camy / camz) * 180 / Math.PI);

        Debug.Log(theta);
        transform.rotation = Quaternion.Euler(-theta, 0f, 0f);
    }

    void Update()
    {
        RotateX();
        //transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        //transform.LookAt(Camera.main.transform.position);
        //transform.rotation = Quaternion.Euler(transform.eulerAngles.x, 0f, 0f);

    }
}
