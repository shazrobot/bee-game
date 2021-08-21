using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveLogic : SelectableLogic
{  
    [SerializeField]
    public Transform rallyPoint;

    [NonSerialized]
    public int beeQueue = 0;
    private int beeQueueMax = 6;
    [NonSerialized]
    public float constructionTimer = 0f;
    [NonSerialized]
    public float beeConstructionTime = 30f;

    private FactionLogic faction;

    [SerializeField]
    private HiveAnimation hiveAnimation;

    private HiveBuildLocation location;

    private bool building = false;

    private float hiveConstructionTime = 60f;
    private float hiveConstructionCounter = 0;

    //Unity methods

    protected override void Awake()
    {
        base.Awake();
    }

    public void FixedUpdate()
    {
        if (!dead)
        {
            IncrementHealthTimer();
        }
        IncrementTime();
    }

    //Getters

    public bool IsBuilding()
    {
        return building;
    }

    public bool IsFullQueue()
    {
        return (beeQueue >= beeQueueMax);
    }

    public FactionLogic GetFaction()
    {
        return faction;
    }

    public HiveBuildLocation GetHiveLocation()
    {
        return location;
    }

    
    //Creation methods

    public void InitialiseConstruction()
    {
        currentHealth = 1;
        hiveAnimation.HideCombs();
        building = true;
        hiveConstructionCounter = 0;
    }

    private void FinaliseConstruction()
    {
        building = false;
        hiveAnimation.ShowCombs();
        faction.UpdateBeeCap();
    }

    public void SetCreationVariables(FactionLogic fact, Material material, HiveBuildLocation loc)
    {
        faction = fact;
        hiveAnimation.ColourCombs(material);
        location = loc;
    }


    //Variety methods

    public void BuildBee()//Build bee
    {
        if (faction.CreateBee(this))
        {
            beeQueue -= 1;
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        constructionTimer = 0f;
    }
    public bool QueueBeeBuild()
    {
        if(beeQueue == beeQueueMax || !faction.CanAffordBee() || IsBuilding())
        {
            return false;
        }
        else
        {
            faction.BeeQueued();
            beeQueue += 1;
            return true;
        }
    }

    private void IncrementTime()
    {
        if (beeQueue > 0)
        {
            if (constructionTimer >= beeConstructionTime)
            {
                BuildBee();
            }
            else
            {
                constructionTimer += Time.deltaTime;
            }
        }

        if (building)
        {
            hiveConstructionCounter += Time.deltaTime;
            float amountGrown = hiveConstructionCounter / hiveConstructionTime;

            ChangeHealth((Time.deltaTime / hiveConstructionTime) * maxHealth);

            hiveAnimation.GrowHive(amountGrown);
            if (hiveConstructionCounter >= hiveConstructionTime)
            {
                FinaliseConstruction();
            }
        }
    }

    

    public override void ChangeHealth(float healthChange)
    {
        ResetHealthTimer();
        currentHealth += healthChange;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        else if (currentHealth <= 0)
        {
            if (location != null)
                location.Unoccupy();
            //make the hive location available again
            faction.HiveDied(this);
            currentHealth = 0;
            dead = true;
            gameObject.SetActive(false);
        }
    }
}
