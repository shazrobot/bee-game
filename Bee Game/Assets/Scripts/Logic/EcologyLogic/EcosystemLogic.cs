using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcosystemLogic : TimeManagerObserver
{
    public static EcosystemLogic instance;

    public List<PlantLogic> plants;

    public List<NoGrow> noGrowObjects;

    public GrassGrowthManager grassGrowthManager;

    public List<HiveBuildLocation> hiveBuildLocations = new List<HiveBuildLocation>();

    //Blade height
    private float grassHeight = 5f; //between 1.5 and 5
    //Blade width
    private float grassWidth = 0.5f; //between 0.1 and 0.5
    //Tesselation uniform
    private float grassTesselationAmount = 6; //between 2 and 4
    //Blade forward amount
    private float grassBendAmount = 1; //between 1 and 4
    //Top Color
    [SerializeField]
    private Color topColour; //need 4 colour types
    //Bottom Color
    [SerializeField]
    private Color bottomColour; //need 4 colour types

    //Good to replace this with a map static class which has map data
    [SerializeField]
    private float xMax = 200;
    [SerializeField]
    private float xMin = -200;
    [SerializeField]
    private float zMax = 200;
    [SerializeField]
    private float zMin = -200;

    private float rayCastGroundDetectionHeight = 100f;
    private float rayCastLength = 150f;
    [SerializeField]
    private Collider water;
    [SerializeField]
    private Collider ground;
    private void Awake()
    {
        instance = this;

        Vector3 testPosition = new Vector3();

    }

    public void AddHiveBuildLocation(HiveBuildLocation buildLocation)
    {
        if (!hiveBuildLocations.Contains(buildLocation))
        {
            hiveBuildLocations.Add(buildLocation);
        }
    }


    //take into account if it has pollen and is being gathered
    public PlantLogic ClosestGatherablePlant(Vector3 position)
    {
        float dist = Mathf.Infinity;
        float calc;
        PlantLogic closest = null;
        foreach (PlantLogic plant in plants)
        {
            calc = Vector3.Distance(plant.transform.position, position);
            if (calc < dist && plant.HasPollen() && !plant.IsBeingPollinated())
            {
                closest = plant;
                dist = calc;
            }
        }
        return closest;
    }

    private void GrassReproduction()
    {
        //could be an ienum in the future
        grassGrowthManager.GrassReproduction();
    }

    

    public override void SeasonChanged(Season season)
    {
        if (season == Season.Spring)
        {
            GrassReproduction();
            PlantReproduction();
        }
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

    public override void SeasonLerp(Season season, float seasonProgressed)
    {
        if (season == Season.Spring)
        {
            float modLerp = seasonProgressed / 2f;
            grassHeight = Mathf.Lerp(2, 5, seasonProgressed);
            grassWidth = Mathf.Lerp(0.2f, 0.5f, seasonProgressed);
            grassTesselationAmount = 9;
            //grassTesselationAmount = Mathf.Lerp(3, 5, modLerp);
            grassBendAmount = Mathf.Lerp(4, 1, seasonProgressed);
            topColour = Color.Lerp(ColourData.instance.winterTopGrass, ColourData.instance.springTopGrass, seasonProgressed);
            bottomColour = Color.Lerp(ColourData.instance.winterBottomGrass, ColourData.instance.springBottomGrass, seasonProgressed);
            SetGlobalShaderVariables();
        }
        else if (season == Season.Summer)
        {
            float modLerp = (seasonProgressed / 2f)+0.5f;
            //grassHeight = Mathf.Lerp(1, 5, modLerp);
            //grassWidth = Mathf.Lerp(0.1f, 0.5f, modLerp);
            grassTesselationAmount = 9;
            //grassTesselationAmount = Mathf.Lerp(3, 5, modLerp);
            grassBendAmount = Mathf.Lerp(1, 2, seasonProgressed);
            topColour = Color.Lerp(ColourData.instance.springTopGrass, ColourData.instance.summerTopGrass, seasonProgressed);
            bottomColour = Color.Lerp(ColourData.instance.springBottomGrass, ColourData.instance.summerBottomGrass, seasonProgressed);
            SetGlobalShaderVariables();
        }
        else if (season == Season.Autumn)
        {
            grassHeight = 5;
            grassWidth = 0.5f;
            grassTesselationAmount = 9;
            //grassTesselationAmount = Mathf.Lerp(5, 3, seasonProgressed);
            grassBendAmount = Mathf.Lerp(2, 3, seasonProgressed);
            topColour = Color.Lerp(ColourData.instance.summerTopGrass, ColourData.instance.autumnTopGrass, seasonProgressed);
            bottomColour = Color.Lerp(ColourData.instance.summerBottomGrass, ColourData.instance.autumnBottomGrass, seasonProgressed);
            SetGlobalShaderVariables();
        }
        else if (season == Season.Winter)
        {
            grassHeight = Mathf.Lerp(5, 2, seasonProgressed);
            grassWidth = Mathf.Lerp(0.5f, 0.2f, seasonProgressed);
            //grassHeight = 5;
            //grassWidth = 0.5f;
            grassTesselationAmount = 9;
            //grassTesselationAmount = Mathf.Lerp(3, 0, seasonProgressed);
            grassBendAmount = Mathf.Lerp(3, 4, seasonProgressed);
            topColour = Color.Lerp(ColourData.instance.autumnTopGrass, ColourData.instance.winterTopGrass, seasonProgressed);
            bottomColour = Color.Lerp(ColourData.instance.autumnBottomGrass, ColourData.instance.winterBottomGrass, seasonProgressed);
            SetGlobalShaderVariables();
        }
    }

    public bool IsOutOfBounds(Vector3 position)
    {
        return !((position.x < xMax) &&
            (position.x > xMin) &&
            (position.z < zMax) &&
            (position.z > zMin));
    }

    //Plant reproduction functions

    private void PlantReproduction()
    {
        List<PlantLogic> reproducingPlants = new List<PlantLogic>();
        foreach (PlantLogic plant in plants)
        {
            if (plant.IsPollinated())
                reproducingPlants.Add(plant);
            plant.CompletedReproduction();
        }
        foreach (PlantLogic plant in reproducingPlants)
        {
            SpawnPlant(plant);
        }
    }

    private void SpawnPlant(PlantLogic plantTemplate)
    {
        Vector3 position = PlantSpawnPositionGeneration(plantTemplate);
        if (position != Vector3.zero)
        {
            PlantLogic plant = Instantiate(plantTemplate) as PlantLogic;

            plant.gameObject.SetActive(true);
            plant.transform.parent = plantTemplate.transform.parent;

            plant.transform.position = position;

            plant.transform.eulerAngles = new Vector3(0,Random.Range(0, 360),0);

            plants.Add(plant);
        }
    }

    private bool PlantPositionViable(Vector3 position)
    {
        if (IsOutOfBounds(position))
            return false;

        if (IsPositionOverWater(position))
            return false;

        position = ConformPositionToGround(position);

        foreach (PlantLogic plant in plants)
        {
            if (Vector3.Distance(position, plant.transform.position) < plant.plantSpawnBoundary)
                return false;
        }

        if (!grassGrowthManager.WithinRangeOfGrass(position))
            return false;

        if (IsPositionOverNoGrow(position))
            return false;

        return true;
    }

    private bool IsPositionOverNoGrow(Vector3 position)
    {
        Vector3 rayOrigin = new Vector3(position.x, rayCastGroundDetectionHeight, position.z);
        Ray ray = new Ray(rayOrigin, Vector3.down);
        RaycastHit groundRaycastHit;
        RaycastHit noGrowRaycastHit;

        foreach (NoGrow noGrow in noGrowObjects)
        {
            Collider collider = noGrow.GetComponent<Collider>();
            if(collider != null)
            {
                if (collider.Raycast(ray, out noGrowRaycastHit, rayCastLength))
                {
                    if (ground.Raycast(ray, out groundRaycastHit, rayCastLength))
                    {
                        if (groundRaycastHit.point.y < noGrowRaycastHit.point.y)
                            return true;
                    }
                    else
                        return true;
                }
            }
        }

        return false;
    }

    private bool IsPositionOverWater(Vector3 position)
    {
        Vector3 rayOrigin = new Vector3(position.x, rayCastGroundDetectionHeight, position.z);
        Ray ray = new Ray(rayOrigin, Vector3.down);
        RaycastHit groundRaycastHit;
        RaycastHit waterRaycastHit;

        
        if(water.Raycast(ray, out waterRaycastHit, rayCastLength))
        {
            if (ground.Raycast(ray, out groundRaycastHit, rayCastLength))
            {
                if (groundRaycastHit.point.y < waterRaycastHit.point.y)
                    return true;
            }
            else
                return true;
        }

        return false;
    }

    private Vector3 ConformPositionToGround(Vector3 position)
    {
        Vector3 rayOrigin = new Vector3(position.x, rayCastGroundDetectionHeight, position.z);
        Ray ray = new Ray(rayOrigin, Vector3.down);
        RaycastHit groundRaycastHit;
        if (ground.Raycast(ray, out groundRaycastHit, rayCastLength))
        {
            return new Vector3(position.x, groundRaycastHit.point.y, position.z);
        }
        return position;
    }

    private Vector3 PlantSpawnPositionGeneration(PlantLogic plantTemplate)
    {
        Vector3 testPosition = new Vector3();

        //randomises direction of the spiral
        int xdirection = (Random.Range(0, 6) > 2) ? -1 : 1;
        int zdirection = (Random.Range(0, 6) > 2) ? -1 : 1;

        //Debug.Log(string.Format("x direction: {0}, z direction: {1}", xdirection, zdirection));

        //uses a spiral equation to generate points to test for eligibility
        float theta;
        float radius;
        for (int i=0; i<5; i++)
        {
            theta = (i / 2f) + 3;
            radius = (float)1.618 * Mathf.Exp((2 / Mathf.PI) * theta);
            testPosition.x = (xdirection*radius * Mathf.Cos(theta)) + plantTemplate.transform.position.x;
            testPosition.y = plantTemplate.transform.position.y;
            testPosition.z = (zdirection*radius * Mathf.Sin(theta)) + plantTemplate.transform.position.z;
            if (PlantPositionViable(testPosition))
            {
                //Debug.Log(string.Format("Making flower at: X; {0}, Z; {1}, R; {2}, i; {3}", testPosition.x, testPosition.z, radius, i));
                testPosition = ConformPositionToGround(testPosition);
                return testPosition;
            }
        }

        return Vector3.zero;
    }
}
