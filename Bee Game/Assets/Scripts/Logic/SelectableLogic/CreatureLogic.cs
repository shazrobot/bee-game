using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using UnityEditor;
using UnityEngine;

public class CreatureLogic : SelectableLogic
{
    //Creature Data
    public List<MoveCommand> moveCommands = new List<MoveCommand>();
    public float moveSpeed;

    public float autoGatherRange = 100f;

    //Gathering Data
    public float gatherTime = 10f;
    private float gatherTimer = 0f;
    [SerializeField]
    private PlantLogic gatherFrom = null;
    private PlantLogic allocatedPlant = null;
    private int pollenGathered = 0;
    [SerializeField]
    private int pollenMax = 10;
    [SerializeField]
    private int attack = 20;

    private float attackCooldownCounter = 0f;
    private float attackCooldown = 2f;

    private bool attackPoweredUp = false;
    private float lethalityDamageBuff = 25f;

    private float aggroRange = 50f;

    private FactionLogic faction;

    private float distanceThreshold = 1f;

    private float attackRange = 1f;

    private float tempFloat;

    [SerializeField]
    private SkinnedMeshRenderer meshRenderer;

    [SerializeField]
    private BeeAnimations animations;

    private FlowerType previouslyPollinated = FlowerType.None;

    protected override void Awake()
    {
        base.Awake();
        SetLethality(false);
    }


    public void SetCreationVariables(FactionLogic fact, Material material)
    {
        faction = fact;
        meshRenderer.material = material;
    }

    public FactionLogic GetFaction()
    {
        return faction;
    }

    public int GetPollenAmount()
    {
        return pollenGathered;
    }


    //Command Logic

    public bool HasNoCommands()
    {
        return (moveCommands.Count == 0);
    }

    //Clears all previous commands and sets current one to given command
    public void ResetCommandsToThis(MoveCommand goal)
    {
        //clear any gathering commands
        ResetGathering();
        //probably good to tell a plant it is no longer being gathered from by the pollinator
        DeallocationCheck();
        moveCommands.Clear();
        moveCommands.Add(goal);
        AllocationCheck(moveCommands[0]);
        if (base.IsSelected())
        {
            RallyPointManager.instance.CreatureFinishedMoveCommand(this);
        }
    }

    public void EnqueueGoal(MoveCommand goal)
    {
        moveCommands.Add(goal);
        if (base.IsSelected())
        {
            RallyPointManager.instance.CreatureFinishedMoveCommand(this);
        }
        if (moveCommands.Count == 1)
        {
            AllocationCheck(moveCommands[0]);
        }
    }

    //this adds a goal to the front of the queue
    public void FrontLoadGoal(MoveCommand goal)
    {
        DeallocationCheck();
        moveCommands.Insert(0, goal);
        AllocationCheck(goal);
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
        if (!HasNoCommands())
        {
            AllocationCheck(moveCommands[0]);
        }
    }

    private void ReplaceCurrentGoal(MoveCommand goal)
    {
        if (!HasNoCommands())
        {
            moveCommands[0] = goal;
            AllocationCheck(goal);
        }
    }

    private void AllocationCheck(MoveCommand goal)
    {
        if(!HasNoCommands())
        {
            if(moveCommands[0].moveType == MoveType.Gather)
            {
                PlantLogic plant = goal.GetObjective().GetComponent<PlantLogic>();
                if(plant != allocatedPlant)
                    RemoveFromPlantGatherList(allocatedPlant);
                AllocateToPlantGatherList(plant);
            }
        }
    }

    private void DeallocationCheck()
    {
        if (!HasNoCommands())
        {
            if (moveCommands[0].moveType == MoveType.Gather)
            {
                PlantLogic plant = moveCommands[0].GetObjective().GetComponent<PlantLogic>();
                RemoveFromPlantGatherList(allocatedPlant);
            }
        }
    }

    private void AllocateToPlantGatherList(PlantLogic plant)
    {
        allocatedPlant = plant;
        plant.AddBeeToGatherAllocation(this);
    }

    public void RemoveFromPlantGatherList(PlantLogic plant)
    {
        allocatedPlant = null;
        if(plant != null)
            plant.RemoveBeeFromGatherAllocation(this);

        animations.StopGatherAnimation();
    }

    public void UnallocatePlant(PlantLogic plant)
    {
        allocatedPlant = null;
    }

    private void UpdateCommands()
    {
        if (moveCommands.Count > 0)
        {
            tempFloat = distanceThreshold;
            if (moveCommands[0].moveType == MoveType.Attack)
                tempFloat += attackRange;
            if (!MovementLogic.ReachedDestination(transform, moveCommands[0], tempFloat, GetRadius()))
            {
                MovementLogic.MoveTowards(transform, moveSpeed, moveCommands[0].GetDestination(), GetRadius());

                if (moveCommands[0].moveType == MoveType.AttackMove)
                {
                    SelectableLogic closestUnit = CheckForNearbyEnemies();
                    if (closestUnit != null)
                    {
                        FrontLoadGoal(new MoveCommand(MoveType.Attack, Vector3.zero, closestUnit.gameObject));
                    }
                }
            }
            else
            {
                if (moveCommands[0].moveType == MoveType.Move || moveCommands[0].moveType == MoveType.AttackMove)
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
            SelectableLogic closestUnit = CheckForNearbyEnemies();
            if (closestUnit != null)
            {
                EnqueueGoal(new MoveCommand(MoveType.Attack, Vector3.zero, closestUnit.gameObject));
            }
        }

        if (moveCommands.Count == 0)
        {
            MovementLogic.IdleAvoidanceOfBees(this, moveSpeed, GetRadius());
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
            animations.PlayGatherAnimation();
        }
        else if (plant != gatherFrom)
        {
            PlantLogic closetEligble = EcosystemLogic.instance.ClosestGatherablePlantOfTypeWithShortestQueue(transform.position, plant.GetFlowerType(), this, autoGatherRange);
            if(closetEligble != null)//otherwise, look for a new plant
            {
                if (Vector3.Distance(closetEligble.GetUIPosition().position, GetUIPosition().position) < autoGatherRange)
                {
                    ReplaceCurrentGoal(new MoveCommand(MoveType.Gather, Vector3.zero, closetEligble.gameObject));
                }
            }
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

                if (plant.GetFlowerType() == FlowerType.Health)
                {
                    animations.PlayHealthAnimation();
                }

                FinishGathering();
            }
        }
        
    }

