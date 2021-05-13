using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RallyPoint : MonoBehaviour
{
    public LineRenderer baseCircle;
    public LineRenderer shaft;
    public Image top;

    public Vector3 point;
    public float baseWidth;

    private float circleYHeight = 0.1f;

    private Vector3[] GenerateBaseCirclePositions()
    {
        float circMul = (Mathf.Sqrt(3) / 2f);
        Vector3[] tempList = new Vector3[13];
        tempList[0] = (new Vector3(point.x+baseWidth, circleYHeight, point.z));
        tempList[1] = (new Vector3(point.x + (baseWidth* circMul), circleYHeight, point.z + (baseWidth * 0.5f)));
        tempList[2] = (new Vector3(point.x + (baseWidth * 0.5f), circleYHeight, point.z + (baseWidth * circMul)));
        tempList[3] = (new Vector3(point.x , circleYHeight, point.z + baseWidth));
        tempList[4] = (new Vector3(point.x + (baseWidth * -0.5f), circleYHeight, point.z + (baseWidth * circMul)));
        tempList[5] = (new Vector3(point.x + (baseWidth * -circMul), circleYHeight, point.z + (baseWidth * 0.5f)));
        tempList[6] = (new Vector3(point.x - baseWidth, circleYHeight, point.z));
        tempList[7] = (new Vector3(point.x + (baseWidth * -circMul), circleYHeight, point.z + (baseWidth * -0.5f)));
        tempList[8] = (new Vector3(point.x + (baseWidth * -0.5f), circleYHeight, point.z + (baseWidth * -circMul)));
        tempList[9] = (new Vector3(point.x, circleYHeight, point.z - baseWidth));
        tempList[10] = (new Vector3(point.x + (baseWidth * 0.5f), circleYHeight, point.z + (baseWidth * -circMul)));
        tempList[11] = (new Vector3(point.x + (baseWidth * circMul), circleYHeight, point.z + (baseWidth * -0.5f)));
        tempList[12] = (new Vector3(point.x + baseWidth, circleYHeight, point.z));
        return tempList;
    }

    private Vector3[] GenerateShaftPositions()
    {
        Vector3[] tempList = new Vector3[2];
        tempList[0] = (new Vector3(point.x, circleYHeight, point.z));
        tempList[1] = point;
        return tempList;
    }

    private void Draw()
    {
        baseCircle.gameObject.SetActive(true);
        baseCircle.SetPositions(GenerateBaseCirclePositions());
        shaft.gameObject.SetActive(true);
        shaft.SetPositions(GenerateShaftPositions());
    }

    public void SetRallyPoint(Vector3 pt)
    {
        point = pt;
        Draw();
    }


    public void DestroyChildren()
    {
        Destroy(baseCircle.gameObject);
        Destroy(shaft.gameObject);
    }
}
