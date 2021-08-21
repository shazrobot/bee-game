using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class FactionAI : MonoBehaviour
{
    public FactionLogic faction;

    public float hiveTerritoryRange = 100;

    [SerializeField]
    private int beeTarget = 0;

    private bool attackMode = true;

    private int attackWaveCounter = 1;
    private int attackWaveBees = 3;

    private void BuildDecisions()
    {
        
        //if you can't afford anything don't bother
        if (!faction.CanAffordBee())
        {
            return;
        }

        //checks how many plants in range
        int needCounter = 0;
        int supplyCounter = 0;
        foreach (HiveLogic hive in faction.hives)
        {
            needCounter += EcosystemLogic.instance.PlantsInRange(hive.GetUIPosition().position, hiveTerritoryRange);
            supplyCounter += hive.beeQueue;
        }

        needCounter /= 2; //current plant to bee ratio
        needCounter = (needCounter == 0) ? 1 : needCounter;

        needCounter += (attackMode) ? (attackWaveCounter* attackWaveBees) : 0;

        supplyCounter += faction.bees.Count;
        beeTarget = needCounter;
        int beesRequired = ((needCounter - supplyCounter)>0) ? (needCounter - supplyCounter) : 0;

        QueueBeeBuilding(beesRequired); //tries to fill hives with queued bees, so long as it can.

        if (!attackMode && faction.CanAffordHive())
        {
            HiveBuildLocation location = EcosystemLogic.instance.ClosestHiveBuildLocation(faction.AveragedHiveLocations());

            if (location != null)
            {
                faction.CreateHive(location);
                attackMode = true;
                UpdateBeeTarget();
            }
            else
            {
                attackMode = true;
                UpdateBeeTarget();
            }
        }
    }

    private void UpdateBeeTarget()
    {
        int needCounter = 0;
        foreach (HiveLogic hive in faction.hives)
        {
            needCounter += EcosystemLogic.instance.PlantsInRange(hive.GetUIPosition().position, hiveTerritoryRange);
        }

        needCounter /= 2; //current plant to bee ratio
        needCounter = (needCounter == 0) ? 1 : needCounter;

        needCounter += (attackMode) ? (attackWaveCounter * attackWaveBees) : 0;
        beeTarget = needCounter;
    }

    private void MoveDecisions()
    {
        //bees available for gathering

        int gatheringBees = ((attackMode) ? (beeTarget - attackWaveCounter * attackWaveBees) : beeTarget);
        int beesPerHive = gatheringBees/faction.hives.Count;

        List<CreatureLogic> allocatedBees = new List<CreatureLogic>();

        int index = 0;
        foreach (HiveLogic hive in faction.hives)
        {
            List<CreatureLogic> hiveGroup = new List<CreatureLogic>();
            for (int i=0; i < beesPerHive; i++)
            {
                if (!(faction.bees.Count <= i + index))
                {
                    hiveGroup.Add(faction.bees[i + index]);
                    allocatedBees.Add(faction.bees[i + index]);
                }
                else
                    break;
                index++;
            }

            PlantLogic gatherGoal = EcosystemLogic.instance.ClosestGatherablePlant(hive.GetUIPosition().position);
            
            if(gatherGoal != null)
            {
                foreach (CreatureLogic bee in faction.bees)
                {
                    if (bee.HasNoCommands() && (Vector3.Distance(gatherGoal.GetUIPosition().position, hive.GetUIPosition().position) < hiveTerritoryRange))
                    {
                        bee.EnqueueGoal(new MoveCommand(MoveType.Gather, Vector3.zero, gatherGoal.gameObject));
                    }
                }
            }
            //send the list of bees a command

        }

        //iterate through your list of bees until this amount of bees per hive is gathering plants

        //check for enough to attack
        if (attackMode && beeTarget == faction.bees.Count)
        {
            HiveLogic hiveAttackTarget = faction.ClosestEnemyHive(faction.AveragedHiveLocations());

            if(hiveAttackTarget != null)
            {

                int beeAmount = faction.bees.Count;
                int squadSize = attackWaveCounter * attackWaveBees;
                for (int i = 0; i < squadSize; i++)
                {
                    faction.bees[(beeAmount - squadSize + i)].EnqueueGoal(new MoveCommand(MoveType.AttackMove, Vector3.zero, hiveAttackTarget.gameObject));
                }
            }
            attackMode = false;
            attackWaveCounter++;
        }

        // gather with your gathering bees, split them into groups (divided by the amount of )
        //if you have enough bees to attack, start an attack on a hive probably need an attack move first
    }

    private void QueueBeeBuilding(int beeBuildQuota)
    {
        int index = 0;
        while(index < beeBuildQuota)
        {
            bool beesBuildable = false;
            foreach(HiveLogic hive in faction.hives)
            {
                if(hive.QueueBeeBuild()){
                    index += 1;
                    beesBuildable = true;
                }
                if(index == beeBuildQuota)
                {
                    return;
                }
            }
            if (!beesBuildable)
            {
                return;
            }
        }
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        BuildDecisions();
        MoveDecisions();
    }
}
