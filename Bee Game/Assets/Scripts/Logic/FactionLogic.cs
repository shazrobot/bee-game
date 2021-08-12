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

    public int BeePollenCost = 500;

    [SerializeField]
    private Material beeMaterial;
    [SerializeField]
    private Material hiveMaterial;

    private void Start()
    {
        beeTemplate.gameObject.SetActive(false);
        NotifyObservers();
        StartupHiveFactionUpdate();
        StartupBeeFactionUpdate();
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

        //only create bee if you can afford it
        return true;
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

}
