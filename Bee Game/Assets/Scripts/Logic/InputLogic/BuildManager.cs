using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    public GameObject hiveBuildDisplayModel;

    private Ray ray;
    private RaycastHit rayHit;
    private float maxRayDistance = 1000.0f;

    public void Start()
    {
        instance = this;
        hiveBuildDisplayModel.SetActive(false);
    }

    public void HandleMouseLeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out rayHit, maxRayDistance))
            {
                HiveBuildLocation location = rayHit.collider.gameObject.GetComponent<HiveBuildLocation>();
                if (location != null)
                {
                    if (!location.IsOccupied())
                    {
                        PlayerLogic.instance.factionLogic.CreateHive(location);
                        //try to build it, if successfully built, then exit build mode, if not holding shift
                        if (!Input.GetKey(KeyCode.LeftShift) || !Input.GetKey(KeyCode.LeftShift))
                            ExitBuildMode();
                        else
                            EnterBuildMode();

                    }                    
                }
            }
        }
    }

    public void HandleMouseRightClick()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            ExitBuildMode();
        }
    }

    public void HandleMouseOver()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out rayHit, maxRayDistance))
        {
            HiveBuildLocation location = rayHit.collider.gameObject.GetComponent<HiveBuildLocation>();
            if (location != null)
            {
                if (!location.IsOccupied())
                {
                    hiveBuildDisplayModel.SetActive(true);
                    hiveBuildDisplayModel.transform.position = location.transform.position;
                    return;
                }
            }
            hiveBuildDisplayModel.SetActive(false);
        }
        //if over a hivebuildlocation, then show a green hive at that location
    }

    public void EnterBuildMode()
    {
        foreach(HiveBuildLocation location in EcosystemLogic.instance.hiveBuildLocations)
        {
            if (!location.IsOccupied())
            {
                location.gameObject.SetActive(true);
                location.DisplayHiveLocation();
            }
            else
            {
                location.HideHiveLocation();
                location.gameObject.SetActive(false);
            }
                
            
            
        }
        //show build position sparkle
    }

    public void ExitBuildMode()
    {
        foreach (HiveBuildLocation location in EcosystemLogic.instance.hiveBuildLocations)
        {
            location.HideHiveLocation();
            location.gameObject.SetActive(false);
        }
        hiveBuildDisplayModel.SetActive(false);
        InputManager.instance.ExitBuildMode();
    }
}