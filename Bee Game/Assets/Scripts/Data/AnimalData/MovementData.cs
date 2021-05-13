using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementData", menuName = "ScriptableObjects/MovementData", order = 1)]
public class MovementData : ScriptableObject
{
    public Vector3 position;

    public List<Vector3> movePathGoals;

    public Vector3 bearing;
    public float moveSpeed;

}
