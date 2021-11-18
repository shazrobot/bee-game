using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackGroupType { Defense, Crush, HitAndRun}

public class BeeAttackGroup
{
    public List<CreatureLogic> beeList;

    public bool Enabled;
    private FactionLogic faction;
    private Vector3 attackPoint;
    private float attackPointRadius;
    private Vector3 rallyPoint;
    private float rallyPointRadius = 25;

    private AttackGroupType groupType;

    private bool rallying;

    public BeeAttackGroup()
    {
        Enabled = false;
        beeList = new List<CreatureLogic>();
    }

    public void Enable(Vector3 point, float radius, List<CreatureLogic> bees, AttackGroupType group, FactionLogic fac)
    {
        attackPoint = point;
        attackPointRadius = radius;
        beeList = bees;
        groupType = group;
        rallyPoint = AverageLocation(bees);
        Enabled = true;
        rallying = true;
        faction = fac;
        //send the group to the rally point
        foreach (CreatureLogic bee in beeList)
        {
            bee.ResetCommandsToThis(new MoveCommand(MoveType.Move, rallyPoint));
        }
    }

    public void Disable()
    {
        Enabled = false;
    }

    public bool Contains(CreatureLogic bee)
    {
        return beeList.Contains(bee);
    }

    private Vector3 AverageLocation(List<CreatureLogic> beeList)
    {
        Vector3 middlePoint = new Vector3(0, 0, 0);

        foreach (CreatureLogic bee in beeList)
        {
            middlePoint += bee.GetUIPosition().position;
        }
        middlePoint /= beeList.Count;

        return middlePoint;
    }

    private void DissolveAttack()
    {
        foreach (CreatureLogic bee in beeList)
        {
            if(!bee.IsDead())
                bee.ResetCommandsToThis(new MoveCommand(MoveType.Move, rallyPoint));
        }
        beeList.Clear();
        Disable();
    }

    public bool proximityCheck()
    {
        bool allBeesPresent = true;
        foreach(CreatureLogic bee in beeList)
        {
            if ((Vector3.Distance(bee.GetUIPosition().position, rallyPoint) > rallyPointRadius) && !bee.IsDead())
            {
                allBeesPresent = false;
                return allBeesPresent;
            }
        }
        return allBeesPresent;
    }

    private bool AllDead()
    {
        foreach (CreatureLogic bee in beeList)
        {
            if (!bee.IsDead())
            {
                return false;
            }
        }

        return true;
    }

    public void UpdateCommands()
    {
        
        if (groupType != AttackGroupType.Defense && rallying)//rally group only for crush and hit and run groups
        {
            //check if group are within rally point
            //if group is in rally point, set rallying to false
            //initiate attack move towards location
            if (proximityCheck())//check for rally point proximity for your dudes
            {
                rallying = false;
                foreach (CreatureLogic bee in beeList)
                {
                    bee.ResetCommandsToThis(new MoveCommand(MoveType.AttackMove, attackPoint));
                }
            }
        }
        
        if(groupType == AttackGroupType.Defense && rallying)
        {
            rallying = false;
            foreach (CreatureLogic bee in beeList)
            {
                bee.ResetCommandsToThis(new MoveCommand(MoveType.AttackMove, attackPoint));
            }
        }

        

        if (!rallying)
        {
            foreach (CreatureLogic bee in beeList)
            {
                if(!bee.IsDead() && bee.HasNoCommands())
                {
                    bee.ResetCommandsToThis(new MoveCommand(MoveType.AttackMove, attackPoint));
                }
            }

            int hiveInclusiveEnemyAmount = faction.AllEnemiesAroundPoint(attackPoint, attackPointRadius).Count;
            int hiveExclusiveEnemyAmount = faction.EnemyBeesAroundPoint(attackPoint, attackPointRadius).Count;

            if (groupType == AttackGroupType.HitAndRun && hiveExclusiveEnemyAmount > 0)
            {
                DissolveAttack();
                Debug.Log("Dissolving Hit and run");
            }
            if ((groupType == AttackGroupType.Defense || groupType == AttackGroupType.Crush || groupType == AttackGroupType.HitAndRun) && hiveInclusiveEnemyAmount == 0)
            {
                DissolveAttack();
            }
        }

        if (AllDead())
        {
            DissolveAttack();
        }


    }

}

public class FactionAI : MonoBehaviour
{
    public FactionLogic faction;

    public float hiveTerritoryRange = 100;

    private bool attackMode = true;

    private int attackWaveCounter = 1;
    private int attackWaveBees = 3;

    private List<CreatureLogic> gatheringBees;


    private float decisionTime = 0.5f;
    private float decisionCounter = 0f;

    private BeeAttackGroup defenseGroup = new BeeAttackGroup();
    private BeeAttackGroup crushGroup = new BeeAttackGroup();
    private BeeAttackGroup hitAndRunGroup = new BeeAttackGroup();

    private void CombatDecisions()
    {
        HiveLogic defenseTest = faction.EnemiesAroundAnyHive(hiveTerritoryRange);
        if (defenseTest != null && !defenseGroup.Enabled)// enemy bees in range of a hive
        {
            Debug.Log("initiate defensive attack");
            List<CreatureLogic> defenseBees = AllocateClosestBees(faction.EnemiesAroundHive(defenseTest, hiveTerritoryRange).Count, defenseTest.GetUIPosition().position);
            defenseGroup.Enable(defenseTest.GetUIPosition().position, hiveTerritoryRange, defenseBees, AttackGroupType.Defense, faction);
        }
        else
        {
            if (!(crushGroup.Enabled || hitAndRunGroup.Enabled))
            {
                AttackOppurtunityCheck();
            }            
        }

        if (crushGroup.Enabled)
        {
            crushGroup.UpdateCommands();
        }
        if (hitAndRunGroup.Enabled)
        {
            hitAndRunGroup.UpdateCommands();
        }
        if (defenseGroup.Enabled)
        {
            defenseGroup.UpdateCommands();
        }

        //do command group updates
    }

