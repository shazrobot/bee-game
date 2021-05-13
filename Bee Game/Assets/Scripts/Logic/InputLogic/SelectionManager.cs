using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;

    public RallyPoint rallyPoint;

    public RectTransform selectionRectangle;
    public RectTransform selectionRectangleOutline;
    private Vector3 selectionStartPosition;
    private Vector3 selectionCurrentPosition;
    private float distanceAllowance = 20f;

    public List<CreatureLogic> SelectedUnits = new List<CreatureLogic>();

    private Plane rayDetectionPlane;
    private Ray ray;
    private RaycastHit rayHit;
    private float maxRayDistance = 100.0f;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        HideSelectionRectangle();
    }

    public void HandleMouseSelectionInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectionStartPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            selectionCurrentPosition = Input.mousePosition;

            if (Vector3.Distance(selectionCurrentPosition, selectionStartPosition) > distanceAllowance)
            {
                DrawSelectionRectangle(selectionCurrentPosition - selectionStartPosition);
            }
            else
            {
                HideSelectionRectangle();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            HideSelectionRectangle();

            selectionCurrentPosition = Input.mousePosition;


            if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                DeselectSelectedUnits();
            }

            if (Vector3.Distance(selectionCurrentPosition, selectionStartPosition) < distanceAllowance)
            {
                //individual select
                ray = Camera.main.ScreenPointToRay(selectionCurrentPosition);
                if (Physics.Raycast(ray, out rayHit, maxRayDistance))
                {
                    if (rayHit.collider.gameObject.GetComponent<CreatureLogic>() != null)
                    {
                        CreatureLogic creature = rayHit.collider.gameObject.GetComponent<CreatureLogic>();

                        SelectUnit(creature);
                    }
                }

            }
            else
            {
                List<CreatureLogic> selecteables = new List<CreatureLogic>(GameObject.FindObjectsOfType<CreatureLogic>());
                Vector3 point = Vector3.zero;

                foreach (CreatureLogic creature in selecteables)
                {
                    float maxX = Mathf.Max(selectionCurrentPosition.x, selectionStartPosition.x);
                    float minX = Mathf.Min(selectionCurrentPosition.x, selectionStartPosition.x);
                    float maxY = Mathf.Max(selectionCurrentPosition.y, selectionStartPosition.y);
                    float minY = Mathf.Min(selectionCurrentPosition.y, selectionStartPosition.y);
                    point = Camera.main.WorldToScreenPoint(creature.movementLogic.data.position);
                    if ((point.x > minX && point.x < maxX) && (point.y > minY && point.y < maxY))
                    {
                        SelectUnit(creature);
                    }
                }
            }
        }
    }

    public void HandleIssueCommandsRightClick()
    {
        
        if (Input.GetMouseButtonDown(1))
        {
            rayDetectionPlane = new Plane(Vector3.up, new Vector3(0, GetAverageSelectedYPosition(), 0));
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //check to see if it is clicking on a plant first, else do a move

            if (Physics.Raycast(ray, out rayHit, maxRayDistance))
            {
                if (rayHit.collider.gameObject.GetComponent<PlantLogic>() != null)
                {
                    PlantLogic plant = rayHit.collider.gameObject.GetComponent<PlantLogic>();

                    IssueCollectionCommand(plant);
                }
                else
                {
                    float collision;
                    if (rayDetectionPlane.Raycast(ray, out collision))
                    {
                        Vector3 hitPoint = ray.GetPoint(collision);
                        IssueMoveCommand(hitPoint);
                    }
                }
            }
            else
            {
                float collision;
                if (rayDetectionPlane.Raycast(ray, out collision))
                {
                    Vector3 hitPoint = ray.GetPoint(collision);
                    IssueMoveCommand(hitPoint);
                }
            }
        }
    }

    private float GetAverageSelectedYPosition()
    {
        float average = 0;
        foreach(CreatureLogic creature in SelectedUnits)
        {
            average += creature.movementLogic.data.position.y;
        }
        if(SelectedUnits.Count > 0)
        {
            average = average / (SelectedUnits.Count);
        }
        return average;
    }

    private void IssueMoveCommand(Vector3 hitPoint)
    {
        
        if(Input.GetKey(KeyCode.LeftShift)|| Input.GetKey(KeyCode.RightShift))
        {
            foreach (CreatureLogic creature in SelectedUnits)
            {
                creature.movementLogic.EnqueueGoal(hitPoint);
            }
        }
        else
        {
            foreach (CreatureLogic creature in SelectedUnits)
            {
                creature.movementLogic.ResetGoalToThis(hitPoint);
            }
        }
        DisplaySelectedRallyPoints();
    }

    private void IssueCollectionCommand(PlantLogic plant)
    {
        //tell selected units to collect pollen from plant
        Debug.Log("gathering");
    }

    private void DisplaySelectedRallyPoints()
    {
        RallyPointManager.instance.GenerateRallyPoints(GetSelectedMovementPoints());
    }

    private List<Vector3> GetSelectedMovementPoints()
    {
        List<Vector3> pointList = new List<Vector3>();
        foreach (CreatureLogic creature in SelectedUnits)
        {
            pointList.AddRange(creature.movementLogic.data.movePathGoals);
        }
        return pointList;
    }

    private void DrawSelectionRectangle(Vector3 area)
    {
        selectionRectangle.gameObject.SetActive(true);
        selectionRectangleOutline.gameObject.SetActive(true);
        selectionRectangle.position = selectionStartPosition;
        selectionRectangle.localScale = area;
        selectionRectangleOutline.position = new Vector3(Mathf.Min(selectionCurrentPosition.x, selectionStartPosition.x), Mathf.Min(selectionCurrentPosition.y, selectionStartPosition.y), 0);
        selectionRectangleOutline.sizeDelta = new Vector2(Mathf.Abs(area.x), Mathf.Abs(area.y));
    }

    private void HideSelectionRectangle()
    {
        selectionRectangle.gameObject.SetActive(false);
        selectionRectangleOutline.gameObject.SetActive(false);
    }

    private bool SelectUnit(CreatureLogic creature)
    {
        if (!creature.selected)
        {
            creature.Select();
            SelectedUnits.Add(creature);
            RallyPointManager.instance.GenerateRallyPoints(GetSelectedMovementPoints());
            return true;
        }
        return false;
    }

    private void DeselectSelectedUnits()
    {
        foreach (CreatureLogic creature in SelectedUnits)
        {
            creature.Deselect();
        }
        SelectedUnits.Clear();
        RallyPointManager.instance.GenerateRallyPoints(GetSelectedMovementPoints());
    }
}
