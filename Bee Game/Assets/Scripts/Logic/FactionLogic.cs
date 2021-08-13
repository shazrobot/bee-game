using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionLogic : MonoBehaviour
{
    public string factionName;

    public int pollenAmount;

    public List<HiveLogic> hives;

    public List<CreatureLogic> bees;

    public List<FactionObserver> observers;

    public CreatureLogic beeTemplate;

    public HiveLogic hiveTemplate;

    public int BeePollenCost = 500;

    public int HivePollenCost = 1000;

    [SerializeField]
    private Color factionColour;

    [SerializeField]
    private Material beeMaterial;
    [SerializeField]
    private Material hiveMaterial;

    public List<FactionLogic> enemyFactions;

    //public List<FactionLogic> 

    private void Start()
    {
        beeTemplate.gameObject.SetActive(false);
        hiveTemplate.gameObject.SetActive(false);
        NotifyObservers();
        StartupHiveFactionUpdate();
        StartupBeeFactionUpdate();
    }

    public Color GetFactionColour()
    {
        return factionColour;
    }

    private void StartupHiveFactionUpdate()
    {
        foreach(HiveLogic hive in hives)
        {
            hive.SetCreationVariables(this, hiveMaterial);
        }
    }

    private void StartupBeeFactionUpdate()
    {
        foreach (CreatureLogic bee in bees)
        {
            bee.SetCreationVariables(this, beeMaterial);
        }
    }

    public void UpdatePollenAmount(int pollenChange)
    {
        pollenAmount += pollenChange;
        NotifyObservers();
    }

    public bool CanAffordBee()
    {
        return (pollenAmount >= BeePollenCost);
    }

    public bool CanAffordHive()
    {
        return (pollenAmount >= HivePollenCost);
    }

    public void BeeQueued()
    {
        pollenAmount -= BeePollenCost;
        NotifyObservers();
    }

    public bool CreateBee(HiveLogic hive)
    {
        CreatureLogic bee = Instantiate(beeTemplate) as CreatureLogic;

        bee.gameObject.SetActive(true);
        bee.SetCreationVariables(this, beeMaterial);
        bee.transform.SetParent(beeTemplate.transform.parent);
        bee.transform.position = hive.transform.position;
        bee.EnqueueGoal(new MoveCommand(MoveType.Move, hive.rallyPoint.position));

        bees.Add(bee);
        //only create bee if you can afford it
        return true;
    }

    public void CreateHive(HiveBuildLocation location)
    {
        pollenAmount -= HivePollenCost;
        NotifyObservers();

        HiveLogic hive = Instantiate(hiveTemplate) as HiveLogic;

        hive.gameObject.SetActive(true);
        hive.SetCreationVariables(this, hiveMaterial);
        hive.transform.SetParent(beeTemplate.transform.parent);
        hive.transform.position = location.transform.position;

        hives.Add(hive);

        location.Occupy();
    }

    public HiveLogic ClosestHive(Vector3 position)
    {
        float dist = Mathf.Infinity;
        float calc;
        HiveLogic closest = null;
        foreach (HiveLogic hive in hives)
        {
            calc = Vector3.Distance(hive.transform.position, position);
            if (calc < dist)
            {
                closest = hive;
                dist = calc;
            }
        }
        return closest;
    }

    public void AddObserver(FactionObserver observer)
    {
        observers.Add(observer);
        NotifyObservers();
    }

    private void NotifyObservers()
    {
        foreach (FactionObserver observer in observers)
        {
            observer.FactionUpdated(this);
        }
    }

    public bool IsEnemy(FactionLogic faction)
    {
        return enemyFactions.Contains(faction);
    }

    public List<SelectableLogic> GetAliveEnemyUnits()
    {
        List<SelectableLogic> enemyUnits = new List<SelectableLogic>();

        foreach (FactionLogic faction in enemyFactions)
        {
            foreach(CreatureLogic bee in faction.bees)
            {
                if(!bee.IsDead() && bee.isActiveAndEnabled)
                    enemyUnits.Add(bee);
            }
            foreach (HiveLogic hive in faction.hives)
            {
                if (!hive.IsDead() && hive.isActiveAndEnabled)
                    enemyUnits.Add(hive);
            }
        }

        return enemyUnits;
    }
}