    private void EconomicDecisions()
    {
        if(faction.GetBeeCap() <= (faction.bees.Count + faction.GetQueuedBeeAmount()))
        {
            if (faction.CanAffordHive())
            {
                HiveBuildLocation location = EcosystemLogic.instance.ClosestHiveBuildLocation(faction.AveragedHiveLocations());

                if (location != null)
                {
                    faction.CreateHive(location);
                }
            }
        }
        else
        {
            foreach(HiveLogic hive in faction.hives)
            {
                if(hive.beeQueue == 0  && faction.CanAffordBee())
                {
                    hive.QueueBeeBuild();
                }
            }
        }
    }

    private void GatheringDecisions()
    {
        //split idle bees amongst the hives that exist
        int hiveIndex = 0;
        GatheringBeesUpdate();
        foreach(CreatureLogic bee in gatheringBees)
        {
            if (bee.IsIdle())
            {
                //sends idle bees to closest gatherable plant to a hive
                PlantLogic gatherGoal = EcosystemLogic.instance.ClosestGatherablePlantWithShortestQueue(faction.hives[hiveIndex].GetUIPosition().position, bee, hiveTerritoryRange);


                if (gatherGoal == null)
                {
                    hiveIndex++;
                    if (hiveIndex >= faction.hives.Count)
                        hiveIndex = 0;
                    gatherGoal = EcosystemLogic.instance.ClosestGatherablePlantWithShortestQueue(faction.hives[hiveIndex].GetUIPosition().position, bee, hiveTerritoryRange);
                }

                if(gatherGoal != null)
                    bee.EnqueueGoal(new MoveCommand(MoveType.Gather, Vector3.zero, gatherGoal.gameObject));

                //if a hive doesn't have any plants around it, increment past it

                hiveIndex++;
                if (hiveIndex >= faction.hives.Count)
                    hiveIndex = 0;
            }
        }
    }

    private void GatheringBeesUpdate()
    {
        gatheringBees = new List<CreatureLogic>(faction.bees);
        foreach (CreatureLogic item in crushGroup.beeList) gatheringBees.Remove(item);
        foreach (CreatureLogic item in hitAndRunGroup.beeList) gatheringBees.Remove(item);
        foreach (CreatureLogic item in defenseGroup.beeList) gatheringBees.Remove(item);
    }

    private void AttackOppurtunityCheck(int factionIndex = 0)
    {
        if(factionIndex >= faction.enemyFactions.Count)
        {
            return;
        }

        //evaluate
        if (WeHaveMoreBees(faction.enemyFactions[factionIndex]))//do I have more bees than my enemy?
        {
            Debug.Log("initiate crush attack");
            //make hit and run group
            List<CreatureLogic> crushBees = AllocateClosestBees(gatheringBees.Count, 
                faction.enemyFactions[factionIndex].hives[0].GetUIPosition().position);
            crushGroup.Enable(faction.ClosestEnemyHive(faction.AveragedHiveLocations()).GetUIPosition().position, 
                hiveTerritoryRange, crushBees, AttackGroupType.Crush, faction);
        }
        else
        {
            HiveLogic vulnerabilityTest = faction.enemyFactions[factionIndex].AlliesAroundAnyHive(hiveTerritoryRange);

            if (vulnerabilityTest != null && faction.bees.Count > 3) //do they have any vulnerable hives?
            {
                Debug.Log("initiate hit and run attack");
                //make hit and run group
                List<CreatureLogic> hitAndRunBees = AllocateClosestBees(3, vulnerabilityTest.GetUIPosition().position);
                hitAndRunGroup.Enable(vulnerabilityTest.GetUIPosition().position, hiveTerritoryRange, hitAndRunBees, AttackGroupType.HitAndRun, faction);
            }
            else
            {
                AttackOppurtunityCheck(factionIndex + 1);
            }
        }
    }

    private bool WeHaveMoreBees(FactionLogic enemyFaction)
    {
        GatheringBeesUpdate();
        if (enemyFaction.hives.Count == 0) return false;
        return (enemyFaction.bees.Count * 1.5 < gatheringBees.Count && gatheringBees.Count >= 5);
    }

    private bool InAttackGroup(CreatureLogic bee)
    {
        return defenseGroup.Contains(bee) || crushGroup.Contains(bee) || hitAndRunGroup.Contains(bee);

    }

    private List<CreatureLogic> AllocateClosestBees(int beeAmount, Vector3 destination)
    {
        GatheringBeesUpdate();
        List<CreatureLogic> allocatedList = new List<CreatureLogic>();
        int i = 0;
        while (i <= beeAmount && i < gatheringBees.Count)
        {
            allocatedList.Add(gatheringBees[i]);
            i++;
        }

        //Yet to implement:
        //sort the list according to proximity to destination

        return allocatedList;
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        // 2 decisions a second
        decisionCounter += Time.deltaTime;

        if(decisionCounter >= decisionTime)
        {
            decisionCounter -= decisionTime;
            if (faction.hives.Count > 0)
            {
                CombatDecisions();
                EconomicDecisions();
                GatheringDecisions();
            }
        }

        
        

        //BuildDecisions();
        //MoveDecisions();
    }
}
