using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeasonUIManager : TimeManagerObserver
{
    public static SeasonUIManager instance;

    public Image seasonSymbol;
    public Image seasonRadialRing;

    public Sprite waxSymbol;
    public Sprite waxRadialRing;

    public Sprite waneSymbol;
    public Sprite waneRadialRing;


    //Observer Methods

    //Swap over sprites
    public override void SeasonChanged(Season season)
    {
        if(season == Season.Wax)
        {
            seasonSymbol.sprite = waxSymbol;
            seasonRadialRing.sprite = waxRadialRing;
        }
        if (season == Season.Wane)
        {
            seasonSymbol.sprite = waneSymbol;
            seasonRadialRing.sprite = waneRadialRing;
        }
    }

    //LerpSeason
    public override void SeasonLerp(Season season, float seasonProgressed)
    {
        seasonRadialRing.fillAmount = seasonProgressed;
    }
}
