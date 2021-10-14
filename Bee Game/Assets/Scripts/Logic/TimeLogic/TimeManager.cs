using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Season { Wax, Wane}

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    public List<TimeManagerObserver> observers;

    public int timeScaleInt;

    private Season season;

    private int scaleMin = 0;
    private int scaleMax = 4;

    private int oldTimeScale;


    //New Parameters

    private float seasonLength = 45f; //in seconds
    private float seasonCounter = 0;

    private Season currentSeason;

    void Awake()
    {
        instance = this;
        season = Season.Wax;
        timeScaleInt = 1;
        NotifySeasonChange();
        NotifyAnyChange();
    }

    void FixedUpdate()
    {
        seasonCounter += Time.deltaTime;

        if (seasonCounter >= seasonLength)
        {
            AdvanceSeason();
        }

        NotifyAnyChange();
    }


    private void AdvanceSeason()
    {
        seasonCounter = 0;
        season = (season == Season.Wax) ? Season.Wane : Season.Wax;
        NotifySeasonChange();
    }

    //0 for first season 1 for last
    public Season GetSeason()
    {
        return currentSeason;
    }

    public float GetSeasonDuration()
    {
        return seasonLength;
    }

    public int GetTimeScale()
    {
        return timeScaleInt;
    }

    public void IncrementTimeScale()
    {
        timeScaleInt += 1;
        timeScaleInt = Mathf.Clamp(timeScaleInt, scaleMin, scaleMax);
        SetTimeScale();
    }

    public void DecrementTimeScale()
    {
        timeScaleInt -= 1;
        timeScaleInt = Mathf.Clamp(timeScaleInt, scaleMin, scaleMax);
        SetTimeScale();

        if (timeScaleInt == 0)
        {
            oldTimeScale = 1;
        }
    }

    public void ZeroTimeScale()
    {
        oldTimeScale = timeScaleInt;
        timeScaleInt = 0;
        SetTimeScale();
    }

    public void SetNormalTimeScale()
    {
        timeScaleInt = oldTimeScale;
        SetTimeScale();
    }

    private void SetTimeScale()
    {
        Time.timeScale = timeScaleInt;
    }

    private float SeasonProgress()
    {
        return seasonCounter / seasonLength;
    }

    private void NotifySeasonChange()
    {
        foreach(TimeManagerObserver observer in observers)
        {
            observer.SeasonChanged(season);
        }
    }

    private void NotifyAnyChange()
    {
        foreach (TimeManagerObserver observer in observers)
        {
            observer.SeasonLerp(season, SeasonProgress());
        }
    }
}
