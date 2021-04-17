using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementLogic : MonoBehaviour
{
    public MovementData data;
    public Transform model;

    private float distanceCheck = .05f;

    private bool DistanceCheck(Vector3 destination)
    {
        return (Vector3.Distance(data.position, destination) < distanceCheck);
    }

    private void MoveTowards()
    {
        data.position += data.bearing * data.moveSpeed * Time.deltaTime;
        model.position = data.position;
    }

    private void UpdateBearing()
    {
        if (data.movePathGoals.Count > 0)
        {
            if (!DistanceCheck(data.movePathGoals[0]))
            {
                data.bearing = Vector3.Normalize(data.movePathGoals[0] - data.position);
            }
            else
            {
                DequeueGoal();
                UpdateBearing();
            }
        }
        else
        {
            data.bearing = Vector3.zero;
        }
    }

    private void UpdateMovement()
    {
        UpdateBearing();
        MoveTowards();
    }

    public void ResetGoalToThis(Vector3 goal)
    {
        data.movePathGoals.Clear();
        data.movePathGoals.Add(goal);
    }

    public void EnqueueGoal(Vector3 goal)
    {
        data.movePathGoals.Add(goal);
    }

    private void DequeueGoal()
    {
        if (data.movePathGoals.Count > 0)
        {
            data.movePathGoals.RemoveAt(0);
        }
    }

    public void Update()
    {
        UpdateMovement();
    }
}
