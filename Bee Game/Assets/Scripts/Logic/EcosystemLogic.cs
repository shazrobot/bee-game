using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcosystemLogic : MonoBehaviour
{
    public static EcosystemLogic instance;

    public List<PlantLogic> plants;

    private void Start()
    {
        instance = this;
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
}
