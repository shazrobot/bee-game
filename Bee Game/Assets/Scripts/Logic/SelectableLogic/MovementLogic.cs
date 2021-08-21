using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationLogic
{
    static float alphaSpread = 5f;
    static float betaSpread = 2f;


    private static List<Vector3> ArrowHeadFormation(int listSize, Vector3 movePosition)
    {
        List<Vector3> formationPositions = new List<Vector3>(); 
        Vector3 root = movePosition;

        if (listSize != 0)
            formationPositions.Add(root);


        for (int i= 1; i <= listSize; i++)
        {
            Vector3 nPos = new Vector3(root.x + alphaSpread * (1+ ((i-1) / 2)) * (Mathf.Pow(-1, i+1)), root.y ,root.z - betaSpread * (1 + ((i -1) / 2)));
            formationPositions.Add(nPos);
        }


        return formationPositions;
    }

    public static void FormationMove(List<CreatureLogic> beeList, Vector3 movePosition, bool enqueueGoal, bool attackMove = false)
    {
        List<Vector3> formationPositions = ArrowHeadFormation(beeList.Count, movePosition);

        for(int i=0; i < beeList.Count; i++)
        {
            if (enqueueGoal)
            {
                beeList[i].EnqueueGoal(new MoveCommand(MoveType.Move, formationPositions[i]));
            }
            else
            {
                beeList[i].ResetCommandsToThis(new MoveCommand(MoveType.Move, formationPositions[i]));
            }
            
        }
    }
}

public class MovementLogic
{
    private static bool DistanceCheck(Transform trans, MoveCommand command, float distanceThreshold, float agentRadius)
    {
        float distanceCheck = distanceThreshold + agentRadius;

        if (command.IsObjectBased())
        {
            SelectableLogic selectable = command.objective.GetComponent<SelectableLogic>();
            distanceCheck += selectable.GetRadius();
        }
        return (Vector3.Distance(trans.position, command.GetDestination()) < distanceCheck);
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

        trans.position += bearing * moveSpeed * Time.deltaTime;
        LookTowards(trans, bearing);
    }

    public static bool ReachedDestination(Transform trans, MoveCommand command, float distanceThreshold, float agentRadius)
    {
        return DistanceCheck(trans, command, distanceThreshold, agentRadius);
    }     
}
