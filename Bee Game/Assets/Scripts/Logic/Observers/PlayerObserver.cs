using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FactionObserver : MonoBehaviour
{
    public abstract void FactionUpdated(FactionLogic faction);
}
