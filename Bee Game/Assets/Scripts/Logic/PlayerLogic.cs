using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    public static PlayerLogic instance;

    public int pollenAmount;

    public List<HiveLogic> hives;

    public List<PlayerObserver> observers;

    private void Start()
    {
        instance = this;
        NotifyObservers();
    }

    public void UpdatePollenAmount(int pollenChange)
    {
        pollenAmount += pollenChange;
        NotifyObservers();
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

    private void NotifyObservers()
    {
        foreach (PlayerObserver observer in observers)
        {
            observer.PlayerUpdated();
        }
    }

    
}
