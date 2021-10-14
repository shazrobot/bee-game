using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RallyPointManager : MonoBehaviour
{
    public static RallyPointManager instance;
    public RallyPoint template;

    private List<RallyPoint> rallyPoints = new List<RallyPoint>();
    private List<CreatureLogic> creatureList = new List<CreatureLogic>();

    public AnimationCurve curve = new AnimationCurve();

    public LineRenderer line;

    public void Awake()
    {
        instance = this;
        template.gameObject.SetActive(false);
    }

    //reset rallypoints
    public void DeleteRallyPoints()
    {
        foreach (RallyPoint rallyPoint in rallyPoints)
        {
            rallyPoint.DestroyChildren();
            Destroy(rallyPoint.gameObject);
        }
        rallyPoints.Clear();
    }

    private RallyPoint AddNewRallyPoint(Vector3 point)
    {
        RallyPoint rallyPoint = Instantiate(template) as RallyPoint;

        rallyPoint.gameObject.SetActive(true);
        rallyPoint.SetRallyPoint(point);

        rallyPoint.transform.SetParent(template.transform.parent, false);

        rallyPoints.Add(rallyPoint);
        return rallyPoint;
    }

    private void RedrawRallyPoints(Vector3 newPoint)
    {
        DeleteRallyPoints();
        foreach (CreatureLogic creature in creatureList)
        {
            foreach (MoveCommand command in creature.moveCommands)
            {
                RallyPoint rallyPoint = AddNewRallyPoint(command.GetDestination());
                if (command.GetDestination() == newPoint)
                    rallyPoint.AnimateOuterBounce();
            }
        }
    }

    //add a bunch of rally points based on vector3 list
    public void GenerateRallyPoints(List<CreatureLogic> creatures, Vector3 newPoint)
    {
        RedrawRallyPoints(newPoint);
        creatureList = creatures;
    }

    public void CreatureFinishedMoveCommand(CreatureLogic creature)
    {
        if (creatureList.Contains(creature))
        {
            RedrawRallyPoints(Vector3.zero);
        }
    }
}
