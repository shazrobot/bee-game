using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodLogic : MonoBehaviour
{
    public FoodData data;

    public void HarvestFood()
    {
        data.foodCurrent = Mathf.Max(0, data.foodCurrent- data.foodCollection);
    }
}
