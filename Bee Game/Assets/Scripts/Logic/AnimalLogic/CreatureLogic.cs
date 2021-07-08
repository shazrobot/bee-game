using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreatureLogic : MonoBehaviour
{
    //Selection Logics
    public SkinnedMeshRenderer modelMesh;
    public bool selected;
    public Material selectedMaterial;
    public Material ordinaryMaterial;

    //Creature Data
    public List<MoveCommand> moveCommands = new List<MoveCommand>();
    public float moveSpeed;

    //Gathering Data
    public float gatherTime = 10f;
    private float gatherTimer = 0f;
    private bool gathering = false;
    private int pollenGathered = 0;
    private int pollenMax = 100;
    

    public void Awake()
    {
        selected = false;
    }

    //Selection Logic
    public void UpdateMaterial()
    {
        if (selected)
        {
            modelMesh.material = selectedMaterial;
        }
        else
        {
            modelMesh.material = ordinaryMaterial;
        }
    }

    public void Select()
    {
        selected = true;
        UpdateMaterial();
    }

    public void Deselect()
    {
        selected = false;
        UpdateMaterial();
    }

    //Command Logic

    //Clears all previous commands and sets current one to given command
    public void ResetCommandsToThis(MoveCommand goal)
    {
        moveCommands.Clear();
        moveCommands.Add(goal);
    }

    public void EnqueueGoal(MoveCommand goal)
    {
        moveCommands.Add(goal);
    }

    private void DequeueGoal()
    {
        if (moveCommands.Count > 0)
        {
            moveCommands.RemoveAt(0);
        }
    }

    private void UpdateCommands()
    {
        if (moveCommands.Count > 0)
        {
            if (!MovementLogic.ReachedDestination(transform, moveCommands[0].GetDestination()))
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
                    DeliverResources();
                }
            }
            
        }
    }

    //Gathering Logic
    
    public bool PollenFull()
    {
        return (pollenGathered >= pollenMax);
    }

    private void GatherResources(GameObject objective)
    {
        PlantLogic plant = objective.GetComponent<PlantLogic>();

        //if you haven't started gathering stuff start it now
        if (!gathering)
        {
            gatherTimer = 0f;
            gathering = true;
        }
        if (plant.HasPollen())
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
        else
        {
            //if theres nothing left in plant, abort gathering further
            FinishGathering();
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
            HiveLogic goal = PlayerLogic.instance.ClosestHive(transform.position);
            if (goal != null)
            {
                EnqueueGoal(new MoveCommand(MoveType.DropOffResources, Vector3.zero, goal.gameObject));
            }
        }
    }

    private void ResetGathering()
    {
        gatherTimer = 0f;
        gathering = false;
    }

    //Delivery Logic

    private void DeliverResources()
    {
        //HiveLogic hive = objective.GetComponent<HiveLogic>();

        PlayerLogic.instance.UpdatePollenAmount(pollenGathered);
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
        }
    }

    public void Update()
    {
        UpdateCommands();
    }

}
