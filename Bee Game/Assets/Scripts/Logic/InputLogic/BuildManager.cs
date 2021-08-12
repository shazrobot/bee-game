using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    public void Start()
    {
        instance = this;
    }

    public void HandleMouseLeftClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //check to see if its the right location
        }
    }

    public void HandleMouseRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ExitBuildMode();
        }
    }

    public void EnterBuildMode()
    {
        //show build position sparkle
    }

    public void ExitBuildMode()
    {
        InputManager.instance.buildMode = false;
    }
}