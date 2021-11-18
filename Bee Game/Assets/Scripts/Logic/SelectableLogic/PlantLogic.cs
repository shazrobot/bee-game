using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlowerType { Fertility, Health, Lethality, None, Rapidity, Expulsion};

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

    [SerializeField]
    private FlowerType flowerType;

    private float flowerHealAmount = 20;

    private List<CreatureLogic> gathererAllocation = new List<CreatureLogic>();

    protected override void Awake()
    {
        base.Awake();
        pollenAmount = pollenMax;
    }


    //Getters
    public bool HasPollen()
    {
        return (pollenAmount > 0);
    }

    public bool IsPollinated()
    {
        return pollinated;
    }

    public FlowerType GetFlowerType()
    {
        return flowerType;
    }

    public float GetFlowerHealAmount()
    {
        return flowerHealAmount;
    }

    public bool IsBeingPollinated()
    {
        return (pollinator != null);
    }

    public bool IsThePollinator(CreatureLogic creature)
    {
        return (pollinator == creature);
    }

    public bool IsBeeInGathererAllocation(CreatureLogic bee)
    {
        return gathererAllocation.Contains(bee);
    }

    public int GetGathererAllocationSize()
    {
        return gathererAllocation.Count;
    }

    public int GetGathererAllocationSizeExcludingBee(CreatureLogic bee)
    {
        int amount = (IsBeeInGathererAllocation(bee)) ? (gathererAllocation.Count - 1) : gathererAllocation.Count;
        return amount;
    }

    //Setter Methods

    public void AddBeeToGatherAllocation(CreatureLogic bee)
    {
        if (!IsBeeInGathererAllocation(bee))
        {
            gathererAllocation.Add(bee);
        }        
    }

    public void RemoveBeeFromGatherAllocation(CreatureLogic bee)
    {
        if (IsBeeInGathererAllocation(bee))
        {
            gathererAllocation.Remove(bee);
        }
    }

    public void ClearAllFromGatherAllocation()
    {
        foreach(CreatureLogic bee in gathererAllocation)
        {
            bee.UnallocatePlant(this);
        }
        gathererAllocation.Clear();
    }

    //Miscellaneous Methods

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
            ClearAllFromGatherAllocation();
        }

        return available;
    }

    

    public void HaltPollination()
    {
        pollinator = null;
    }

    public void InitiatePollination(CreatureLogic creature)
    {
        pollinator = creature;
    }

    public override void ChangeHealth(float healthChange)
    {
        ResetHealthTimer();
        currentHealth += healthChange;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        else if (currentHealth <= 0)
        {
            SelectionManager.instance.SelectableDied(this);
            EcosystemLogic.instance.PlantDied(this);
            currentHealth = 0;
            dead = true;
            gameObject.SetActive(false);
        }
    }
}
