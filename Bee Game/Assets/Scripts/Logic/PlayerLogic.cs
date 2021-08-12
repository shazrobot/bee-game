using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    public static PlayerLogic instance;

    public FactionLogic factionLogic;

    private void Awake()
    {
        instance = this;
    }

    public void SubscribeToPlayersFactionObserver(FactionObserver factionObserver)
    {
        factionLogic.AddObserver(factionObserver);
    }
}
