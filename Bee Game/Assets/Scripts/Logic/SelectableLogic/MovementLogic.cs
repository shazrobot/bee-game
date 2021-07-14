using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementLogic
{
    private static float distanceCheck = .5f;

    private static bool DistanceCheck(Transform trans, Vector3 destination)
    {
        return (Vector3.Distance(trans.position, destination) < distanceCheck);
    }

    private static void LookTowards(Transform trans, Vector3 bearing)
    {
        Vector3 newDirection = Vector3.RotateTowards(trans.forward, -bearing, 1f, 0f);
        trans.rotation = Quaternion.LookRotation(newDirection);
    }

    public static void MoveTowards(Transform trans, float moveSpeed, Vector3 destination)
    {
        Vector3 bearing = Vector3.Normalize(destination - trans.position);
        trans.position += bearing * moveSpeed * Time.deltaTime;
        LookTowards(trans, bearing);
    }

    public static bool ReachedDestination(Transform trans, Vector3 destination)
    {
        return DistanceCheck(trans, destination);
    }     
}
