using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class FormationLogic
{
    static float alphaSpread = 6f;
    static float betaSpread = 3f;


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
        //float distanceCheck = distanceThreshold + agentRadius;
        float distanceCheck = distanceThreshold;

        if (command.IsObjectBased())
        {
            SelectableLogic selectable = command.objective.GetComponent<SelectableLogic>();
            distanceCheck += selectable.GetRadius() + agentRadius + distanceCheck;
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

    public static void IdleAvoidanceOfBees(CreatureLogic idleBee, float moveSpeed, float unitRadius)
    {
        List<CreatureLogic> beeList = new List<CreatureLogic>(GameObject.FindObjectsOfType<CreatureLogic>());

        Vector3 bearing = new Vector3(0, 0, 0);

        foreach (CreatureLogic bee in beeList)
        {
            if (Vector3.Distance(idleBee.GetUIPosition().position, bee.GetUIPosition().position) < (unitRadius*4) + (moveSpeed * Time.deltaTime))
            {
                bearing += -1*(bee.GetUIPosition().position - idleBee.GetUIPosition().position);
            }
        }
        bearing = Vector3.Normalize(bearing);
        idleBee.transform.position += bearing * moveSpeed * Time.deltaTime;

        //avoid anyone idly

        /*search through all bees, for the ones in range of you, drift away from them. (you can use a pretty good radius for this as it 
        is specifically when the bee has nothing else to do)*/
    }

    private static Vector3 LocalAvoidance(Transform trans, float moveSpeed, Vector3 destination, float unitRadius)
    {
        //do a sphere cast towards a radius, and if 
        RaycastHit rayHit;

        int angleIncrement = 15;

        int angleTestRange = 150;

        int pathTests = 1 + angleTestRange / angleIncrement;
        

        Vector3 bearing = destination - trans.position;
        int index = 0;
        for (int i=0; i < pathTests; i++)
        {
            index += i * ((int)Mathf.Pow(-1, i));
            if (Physics.SphereCast(trans.position, unitRadius, Quaternion.Euler(0, (index) *angleIncrement, 0)*bearing, out rayHit, (moveSpeed * Time.deltaTime) + unitRadius))
            {
                if (rayHit.transform.gameObject.GetComponent<CreatureLogic>() == null)
                {
                    return Vector3.Normalize(Quaternion.Euler(0, (index) * angleIncrement, 0) * bearing);
                }
                if(rayHit.transform == trans)
                {
                    Debug.Log("I am avoiding myself like a dummy");
                }
            }
            else
            {
                return Vector3.Normalize(Quaternion.Euler(0, (index) * angleIncrement, 0) * bearing);
            }
        }

        return Vector3.zero;
    }

    public static void MoveTowards(Transform trans, float moveSpeed, Vector3 destination, float unitRadius)
    {
        Vector3 bearing = LocalAvoidance(trans, moveSpeed, destination, unitRadius);

        //has a bearing, shoot a raycast using the destination and your position, and use it to find your bearing


        trans.position += bearing * moveSpeed * Time.deltaTime;
        LookTowards(trans, bearing);
    }

    public static bool ReachedDestination(Transform trans, MoveCommand command, float distanceThreshold, float agentRadius)
    {
        return DistanceCheck(trans, command, distanceThreshold, agentRadius);
    }     
}
