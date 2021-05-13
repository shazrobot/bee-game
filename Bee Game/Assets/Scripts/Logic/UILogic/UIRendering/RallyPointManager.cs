using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RallyPointManager : MonoBehaviour
{
    public static RallyPointManager instance;
    public RallyPoint template;

    private List<RallyPoint> rallyPoints = new List<RallyPoint>();

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

    private void AddNewRallyPoint(Vector3 point)
    {
        RallyPoint rallyPoint = Instantiate(template) as RallyPoint;

        rallyPoint.gameObject.SetActive(true);
        rallyPoint.SetRallyPoint(point);

        rallyPoint.transform.SetParent(template.transform.parent, false);

        rallyPoints.Add(rallyPoint);
    }

    //add a bunch of rally points based on vector3 list
    public void GenerateRallyPoints(List<Vector3> pointArray)
    {
        line.positionCount = pointArray.Count;
        line.SetPositions(pointArray.ToArray());
        DeleteRallyPoints();
        foreach (Vector3 point in pointArray)
        {
            AddNewRallyPoint(point);
        }
    }
}
