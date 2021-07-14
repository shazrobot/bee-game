using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverSpriteLogic : MonoBehaviour
{
    public static MouseOverSpriteLogic instance;
    [SerializeField]
    private SpriteRenderer mouseOverDisplay;
    private Transform target;
    private void Start()
    {
        instance = this;
        mouseOverDisplay.gameObject.SetActive(false);
    }

    public void SetTarget(Transform newTarget = null, int scale = 5)
    {
        if(newTarget == null)
        {
            mouseOverDisplay.gameObject.SetActive(false);
        }
        else
        {
            mouseOverDisplay.gameObject.SetActive(true);
            mouseOverDisplay.transform.position = newTarget.transform.position;
            mouseOverDisplay.transform.localScale = scale*Vector3.one;
        }
    }
}
