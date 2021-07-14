using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableLogic : MonoBehaviour
{
    private bool selected;
    [SerializeField]
    private int uIScale = 5;
    [SerializeField]
    private Transform manualUITransform;

    protected virtual void Awake()
    {
        selected = false;
        if (manualUITransform != null)
        {
            manualUITransform.gameObject.SetActive(false);
        }
    }

    public bool IsSelected()
    {
        return selected;
    }

    public void Select()
    {
        selected = true;
    }

    public void Deselect()
    {
        selected = false;
    }

    public int GetUIScale()
    {
        if(uIScale <= 0)
        {
            return 100;
        }
        return uIScale;
    }

    public Transform GetUIPosition()
    {
        if (manualUITransform != null)
            return manualUITransform;
        else
            return transform;
    }
}
