using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantLogic : SelectableLogic
{
    public int pollenAmount = 100;
    public int pollenMax = 100;

    protected override void Awake()
    {
        base.Awake();
    }

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
