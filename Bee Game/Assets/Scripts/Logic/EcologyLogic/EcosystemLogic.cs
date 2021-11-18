using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcosystemLogic : TimeManagerObserver
{
    public static EcosystemLogic instance;

    public List<PlantLogic> plants;

    public List<NoGrow> noGrowObjects;

    public GrassGrowthManager grassGrowthManager;

    public List<HiveBuildLocation> hiveBuildLocations;

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

    //Grass parameters

    private float waxHeight = 6;
    private float waxWidth = 0.6f;
    private int waxBend = 1;

    private float waneHeight = 3;
    private float waneWidth = 0.3f;
    private int waneBend = 4;

    private void Awake()
    {
        instance = this;

        Vector3 testPosition = new Vector3();
        StartupPlantPopulate();
        StartupNoGrowPopulate();
        StartupHiveBuildLocationPopulate();
    }

    private void StartupPlantPopulate()
    {
        plants.Clear();

        plants.AddRange(FindObjectsOfType<PlantLogic>());
    }

    private void StartupNoGrowPopulate()
    {
        noGrowObjects.Clear();

        noGrowObjects.AddRange(FindObjectsOfType<NoGrow>());
    }

    private void StartupHiveBuildLocationPopulate()
    {
        hiveBuildLocations.Clear();

        hiveBuildLocations.AddRange(FindObjectsOfType<HiveBuildLocation>());
    }

    public void AddHiveBuildLocation(HiveBuildLocation buildLocation)
    {
        if (!hiveBuildLocations.Contains(buildLocation))
        {
            hiveBuildLocations.Add(buildLocation);
        }
    }

    //can return null if there are no eligble locations
    public HiveBuildLocation ClosestHiveBuildLocation(Vector3 position)
    {
        float distance = Mathf.Infinity;
        HiveBuildLocation bestChoice = null;
        foreach (HiveBuildLocation hiveBuildLocation in hiveBuildLocations)
        {
            float dist = Vector3.Distance(hiveBuildLocation.transform.position, position);
            if((dist < distance) && !hiveBuildLocation.IsOccupied())
            {
                distance = dist;
                bestChoice = hiveBuildLocation;
            }
        }

        return bestChoice;
    }

    //take into account if it has pollen and is being gathered
    public PlantLogic ClosestGatherablePlant(Vector3 position)
    {
        float dist = Mathf.Infinity;
        float calc;
        PlantLogic closest = null;
        foreach (PlantLogic plant in plants)
        {
            calc = Vector3.Distance(plant.GetUIPosition().position, position);
            if (calc < dist && plant.HasPollen() && !plant.IsBeingPollinated())
            {
                closest = plant;
                dist = calc;
            }
        }
        return closest;
    }

    public PlantLogic ClosestGatherablePlantWithShortestQueue(Vector3 position, CreatureLogic bee, float GatherRangeCutoff)
    {
        float dist = Mathf.Infinity;
        int allocationSize = 1000;
        float calc;
        PlantLogic closest = null;
        foreach (PlantLogic plant in plants)
        {
            calc = Vector3.Distance(plant.GetUIPosition().position, position);
            if (plant.HasPollen() && calc < GatherRangeCutoff)
            {
                if (plant.GetGathererAllocationSizeExcludingBee(bee) < allocationSize)
                {
                    closest = plant;
                    dist = calc;
                    allocationSize = plant.GetGathererAllocationSizeExcludingBee(bee);
                }

                if (plant.GetGathererAllocationSizeExcludingBee(bee) == allocationSize && calc < dist)
                {
                    closest = plant;
                    dist = calc;
                }
            }
        }
        return closest;
    }

    public PlantLogic ClosestGatherablePlantOfTypeWithShortestQueue(Vector3 position, FlowerType type, CreatureLogic bee, float GatherRangeCutoff)
    {
        if (type == FlowerType.None)
        {
            return (ClosestGatherablePlant(position));
        }
        float dist = Mathf.Infinity;
        int allocationSize = 1000;
        float calc;
        PlantLogic closest = null;
        foreach (PlantLogic plant in plants)
        {
            calc = Vector3.Distance(plant.GetUIPosition().position, position);
            if (type == plant.GetFlowerType() && plant.HasPollen() && calc < GatherRangeCutoff)
            {                
                if (plant.GetGathererAllocationSizeExcludingBee(bee) < allocationSize)
                {
                    closest = plant;
                    dist = calc;
                    allocationSize = plant.GetGathererAllocationSizeExcludingBee(bee);
                }

                if (plant.GetGathererAllocationSizeExcludingBee(bee) == allocationSize && calc < dist)
                {
                    closest = plant;
                    dist = calc;
                }
            }            
        }
        return closest;
    }

    public PlantLogic ClosestGatherablePlantOfType(Vector3 position, FlowerType type)
    {
        if (type == FlowerType.None)
        {
            return (ClosestGatherablePlant(position));
        }
        float dist = Mathf.Infinity;
        float calc;
        PlantLogic closest = null;
        foreach (PlantLogic plant in plants)
        {
            calc = Vector3.Distance(plant.GetUIPosition().position, position);
            if (calc < dist && plant.HasPollen() && type == plant.GetFlowerType())
            {
                closest = plant;
                dist = calc;
            }
        }
        return closest;
    }

    public PlantLogic ClosestGatherableUnoccupiedPlantOfType(Vector3 position, FlowerType type)
    {
        if(type == FlowerType.None)
        {
            return (ClosestGatherablePlant(position));
        }
        float dist = Mathf.Infinity;
        float calc;
        PlantLogic closest = null;
        foreach (PlantLogic plant in plants)
        {
            calc = Vector3.Distance(plant.GetUIPosition().position, position);
            if (calc < dist && plant.HasPollen() && !plant.IsBeingPollinated() && type == plant.GetFlowerType())
            {
                closest = plant;
                dist = calc;
            }
        }
        return closest;
    }

    public int PlantsInRange(Vector3 position, float distance)
    {
        int counter = 0;

        foreach(PlantLogic plant in plants)
        {
            if(Vector3.Distance(plant.GetUIPosition().position, position) <= distance)
            {
                counter++;
            }
        }

        return counter;
    }

    public void PlantDied(PlantLogic plant)
    {
        plants.Remove(plant);
    }

    private void GrassReproduction()
    {
        //could be an ienum in the future
        StartCoroutine(grassGrowthManager.GrassReproductionCoroutine());
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

    public bool IsOutOfBounds(Vector3 position)
    {
        return !((position.x < xMax) &&
            (position.x > xMin) &&
            (position.z < zMax) &&
            (position.z > zMin));
    }

    //Plant reproduction functions

    public List<PlantLogic> PlantListRandomiser(List<PlantLogic> list)
    {
        List<PlantLogic> alpha = new List<PlantLogic>(list);
        for (int i = 0; i < list.Count; i++)
        {
            PlantLogic temp = alpha[i];
            int randomIndex = Random.Range(i, list.Count);
            alpha[i] = alpha[randomIndex];
            alpha[randomIndex] = temp;
        }

        return alpha;
    }

    private IEnumerator PlantReproduction(List<PlantLogic> reproducingPlants)
    {
        float timeBetweenSpawns = TimeManager.instance.GetSeasonDuration()/reproducingPlants.Count;
        foreach (PlantLogic plant in PlantListRandomiser(reproducingPlants))
        {
            SpawnPlant(plant);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    private void PlantReproduction(FlowerType flowerType)
    {
        List<PlantLogic> reproducingPlants = new List<PlantLogic>();
        foreach (PlantLogic plant in plants)
        {
            if (plant.IsPollinated() && plant.GetFlowerType() == flowerType)
            {
                reproducingPlants.Add(plant);
                plant.CompletedReproduction();
            }
                
            
        }
        foreach (PlantLogic plant in reproducingPlants)
        {
            SpawnPlant(plant);
        }
    }

    private void PlantReproductionIncremental(FlowerType flowerType)
    {
        List<PlantLogic> reproducingPlants = new List<PlantLogic>();
        foreach (PlantLogic plant in plants)
        {
            if (plant.IsPollinated() && plant.GetFlowerType() == flowerType)
            {
                reproducingPlants.Add(plant);
                plant.CompletedReproduction();
            }


        }

        StartCoroutine(PlantReproduction(reproducingPlants));
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

            if (plant.GetFlowerType() == FlowerType.Fertility)
            {
                grassGrowthManager.CreateCustomGrassNode(position);
            }
            //if this plant is a fertility plant, then create a custom Grass Node at this location
        }
    }

    private bool PlantPositionViable(Vector3 position, PlantLogic parentPlant)
    {
        if (IsOutOfBounds(position))
            return false;

        if (IsPositionOverWater(position))
            return false;

        position = ConformPositionToGround(position);

        foreach (PlantLogic plant in plants)
        {
            if (Vector3.Distance(position, plant.transform.position) < (plant.plantSpawnBoundary + parentPlant.plantSpawnBoundary))
                return false;
        }

        if (parentPlant.GetFlowerType() != FlowerType.Fertility && !grassGrowthManager.WithinRangeOfGrass(position))
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


        //uses a spiral equation to generate points to test for eligibility
        float theta;
        float radius;
        for (int i=0; i<6; i++)
        {
            theta = (i / 2f) + 3;
            radius = (float)1.618 * Mathf.Exp((2 / Mathf.PI) * theta);
            testPosition.x = (xdirection*radius * Mathf.Cos(theta)) + plantTemplate.transform.position.x;
            testPosition.y = plantTemplate.transform.position.y;
            testPosition.z = (zdirection*radius * Mathf.Sin(theta)) + plantTemplate.transform.position.z;
            if (PlantPositionViable(testPosition, plantTemplate))
            {
                testPosition = ConformPositionToGround(testPosition);
                return testPosition;
            }
        }

        return Vector3.zero;
    }


    //Observer Methods
    public override void SeasonChanged(Season season)
    {
        if (season == Season.Wax)
        {
            GrassReproduction();
            PlantReproductionIncremental(FlowerType.Fertility);
            PlantReproductionIncremental(FlowerType.Health);
            PlantReproductionIncremental(FlowerType.Lethality);
            PlantReproductionIncremental(FlowerType.Expulsion);
            PlantReproductionIncremental(FlowerType.Rapidity);
        }
    }

    public override void SeasonLerp(Season season, float seasonProgressed)
    {
        grassHeight = waxHeight;
        grassWidth = waxWidth;
        //grassTesselationAmount = 9;
        grassBendAmount = waxBend;
        topColour = ColourData.instance.springTopGrass;
        bottomColour = ColourData.instance.springBottomGrass;
        SetGlobalShaderVariables();
        /*
        if (season == Season.Wax)
        {
            if (seasonProgressed <= 0.33f)
            {
                float modLerp = 0.5f + seasonProgressed / 0.66f;
                grassHeight = Mathf.Lerp(waneHeight, waxHeight, modLerp);
                grassWidth = Mathf.Lerp(waneWidth, waxWidth, modLerp);
                //grassTesselationAmount = 9;
                grassBendAmount = Mathf.Lerp(waneBend, waxBend, modLerp);
                topColour = Color.Lerp(ColourData.instance.winterTopGrass, ColourData.instance.springTopGrass, modLerp);
                bottomColour = Color.Lerp(ColourData.instance.winterBottomGrass, ColourData.instance.springBottomGrass, modLerp);
                SetGlobalShaderVariables();
            }
            else if (seasonProgressed <= 0.67f)
            {
                grassHeight = waxHeight;
                grassWidth = waxWidth;
                //grassTesselationAmount = 9;
                grassBendAmount = waxBend;
                topColour = ColourData.instance.springTopGrass;
                bottomColour = ColourData.instance.springBottomGrass;
                SetGlobalShaderVariables();
            }
            else
            {
                float modLerp = (seasonProgressed -0.67f) / 0.66f;
                grassHeight = Mathf.Lerp(waxHeight, waneHeight, modLerp);
                grassWidth = Mathf.Lerp(waxWidth, waneWidth, modLerp);
                //grassTesselationAmount = 9;
                grassBendAmount = Mathf.Lerp(waxBend, waneBend, modLerp);
                topColour = Color.Lerp(ColourData.instance.springTopGrass, ColourData.instance.winterTopGrass, modLerp);
                bottomColour = Color.Lerp(ColourData.instance.springBottomGrass, ColourData.instance.winterBottomGrass, modLerp);
                SetGlobalShaderVariables();
            }
        }
        else if (season == Season.Wane)
        {
            if (seasonProgressed <= 0.33f)
            {
                float modLerp = 0.5f + (seasonProgressed) / 0.66f;
                grassHeight = Mathf.Lerp(waxHeight, waneHeight, modLerp);
                grassWidth = Mathf.Lerp(waxWidth, waneWidth, modLerp);
                //grassTesselationAmount = 9;
                grassBendAmount = Mathf.Lerp(waxBend, waneBend, modLerp);
                topColour = Color.Lerp(ColourData.instance.springTopGrass, ColourData.instance.winterTopGrass, modLerp);
                bottomColour = Color.Lerp(ColourData.instance.springBottomGrass, ColourData.instance.winterBottomGrass, modLerp);
                SetGlobalShaderVariables();
            }
            else if (seasonProgressed <= 0.67f)
            {
                grassHeight = waneHeight;
                grassWidth = waneWidth;
                //grassTesselationAmount = 9;
                grassBendAmount = waneBend;
                topColour = ColourData.instance.winterTopGrass;
                bottomColour = ColourData.instance.winterBottomGrass;
                SetGlobalShaderVariables();
            }
            else
            {
                float modLerp = (seasonProgressed - 0.67f) / 0.66f;
                grassHeight = Mathf.Lerp(waneHeight, waxHeight, modLerp);
                grassWidth = Mathf.Lerp(waneWidth, waxWidth, modLerp);
                //grassTesselationAmount = 9;
                grassBendAmount = Mathf.Lerp(waneBend, waxBend, modLerp);
                topColour = Color.Lerp(ColourData.instance.winterTopGrass, ColourData.instance.springTopGrass, modLerp);
                bottomColour = Color.Lerp(ColourData.instance.winterBottomGrass, ColourData.instance.springBottomGrass, modLerp);
                SetGlobalShaderVariables();
            }
        }
        */


    }
}
