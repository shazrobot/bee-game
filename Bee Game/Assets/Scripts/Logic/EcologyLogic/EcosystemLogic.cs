using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcosystemLogic : TimeManagerObserver
{
    public static EcosystemLogic instance;

    public List<PlantLogic> plants;

    public GrassGrowthManager grassGrowthManager;

    //Blade height
    private float grassHeight = 5f; //between 1.5 and 5
    //Blade width
    private float grassWidth = 0.5f; //between 0.1 and 0.5
    //Tesselation uniform
    private float grassTesselationAmount = 4; //between 2 and 4
    //Blade forward amount
    private float grassBendAmount = 1; //between 1 and 4
    //Top Color
    [SerializeField]
    private Color topColour; //need 4 colour types
    //Bottom Color
    [SerializeField]
    private Color bottomColour; //need 4 colour types

    private void Start()
    {
        instance = this;
        SetSeasonalGrassShaderVariables(Season.Summer);
    }

    public PlantLogic ClosestGatherablePlant(Vector3 position)
    {
        float dist = Mathf.Infinity;
        float calc;
        PlantLogic closest = null;
        foreach (PlantLogic plant in plants)
        {
            calc = Vector3.Distance(plant.transform.position, position);
            if (calc < dist && plant.HasPollen())
            {
                closest = plant;
                dist = calc;
            }
        }
        return closest;
    }

    private void GrassReproduction()
    {
        //could be an enum in the future
        grassGrowthManager.GrassReproduction();
    }

    public override void SeasonChanged(Season season)
    {
        if (season == Season.Spring)
        {
            GrassReproduction();
        }
        SetSeasonalGrassShaderVariables(season);
    }

    private void SetGlobalShaderVariables()
    {
        Shader.SetGlobalFloat("_SeasonalGrassHeight", grassHeight);
        Shader.SetGlobalFloat("_SeasonalGrassWidth", grassWidth);
        Shader.SetGlobalFloat("_SeasonalTesselation", grassTesselationAmount);
        Shader.SetGlobalFloat("_SeasonalGrassBend", grassBendAmount);
        Shader.SetGlobalColor("_SeasonalTopColour", topColour);
        Shader.SetGlobalColor("_SeasonalBottomColour", bottomColour);
    }

    private void GrassProgress()
    {
        //need current season
        //need decimal between 0 and 1 indicating how progressed through the month.
    }

    public override void SeasonLerp(Season season, float seasonProgressed)
    {
        if (season == Season.Spring)
        {
            float modLerp = seasonProgressed / 2f;
            grassHeight = Mathf.Lerp(0, 5, modLerp);
            grassWidth = Mathf.Lerp(0, 0.5f, modLerp);
            //grassTesselationAmount = 4;
            grassTesselationAmount = Mathf.Lerp(3, 5, modLerp);
            grassBendAmount = Mathf.Lerp(1, 2, modLerp);
            topColour = Color.Lerp(ColourData.instance.springTopGrass, ColourData.instance.summerTopGrass, modLerp);
            bottomColour = Color.Lerp(ColourData.instance.springBottomGrass, ColourData.instance.summerBottomGrass, modLerp);
            SetGlobalShaderVariables();
        }
        else if (season == Season.Summer)
        {
            float modLerp = (seasonProgressed / 2f)+0.5f;
            grassHeight = Mathf.Lerp(0, 5, modLerp);
            grassWidth = Mathf.Lerp(0, 0.5f, modLerp);
            //grassTesselationAmount = 4;
            grassTesselationAmount = Mathf.Lerp(3, 5, modLerp);
            //grassBendAmount = Mathf.Lerp(1, 2, modLerp);
            topColour = Color.Lerp(ColourData.instance.springTopGrass, ColourData.instance.summerTopGrass, modLerp);
            bottomColour = Color.Lerp(ColourData.instance.springBottomGrass, ColourData.instance.summerBottomGrass, modLerp);
            SetGlobalShaderVariables();
        }
        else if (season == Season.Autumn)
        {
            grassHeight = 5;
            grassWidth = 0.5f;
            //grassTesselationAmount = 4;
            grassTesselationAmount = Mathf.Lerp(5, 3, seasonProgressed);
            grassBendAmount = Mathf.Lerp(2, 3, seasonProgressed);
            topColour = Color.Lerp(ColourData.instance.summerTopGrass, ColourData.instance.autumnTopGrass, seasonProgressed);
            bottomColour = Color.Lerp(ColourData.instance.summerBottomGrass, ColourData.instance.autumnBottomGrass, seasonProgressed);
            SetGlobalShaderVariables();
        }
        else if (season == Season.Winter)
        {
            grassHeight = 5;
            grassWidth = 0.5f;
            //grassTesselationAmount = 4;
            grassTesselationAmount = Mathf.Lerp(3, 0, seasonProgressed);
            grassBendAmount = Mathf.Lerp(3, 4, seasonProgressed);
            topColour = Color.Lerp(ColourData.instance.autumnTopGrass, ColourData.instance.winterTopGrass, seasonProgressed);
            bottomColour = Color.Lerp(ColourData.instance.autumnBottomGrass, ColourData.instance.winterBottomGrass, seasonProgressed);
            SetGlobalShaderVariables();
        }
    }

    private void SetSeasonalGrassShaderVariables(Season season)
    {
        return;
        if (season == Season.Spring)
        {
            grassHeight = 1.5f; //between 1.5 and 5
            grassWidth = 0.2f; //between 0.2 and 0.5
            grassTesselationAmount = 3; //between 2 and 4
            grassBendAmount = 1; //between 1 and 4
            topColour = ColourData.instance.springTopGrass; //need 4 colour types
            bottomColour = ColourData.instance.springBottomGrass; //need 4 colour types
            SetGlobalShaderVariables();
        }
        else if (season == Season.Summer)
        {
            grassHeight = 5f; //between 1.5 and 5
            grassWidth = 0.5f; //between 0.1 and 0.5
            grassTesselationAmount = 4; //between 2 and 4
            grassBendAmount = 2; //between 1 and 4
            topColour = ColourData.instance.summerTopGrass; //need 4 colour types
            bottomColour = ColourData.instance.summerBottomGrass; //need 4 colour types
            SetGlobalShaderVariables();
        }
        else if (season == Season.Autumn)
        {
            grassHeight = 5f; //between 1.5 and 5
            grassWidth = 0.5f; //between 0.1 and 0.5
            grassTesselationAmount = 3; //between 2 and 4
            grassBendAmount = 3; //between 1 and 4
            topColour = ColourData.instance.autumnTopGrass; //need 4 colour types
            bottomColour = ColourData.instance.autumnBottomGrass; //need 4 colour types
            SetGlobalShaderVariables();
        }
        else if (season == Season.Winter)
        {
            grassHeight = 5f; //between 1.5 and 5
            grassWidth = 0.5f; //between 0.1 and 0.5
            grassTesselationAmount = 0; //between 2 and 4
            grassBendAmount = 4; //between 1 and 4
            topColour = ColourData.instance.winterTopGrass; //need 4 colour types
            bottomColour = ColourData.instance.winterBottomGrass; //need 4 colour types
            SetGlobalShaderVariables();
        }
    }
}
