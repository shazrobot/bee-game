using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RallyPoint : MonoBehaviour
{
    public GameObject middle;
    public GameObject outer;


    public Vector3 point;
    public float baseWidth;

    private float circleYHeight = 0.1f;

    private void PositionMiddle()
    {
        middle.transform.position = point;
        middle.transform.LookAt(Camera.main.transform.position, -Vector3.up);
    }

    private void PositionOuter()
    {
        outer.transform.position = point;
        outer.transform.LookAt(Camera.main.transform.position, -Vector3.up);
    }

    private void Draw()
    {
        middle.gameObject.SetActive(true);
        outer.gameObject.SetActive(true);
        PositionOuter();
        PositionMiddle();
    }

    public void SetRallyPoint(Vector3 pt)
    {
        point = pt;
        Draw();
    }


    public void DestroyChildren()
    {
        Destroy(outer);
        Destroy(middle);
    }

    public void AnimateOuterBounce()
    {
        StartCoroutine(UIAnimations.BounceScaleAnimation(outer.transform.localScale.x * 3, outer.transform.localScale.x, 0.5f, outer));
        //have an outer ring which you trigger a ui animation coroutine for
    }
}
