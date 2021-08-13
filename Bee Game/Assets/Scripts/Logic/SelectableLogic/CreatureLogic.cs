﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreatureLogic : SelectableLogic
{
    //Creature Data
    public List<MoveCommand> moveCommands = new List<MoveCommand>();
    public float moveSpeed;

    //Gathering Data
    public float gatherTime = 10f;
    private float gatherTimer = 0f;
    [SerializeField]
    private PlantLogic gatherFrom = null;
    private int pollenGathered = 0;
    private int pollenMax = 100;
    [SerializeField]
    private int attack = 20;

    private float attackCooldownCounter = 0f;
    private float attackCooldown = 2f;

    private float aggroRange = 50f;

    private FactionLogic faction;

    private float distanceThreshold = 5f;

    [SerializeField]
    private SkinnedMeshRenderer meshRenderer;

    protected override void Awake()
    {
        base.Awake();
    }

    //
    public void SetCreationVariables(FactionLogic fact, Material material)
    {
        faction = fact;
        meshRenderer.material = material;
    }

    public FactionLogic GetFaction()
    {
        return faction;
    }

    //Command Logic

    //Clears all previous commands and sets current one to given command
    public void ResetCommandsToThis(MoveCommand goal)
    {
        //clear any gathering commands
        ResetGathering();
        //probably good to tell a plant it is no longer being gathered from by the pollinator

        moveCommands.Clear();
        moveCommands.Add(goal);
    }

    public void EnqueueGoal(MoveCommand goal)
    {
        moveCommands.Add(goal);
        if (base.IsSelected())
        {
            RallyPointManager.instance.CreatureFinishedMoveCommand(this);
        }        
    }

    private void DequeueGoal()
    {
        if (moveCommands.Count > 0)
        {
            moveCommands.RemoveAt(0);
            if (base.IsSelected())
            {
                RallyPointManager.instance.CreatureFinishedMoveCommand(this);
            }
        }
    }

    private void ReplaceCurrentGoal(MoveCommand goal)
    {
        if (moveCommands.Count >= 1)
        {
            moveCommands[0] = goal;
        }
    }

    private void UpdateCommands()
    {
        if (moveCommands.Count > 0)
        {
            if (!MovementLogic.ReachedDestination(transform, moveCommands[0].GetDestination(), distanceThreshold))
            {
                MovementLogic.MoveTowards(transform, moveSpeed, moveCommands[0].GetDestination());
            }
            else
            {
                if (moveCommands[0].moveType == MoveType.Move)
                    DequeueGoal();
                else if (moveCommands[0].moveType == MoveType.Gather)
                {
                    GatherResources(moveCommands[0].objective);
                }
                else if (moveCommands[0].moveType == MoveType.DropOffResources)
                {
                    DeliverResources(moveCommands[0].objective);
                }
                else if (moveCommands[0].moveType == MoveType.Attack)
                {
                    AttackObjective(moveCommands[0].objective);
                }
            }
        }

        if(moveCommands.Count == 0)
        {
            CheckForNearbyEnemies();
        }


        if (attackCooldownCounter > 0)
        {
            attackCooldownCounter -= Time.deltaTime;
            if (attackCooldownCounter < 0)
                attackCooldownCounter = 0f;
        }
        //if idle, you could spread yourself out, or check for enemies
    }

    //Gathering Logic
    
    public bool PollenFull()
    {
        return (pollenGathered >= pollenMax);
    }

    private void GatherResources(GameObject objective)
    {
        
        PlantLogic plant = objective.GetComponent<PlantLogic>();
        
        if (!plant.IsBeingPollinated() && plant.HasPollen()) //if the plant is available, and has pollen, start gathering from it,
        {
            gatherTimer = 0f;
            gatherFrom = plant;
            plant.InitiatePollination(this);
        }
        else if (plant != gatherFrom)
        {
            PlantLogic closetEligble = EcosystemLogic.instance.ClosestGatherablePlant(transform.position);
            if(closetEligble != null)//otherwise, look for a new plant
            {
                ReplaceCurrentGoal(new MoveCommand(MoveType.Gather, Vector3.zero, closetEligble.gameObject));
            }
            else
            {
                FinishGathering();
            }
            //if no new plants, then just hang out here until new command is given, or t he plant becomes available
        }
        

        if (plant.IsThePollinator(this) && gatherFrom == plant)
        {
            if (PollenFull())
            {
                //if the dude already has all the pollen he can carry, then just turn it in
                FinishGathering();
            }
            gatherTimer += Time.deltaTime;
            if (gatherTimer >= gatherTime)
            {
                CollectPollen(plant);
                FinishGathering();
            }
        }
        
    }

    private void CollectPollen(PlantLogic plant)
    {
        pollenGathered += plant.GatherAvailablePollen(pollenMax-pollenGathered);
    }

    private void FinishGathering()
    {
        int commands = moveCommands.Count;
        ResetGathering();
        DequeueGoal();
        if (commands == 1)
        {
            HiveLogic goal = faction.ClosestHive(transform.position);
            if (goal != null)
            {
                EnqueueGoal(new MoveCommand(MoveType.DropOffResources, Vector3.zero, goal.gameObject));
            }
        }
    }

    private void ResetGathering()
    {
        gatherTimer = 0f;
        if(gatherFrom != null)
        {
            gatherFrom.HaltPollination();
        }
        gatherFrom = null;

    }

    //Delivery Logic

    private void DeliverResources(GameObject objective)
    {
        HiveLogic hive = objective.GetComponent<HiveLogic>();

        faction.UpdatePollenAmount(pollenGathered);
        pollenGathered = 0;

        int commands = moveCommands.Count;
        DequeueGoal();
        if (commands == 1)
        {
            PlantLogic goal = EcosystemLogic.instance.ClosestGatherablePlant(transform.position);
            if(goal != null)
            {
                EnqueueGoal(new MoveCommand(MoveType.Gather, Vector3.zero, goal.gameObject));
            }
            else
            {
                EnqueueGoal(new MoveCommand(MoveType.Move, hive.rallyPoint.position));
            }
            //else move command to rally point
        }
    }

    private void CheckForNearbyEnemies()
    {
        SelectableLogic closestUnit = null;
        float unitDist = Mathf.Infinity;
        foreach(SelectableLogic unit in faction.GetAliveEnemyUnits())
        {
            float dist = Vector3.Distance(transform.position, unit.transform.position);
            if ((dist < aggroRange) && (dist < unitDist))
            {
                unitDist = dist;
                closestUnit = unit;
            }
        }

        if(closestUnit != null)
        {
            if(closestUnit.GetComponent<HiveLogic>() != null)
                Debug.Log("attacking hive");
            if (closestUnit.GetComponent<CreatureLogic>() != null)
                Debug.Log("attacking bee");
            EnqueueGoal(new MoveCommand(MoveType.Attack, Vector3.zero, closestUnit.gameObject));
        }
    }

    //Attack Logic

    private void AttackObjective(GameObject objective)
    {
        SelectableLogic selectable = objective.GetComponent<SelectableLogic>();

        if (attackCooldownCounter <= 0)
        {
            selectable.ChangeHealth(-attack);
            attackCooldownCounter = attackCooldown;
        }

        if (selectable.IsDead())
        {
            DequeueGoal();
        }
    }

    public void FixedUpdate()
    {
        UpdateCommands();
    }


    public override void ChangeHealth(int healthChange)
    {
        currentHealth += healthChange;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        else if (currentHealth <= 0)
        {
            ResetGathering();
            currentHealth = 0;
            dead = true;
            gameObject.SetActive(false);
        }
    } 
}
