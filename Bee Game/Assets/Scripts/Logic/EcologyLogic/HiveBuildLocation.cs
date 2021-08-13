using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HiveBuildLocation : MonoBehaviour
{
    [SerializeField]
    private GameObject emphasisSprite;
    private bool occupied = false;

    // Start is called before the first frame update
    void Start()
    {
        emphasisSprite.SetActive(false);
        EcosystemLogic.instance.AddHiveBuildLocation(this);
    }

    public bool IsOccupied()
    {
        return occupied;
    }

    public void Unoccupy()
    {
        occupied = false;
    }

    public void Occupy()
    {
        occupied = true;
    }

    public void DisplayHiveLocation()
    {
        if (!occupied)
        {
            emphasisSprite.SetActive(true);
        }
    }

    public void HideHiveLocation()
    {
        emphasisSprite.SetActive(false);
    }
}
