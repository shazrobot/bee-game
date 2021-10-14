using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FriendlinessType { Friendly, Neutral, Hostile, None }
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

    private int beeCap;
    private int beeCapPerHive = 8;
    //public List<FactionLogic> 

    private void Start()
    {
        beeTemplate.gameObject.SetActive(false);
        hiveTemplate.gameObject.SetActive(false);
        NotifyObservers();
        StartupHiveFactionUpdate();
        StartupBeeFactionUpdate();
        UpdateBeeCap();
    }

    //Getters

    public Color GetFactionColour()
    {
        return factionColour;
    }

    public Vector3 AveragedHiveLocations()
    {
        Vector3 pos = Vector3.zero;

        foreach (HiveLogic hive in hives)
        {
            pos += hive.GetUIPosition().position;
        }

        pos /= hives.Count;

        return pos;
    }

    public bool IsAtBeeCap()
    {
        UpdateBeeCap();
        return (bees.Count >= beeCap);
    }

    public int GetBeeAmount()
    {
        return bees.Count;
    }

    public int GetBeeCap()
    {
        return beeCap;
    }

    public FriendlinessType GetFriendlinessOfSelectable(SelectableLogic selectable)
    {
        if(selectable.GetComponent<PlantLogic>() != null)
        {
            return FriendlinessType.Neutral;
        }
        if (selectable.GetComponent<HiveLogic>() != null)
        {
            HiveLogic hive = selectable.GetComponent<HiveLogic>();
            if (hives.Contains(hive))
                return FriendlinessType.Friendly;
            foreach (FactionLogic faction in enemyFactions)
            {
                if (faction.hives.Contains(hive))
                    return FriendlinessType.Hostile;
            }
            
        }
        if (selectable.GetComponent<CreatureLogic>() != null)
        {
            CreatureLogic bee = selectable.GetComponent<CreatureLogic>();
            if (bees.Contains(bee))
                return FriendlinessType.Friendly;
            foreach (FactionLogic faction in enemyFactions)
            {
                if (faction.bees.Contains(bee))
                    return FriendlinessType.Hostile;
            }
        }

        return FriendlinessType.None;
    }

    public List<CreatureLogic> FilterMyBees(List<CreatureLogic> beeList)
    {
        List<CreatureLogic> filteredBees = new List<CreatureLogic>();

        foreach(CreatureLogic bee in beeList)
        {
            if (IsMyBee(bee))
                filteredBees.Add(bee);
        }
        return filteredBees;
    }

    public bool IsMyBee(CreatureLogic bee)
    {
        return bees.Contains(bee);
    }

    public bool IsMyHive(HiveLogic hive)
    {
        return hives.Contains(hive);
    }

    //Updates

    private void StartupHiveFactionUpdate()
    {
        foreach(HiveLogic hive in hives)
        {
            hive.SetCreationVariables(this, hiveMaterial, hive.GetHiveLocation());
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

    public void UpdateBeeCap()
    {
        int newCap = 0;
        foreach(HiveLogic hive in hives)
        {
            if (!hive.IsBuilding())
                newCap += beeCapPerHive;
        }
        beeCap = newCap;
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

    public void BeeDied(CreatureLogic bee)
    {
        bees.Remove(bee);
    }

    public void HiveDied(HiveLogic hive)
    {
        hives.Remove(hive);
        UpdateBeeCap();
    }

    public bool CreateBee(HiveLogic hive)
    {
        if (!IsAtBeeCap())
        {
            CreatureLogic bee = Instantiate(beeTemplate) as CreatureLogic;

            bee.gameObject.SetActive(true);
            bee.SetCreationVariables(this, beeMaterial);
            bee.transform.SetParent(beeTemplate.transform.parent);
            bee.transform.position = hive.transform.position;
            bee.EnqueueGoal(new MoveCommand(MoveType.Move, hive.rallyPoint.position));

            bees.Add(bee);
            NotifyObservers();
            //only create bee if you can afford it
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public void CreateHive(HiveBuildLocation location)
    {
        pollenAmount -= HivePollenCost;
        NotifyObservers();

        HiveLogic hive = Instantiate(hiveTemplate) as HiveLogic;

        hive.gameObject.SetActive(true);
        hive.SetCreationVariables(this, hiveMaterial, location);
        hive.InitialiseConstruction();
        hive.transform.SetParent(beeTemplate.transform.parent);
        hive.transform.position = location.transform.position;

        hives.Add(hive);

        location.Occupy();
    }

    public HiveLogic ClosestBuiltHive(Vector3 position)
    {
        float dist = Mathf.Infinity;
        float calc;
        HiveLogic closest = null;
        foreach (HiveLogic hive in hives)
        {
            calc = Vector3.Distance(hive.transform.position, position);
            if (calc < dist & !hive.IsBuilding())
            {
                closest = hive;
                dist = calc;
            }
        }
        return closest;
    }

    public HiveLogic ClosestEnemyHive(Vector3 position)
    {
        float dist = Mathf.Infinity;
        float calc;
        HiveLogic closest = null;
        List<HiveLogic> enemyHivesList = new List<HiveLogic>();
        foreach (FactionLogic enemy in enemyFactions)
        {
            enemyHivesList.AddRange(enemy.hives);
        }
        foreach (HiveLogic hive in enemyHivesList)
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


    public SelectableLogic EnemyInRange(Vector3 position, float distance)
    {
        SelectableLogic enemyUnit = null;
        float closestMeasure = Mathf.Infinity;
        foreach (FactionLogic faction in enemyFactions)
        {
            foreach (CreatureLogic bee in faction.bees)
            {
                float dist = Vector3.Distance(bee.GetUIPosition().position, position);
                if (!bee.IsDead() && bee.isActiveAndEnabled && (dist < distance) && (dist < closestMeasure))
                {
                    enemyUnit = bee;
                    closestMeasure = dist;
                }
            }
        }
        return enemyUnit;
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
