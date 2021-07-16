using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimeManagerObserver : MonoBehaviour
{
    public abstract void SeasonChanged(Season season);
    public abstract void SeasonLerp(Season season, float seasonProgressed);
}
