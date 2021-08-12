using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantLogic : SelectableLogic
{
    [NonSerialized]
    public int pollenAmount = 100;
    public int pollenMax = 100;

    //a plant cannot spawn closer than this
    public int plantSpawnBoundary = 10;

    [SerializeField]
    private bool pollinated = false;
    [SerializeField]
    private PlantAnimations animations;

    private CreatureLogic pollinator = null;

    protected override void Awake()
    {
        base.Awake();
        pollenAmount = pollenMax;
    }

    public bool HasPollen()
    {
        return (pollenAmount > 0);
    }

    public bool IsPollinated()
    {
        return pollinated;
    }

    public void CompletedReproduction()
    {
        pollinated = false;
        pollenAmount = pollenMax;
        animations.ShowFlowers();
    }

    public int GatherAvailablePollen(int gatherAmount)
    {
        pollinator = null;
        int available = (gatherAmount > pollenAmount)?pollenAmount: gatherAmount;
        pollenAmount -= available;
        if (pollenAmount == 0)
        {
            pollinated = true;
            animations.HideFlowers();
        }
        return available;
    }

    public bool IsBeingPollinated()
    {
        return (pollinator != null);
    }

    public bool IsThePollinator(CreatureLogic creature)
    {
        return (pollinator == creature);
    }

    public void HaltPollination()
    {
        pollinator = null;
    }

    public void InitiatePollination(CreatureLogic creature)
    {
        pollinator = creature;
    }
}
