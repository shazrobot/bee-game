using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType { Move, Gather, Attack, DropOffResources};

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

    public Vector3 GetDestination()
    {
        if (moveType == MoveType.Move)
        {
            return moveDestination;
        }
        else
        {
            return objective.transform.position;
        }
    }

}