    private void SetLethality(bool status)
    {
        attackPoweredUp = status;
        animations.SetRedStatus(status);
    }

    private void CollectPollen(PlantLogic plant)
    {
        animations.StopGatherAnimation();
        pollenGathered += plant.GatherAvailablePollen(pollenMax-pollenGathered);
        previouslyPollinated = plant.GetFlowerType();
        if (plant.GetFlowerType() == FlowerType.Health)
        {
            ChangeHealth(plant.GetFlowerHealAmount());
        }
        if (plant.GetFlowerType() == FlowerType.Lethality)
        {
            SetLethality(true);
        }
        if(pollenGathered > 0)
        {
            animations.ShowPollen();
        }
    }

    private void FinishGathering()
    {
        animations.StopGatherAnimation();
        int commands = moveCommands.Count;
        ResetGathering();
        DequeueGoal();
        if (commands == 1)
        {
            HiveLogic goal = faction.ClosestBuiltHive(transform.position);
            if (goal != null)
            {
                EnqueueGoal(new MoveCommand(MoveType.DropOffResources, Vector3.zero, goal.gameObject));
            }
        }
    }

    private void ResetGathering()
    {
        animations.StopGatherAnimation();
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

        if (!hive.IsBuilding())
        {
            faction.UpdatePollenAmount(pollenGathered);
            pollenGathered = 0;
            animations.HidePollen();

            int commands = moveCommands.Count;
            DequeueGoal();
            if (commands == 1)
            {
                if (allocatedPlant != null && !allocatedPlant.IsDead())
                {
                    EnqueueGoal(new MoveCommand(MoveType.Gather, Vector3.zero, allocatedPlant.gameObject));
                }
                else
                {
                    PlantLogic closetEligble = EcosystemLogic.instance.ClosestGatherablePlantOfTypeWithShortestQueue(transform.position, previouslyPollinated, this, autoGatherRange);
                    if (closetEligble != null)
                    {
                        if (Vector3.Distance(closetEligble.GetUIPosition().position, GetUIPosition().position) < autoGatherRange)
                        {
                            EnqueueGoal(new MoveCommand(MoveType.Gather, Vector3.zero, closetEligble.gameObject));
                        }
                    }
                }

            }
        }
        else
        {
            DequeueGoal();
        }        
    }

    

    private SelectableLogic CheckForNearbyEnemies()
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
        return closestUnit;
    }

    //Attack Logic

    private void InitiateAttackOnObjective(GameObject objective)
    {
        SelectableLogic selectable = objective.GetComponent<SelectableLogic>();

        if (attackCooldownCounter <= 0)
        {
            animations.PlayAttackAnimation();
            attackCooldownCounter = attackCooldown;
        }

        if (selectable.IsDead())
        {
            DequeueGoal();
        }
    }

    private void AttackLandedOnObjective(GameObject objective)
    {
        SelectableLogic selectable = objective.GetComponent<SelectableLogic>();

        float damage = (attackPoweredUp) ? -attack - lethalityDamageBuff : -attack;
        if (selectable.GetComponent<CreatureLogic>() != null)
        {
            selectable.GetComponent<CreatureLogic>().AttackedByBee(this);
        }
        selectable.ChangeHealth(damage);
        
        if (attackPoweredUp)
            SetLethality(false);

        if (selectable.IsDead())
        {
            DequeueGoal();
        }
    }

    private void AttackObjective(GameObject objective)
    {
        SelectableLogic selectable = objective.GetComponent<SelectableLogic>();

        if (attackCooldownCounter <= 0)
        {
            animations.PlayAttackAnimation();
            float damage = (attackPoweredUp) ? -attack - lethalityDamageBuff : -attack;
            if(selectable.GetComponent<CreatureLogic>() != null)
            {
                selectable.GetComponent<CreatureLogic>().AttackedByBee(this);
            }
            selectable.ChangeHealth(damage);
            attackCooldownCounter = attackCooldown;
            if (attackPoweredUp)
                SetLethality(false);
        }

        if (selectable.IsDead())
        {
            DequeueGoal();
        }
    }

    public void FixedUpdate()
    {
        if (!dead)
        {
            IncrementHealthTimer();
        }
        UpdateCommands();
    }


    public void AttackedByBee(CreatureLogic aggressor)
    {
        if(faction.GetFriendlinessOfSelectable(aggressor) == FriendlinessType.Hostile)
        {
            if(moveCommands.Count > 0)
            {
                if (moveCommands[0].moveType == MoveType.Gather || moveCommands[0].moveType == MoveType.DropOffResources)
                {
                    FrontLoadGoal(new MoveCommand(MoveType.Attack, Vector3.zero, aggressor.gameObject));
                }
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
            SelectionManager.instance.SelectableDied(this);
            faction.BeeDied(this);
            ResetGathering();
            currentHealth = 0;
            dead = true;
            gameObject.SetActive(false);
        }
    } 
}
