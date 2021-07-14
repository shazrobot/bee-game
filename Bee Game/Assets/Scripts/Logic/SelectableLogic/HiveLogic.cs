using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveLogic : SelectableLogic
{
    public MeshRenderer modelMesh;


    public CreatureLogic beeTemplate;

    [SerializeField]
    private Transform rallyPoint;

    [NonSerialized]
    public int beeQueue = 0;
    private int beeQueueMax = 6;
    [NonSerialized]
    public float constructionTimer = 0f;
    [NonSerialized]
    public float beeConstructionTime = 30f;


    protected override void Awake()
    {
        beeTemplate.gameObject.SetActive(false);
        base.Awake();
    }

    public void BuildBee()//Build bee
    {
        CreatureLogic bee = Instantiate(beeTemplate) as CreatureLogic;

        bee.gameObject.SetActive(true);
        bee.transform.SetParent(beeTemplate.transform.parent);
        bee.transform.position = transform.position;
        bee.EnqueueGoal(new MoveCommand(MoveType.Move, rallyPoint.position));
        beeQueue -= 1;
        ResetTimer();
    }

    private void ResetTimer()
    {
        constructionTimer = 0f;
    }

    public bool QueueBeeBuild()
    {
        if(beeQueue == 6)
        {
            return false;
        }
        else
        {
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
