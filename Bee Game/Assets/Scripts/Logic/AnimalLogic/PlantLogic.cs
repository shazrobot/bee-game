using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantLogic : MonoBehaviour
{
    public int pollenAmount = 100;
    public int pollenMax = 100;

    public bool HasPollen()
    {
        return (pollenAmount > 0);
    }

    public int GatherAvailablePollen(int gatherAmount)
    {
        int available = (gatherAmount > pollenAmount)?pollenAmount: gatherAmount;
        pollenAmount -= available;
        return available;
    }
}
