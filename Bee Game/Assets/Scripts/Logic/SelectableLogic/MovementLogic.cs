using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementLogic
{
    private static bool DistanceCheck(Transform trans, Vector3 destination, float distanceThreshold)
    {
        return (Vector3.Distance(trans.position, destination) < distanceThreshold);
    }

    private static void LookTowards(Transform trans, Vector3 bearing)
    {
        Vector3 lookDir = bearing;
        lookDir.y = 0;

        Vector3 newDirection = Vector3.RotateTowards(trans.forward, -lookDir, 1f, 0f);


        trans.rotation = Quaternion.LookRotation(newDirection);
    }

    public static void MoveTowards(Transform trans, float moveSpeed, Vector3 destination)
    {
        Vector3 bearing = Vector3.Normalize(destination - trans.position);

        if(Vector3.Distance(destination, trans.position) < moveSpeed * Time.deltaTime)
        {
            Debug.Log("close enough to slow down");
            trans.position += bearing * Vector3.Distance(destination, trans.position);
        }
        else
        {

            trans.position += bearing * moveSpeed * Time.deltaTime;
        }
        LookTowards(trans, bearing);
    }

    public static bool ReachedDestination(Transform trans, Vector3 destination, float distanceThreshold)
    {
        return DistanceCheck(trans, destination, distanceThreshold);
    }     
}
