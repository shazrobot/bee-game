using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    public bool seasonProgressed;
    public bool monthProgressed;
    public bool dayProgressed;

    public int timeScaleInt;

    private DateTime date;

    private double secondsElapsed;
    private double prevTime;
    private int season;
    private int month;
    private int day;

    private int scaleMin = 0;
    private int scaleMax = 4;

    private int oldTimeScale;

    public DateTime GetDate()
    {
        return date;
    }

    public int GetMonth()
    {
        return date.Month;
    }

    public int GetDay()
    {
        return date.Day;
    }

    //0 for first season 3 for last
    public int GetSeason()
    {
        return ((date.Month-1)/3);
    }

    //returns what percentage of the year has elapsed
    public float GetYearProgress()
    {
        return ((float)date.DayOfYear) / 365.0f;
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

    void Awake()
    {
        instance = this;
        date = new DateTime(1, 1, 1);
        season = 0;
        month = 1;
        day = 1;
        timeScaleInt = 1;
    }

    void Update()
    {
        if (month != GetMonth())
        {
            monthProgressed = true;
            month = GetMonth();
        }
        else
        {
            monthProgressed = false;
        }

        if (season != GetSeason())
        {
            seasonProgressed = true;
            season = GetSeason();
        }
        else
        {
            seasonProgressed = false;
        }

        if (day != GetDay())
        {
            dayProgressed = true;
            day = GetDay();
        }
        else
        {
            dayProgressed = false;
        }

        prevTime = (int)secondsElapsed;
        secondsElapsed += Time.deltaTime;
        if (prevTime < (int)secondsElapsed)
        {
            date = date.AddDays(1);
        }
    }
}
