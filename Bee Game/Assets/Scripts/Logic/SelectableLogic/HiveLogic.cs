using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveLogic : SelectableLogic
{
    public MeshRenderer modelMesh;


    

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
    private MeshRenderer meshRenderer;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SetCreationVariables(FactionLogic fact, Material material)
    {
        faction = fact;
        meshRenderer.material = material;
    }

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
        if(beeQueue == 6 || !faction.CanAffordBee())
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
            constructionTimer += Time.deltaTime;
            if (constructionTimer >= beeConstructionTime)
            {
                BuildBee();
            }
        }
    }

    public void Update()
    {
        IncrementTime();
    }
}
