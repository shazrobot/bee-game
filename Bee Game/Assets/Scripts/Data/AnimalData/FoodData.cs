using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FoodType { Pollen };

[CreateAssetMenu(fileName = "FoodData", menuName = "ScriptableObjects/FoodData", order = 2)]
public class FoodData : ScriptableObject
{
    public FoodType foodType;
    public int foodCurrent;
    public int foodCollection;
}
