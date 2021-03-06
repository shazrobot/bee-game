using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType { Move, Gather, Attack, DropOffResources, AttackMove};

public struct MoveCommand
{
    public MoveType moveType;
    public Vector3 moveDestination;
    //A null objective suggests it is not a gather or attack command
    public GameObject objective;

    public MoveCommand(MoveType mT, Vector3 mD, GameObject obj = null)
    {
        moveType = mT;
        moveDestination = mD;
        objective = obj;
    }

    public bool IsObjectBased()
    {
        return (moveDestination == Vector3.zero);
    }

    public GameObject GetObjective()
    {
        return objective;
    }

    public Vector3 GetDestination()
    {
        if (moveType == MoveType.Move || moveType == MoveType.AttackMove)
        {
            if (objective != null)
            {
                if (objective.GetComponent<SelectableLogic>() != null)
                {
                    return objective.GetComponent<SelectableLogic>().GetUIPosition().position;
                }
                return objective.transform.position;
            }
            else
                return moveDestination;
        }
        else
        {
            if(objective.GetComponent<SelectableLogic>() != null)
            {
                return objective.GetComponent<SelectableLogic>().GetUIPosition().position;
            }
            return objective.transform.position;
        }
    }

}
