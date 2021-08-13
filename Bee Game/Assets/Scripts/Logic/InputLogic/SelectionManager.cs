using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;

    public RallyPoint rallyPoint;

    public RectTransform selectionRectangle;
    public RectTransform selectionRectangleOutline;
    private Vector3 selectionStartPosition;
    private Vector3 selectionCurrentPosition;
    private float distanceAllowance = 2f;

    public List<CreatureLogic> SelectedUnits = new List<CreatureLogic>();
    public HiveLogic SelectedHives = null;
    public List<PlantLogic> SelectedPlants = new List<PlantLogic>();

    private Plane rayDetectionPlane;
    private Ray ray;
    private RaycastHit rayHit;
    private float maxRayDistance = 1000.0f;

    private bool leftClickInitiated = false;
    private bool rightClickInitiated = false;

    private Vector3 heightAdjustStartPosition;
    private Vector3 heightAdjustStartMousePosition;
    private Vector3 heightAdjustCurrentPosition;
    private Vector3 heightAdjustCurrentMousePosition;
    private float heightAdjustDistanceAllowance = 2f;
    [SerializeField]
    private float defaultMoveCommandLevelHeight = 10f;


    // Start is called before the first frame update

    void Start()
    {
        instance = this;
        HideSelectionRectangle();
    }

    public void HandleMouseSelectionInput()
    {
        if (Input.GetMouseButtonDown(0) && !IsMouseOverUI())
        {
            selectionStartPosition = Input.mousePosition;
            leftClickInitiated = true;
        }
        if (Input.GetMouseButton(0) && leftClickInitiated)
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
        if (Input.GetMouseButtonUp(0) & leftClickInitiated)//Left Click
        {
            leftClickInitiated = false;
            HideSelectionRectangle();

            selectionCurrentPosition = Input.mousePosition;

            DeselectSelectedHives();
            DeselectSelectedPlants();
            if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                DeselectSelectedUnits();
            }

            if (Vector3.Distance(selectionCurrentPosition, selectionStartPosition) < distanceAllowance) // Click select
            {
                //individual select
                ray = Camera.main.ScreenPointToRay(selectionCurrentPosition);
                if (Physics.Raycast(ray, out rayHit, maxRayDistance))
                {
                    if (rayHit.collider.gameObject.GetComponent<CreatureLogic>() != null) //creature select
                    {
                        CreatureLogic creature = rayHit.collider.gameObject.GetComponent<CreatureLogic>();

                        SelectUnit(creature);
                    }

                    if (rayHit.collider.gameObject.GetComponent<HiveLogic>() != null) //Hive select
                    {
                        HiveLogic hive = rayHit.collider.gameObject.GetComponent<HiveLogic>();
                        DeselectSelectedUnits();
                        SelectHive(hive);
                    }

                    if (rayHit.collider.gameObject.GetComponent<PlantLogic>() != null) //Hive select
                    {
                        PlantLogic plant = rayHit.collider.gameObject.GetComponent<PlantLogic>();
                        DeselectSelectedUnits();
                        SelectPlant(plant);
                    }
                }

            }
            else // Drag select
            {
                List<CreatureLogic> selecteables = new List<CreatureLogic>(FindObjectsOfType<CreatureLogic>());
                Vector3 point = Vector3.zero;

                foreach (CreatureLogic creature in selecteables)
                {
                    float maxX = Mathf.Max(selectionCurrentPosition.x, selectionStartPosition.x);
                    float minX = Mathf.Min(selectionCurrentPosition.x, selectionStartPosition.x);
                    float maxY = Mathf.Max(selectionCurrentPosition.y, selectionStartPosition.y);
                    float minY = Mathf.Min(selectionCurrentPosition.y, selectionStartPosition.y);
                    point = Camera.main.WorldToScreenPoint(creature.transform.position);
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
        
        if (Input.GetMouseButtonDown(1) && !IsMouseOverUI() && HaveCreaturesSelected())
        {
            //Replaced the averaging of selected y values, to just a simple default height
            //rayDetectionPlane = new Plane(Vector3.up, new Vector3(0, GetAverageSelectedYPosition(), 0));
            rayDetectionPlane = new Plane(Vector3.up, new Vector3(0, defaultMoveCommandLevelHeight, 0));
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            heightAdjustStartMousePosition = Input.mousePosition;

            //check to see if it is clicking on a plant first, else do a move

            if (Physics.Raycast(ray, out rayHit, maxRayDistance))
            {
                if (rayHit.collider.gameObject.GetComponent<PlantLogic>() != null)
                {
                    PlantLogic plant = rayHit.collider.gameObject.GetComponent<PlantLogic>();

                    IssueGatherCommand(plant);
                }
                else if (rayHit.collider.gameObject.GetComponent<HiveLogic>() != null)
                {
                    HiveLogic hive = rayHit.collider.gameObject.GetComponent<HiveLogic>();
                    if (PlayerLogic.instance.factionLogic.IsEnemy(hive.GetFaction()))
                    {
                        IssueAttackCommand(hive);
                    }
                    else if(PlayerLogic.instance.factionLogic == hive.GetFaction())
                    {
                        IssueDeliverCommand(hive);
                    }                    
                }
                else if (rayHit.collider.gameObject.GetComponent<CreatureLogic>() != null)
                {
                    CreatureLogic bee = rayHit.collider.gameObject.GetComponent<CreatureLogic>();

                    if (PlayerLogic.instance.factionLogic.IsEnemy(bee.GetFaction()))
                    {
                        IssueAttackCommand(bee);
                    }
                    else
                    {
                        float collision;
                        if (rayDetectionPlane.Raycast(ray, out collision))
                        {
                            heightAdjustStartPosition = ray.GetPoint(collision);
                            rightClickInitiated = true;
                        }
                    }
                }
                else
                {
                    float collision;
                    if (rayDetectionPlane.Raycast(ray, out collision))
                    {
                        heightAdjustStartPosition = ray.GetPoint(collision);
                        rightClickInitiated = true;
                    }
                    
                }
            }
            else
            {
                float collision;
                if (rayDetectionPlane.Raycast(ray, out collision))
                {
                    heightAdjustStartPosition = ray.GetPoint(collision);
                    rightClickInitiated = true;
                }
            }
        }
        if (Input.GetMouseButton(1) && rightClickInitiated)
        {
            heightAdjustCurrentMousePosition = Input.mousePosition;
            heightAdjustCurrentPosition = heightAdjustStartPosition + (Vector3.up * (heightAdjustCurrentMousePosition.y-heightAdjustStartMousePosition.y)*0.5f);
            HeightAdjustmentUI.instance.AdjustHeightUI(heightAdjustCurrentPosition, heightAdjustStartPosition);
            //track changes in y in screenpos, and adjust heightUI
        }
        if (Input.GetMouseButtonUp(1) && rightClickInitiated)
        {
            IssueMoveCommand(heightAdjustCurrentPosition);
            HeightAdjustmentUI.instance.HideHeightUI();
            rightClickInitiated = false;
        }
    }



    //Private Helper functions

    private bool HaveCreaturesSelected()
    {
        return (SelectedUnits.Count > 0);
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private float GetAverageSelectedYPosition()
    {
        float average = 0;
        foreach(CreatureLogic creature in SelectedUnits)
        {
            average += creature.transform.position.y;
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
                creature.EnqueueGoal(new MoveCommand(MoveType.Move, hitPoint));
            }
        }
        else
        {
            foreach (CreatureLogic creature in SelectedUnits)
            {
                creature.ResetCommandsToThis(new MoveCommand(MoveType.Move, hitPoint));
            }
        }
        DisplaySelectedRallyPoints();
    }

    private void IssueGatherCommand(PlantLogic plant)
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            foreach (CreatureLogic creature in SelectedUnits)
            {
                creature.EnqueueGoal(new MoveCommand(MoveType.Gather, Vector3.zero, plant.gameObject));
            }
        }
        else
        {
            foreach (CreatureLogic creature in SelectedUnits)
            {
                creature.ResetCommandsToThis(new MoveCommand(MoveType.Gather, Vector3.zero, plant.gameObject));
            }
        }
        DisplaySelectedRallyPoints();
    }

    private void IssueDeliverCommand(HiveLogic hive)
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            foreach (CreatureLogic creature in SelectedUnits)
            {
                creature.EnqueueGoal(new MoveCommand(MoveType.DropOffResources, Vector3.zero, hive.gameObject));
            }
        }
        else
        {
            foreach (CreatureLogic creature in SelectedUnits)
            {
                creature.ResetCommandsToThis(new MoveCommand(MoveType.DropOffResources, Vector3.zero, hive.gameObject));
            }
        }
        DisplaySelectedRallyPoints();
    }

    private void IssueAttackCommand(SelectableLogic selectable)
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            foreach (CreatureLogic creature in SelectedUnits)
            {
                creature.EnqueueGoal(new MoveCommand(MoveType.Attack, Vector3.zero, selectable.gameObject));
            }
        }
        else
        {
            foreach (CreatureLogic creature in SelectedUnits)
            {
                creature.ResetCommandsToThis(new MoveCommand(MoveType.Attack, Vector3.zero, selectable.gameObject));
            }
        }
        DisplaySelectedRallyPoints();
    }

    private void DisplaySelectedRallyPoints()
    {
        RallyPointManager.instance.GenerateRallyPoints(SelectedUnits);
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
        if (!creature.IsSelected())
        {
            creature.Select();
            SelectedUnits.Add(creature);
            DisplaySelectedRallyPoints();
            SelectionSpriteLogic.instance.AddSelectedObject(creature);
            return true;
        }
        return false;
    }

    private void DeselectAll()
    {
        DeselectSelectedHives();
        DeselectSelectedPlants();
        DeselectSelectedUnits();
    }

    private void DeselectSelectedUnits()
    {
        foreach (CreatureLogic creature in SelectedUnits)
        {
            creature.Deselect();
            SelectionSpriteLogic.instance.RemoveSelectedObject(creature);
        }
        SelectedUnits.Clear();
        DisplaySelectedRallyPoints();
        //SelectionSpriteLogic.instance.SetSelectedObjects(new List<SelectableLogic>(SelectedUnits));
    }

    private bool SelectHive(HiveLogic hive)
    {
        if (!hive.IsSelected())
        {
            hive.Select();
            SelectedHives = hive;
            DeselectSelectedUnits();
            HiveUILogic.instance.SelectHive(SelectedHives);
            SelectionSpriteLogic.instance.AddSelectedObject(hive);
            return true;
        }
        return false;
    }

    private void DeselectSelectedHives()
    {
        if(SelectedHives != null)
        {
            SelectionSpriteLogic.instance.RemoveSelectedObject(SelectedHives);
            SelectedHives.Deselect();
            SelectedHives = null;
        }
        HiveUILogic.instance.SelectHive();
    }

    private bool SelectPlant(PlantLogic plant)
    {
        if (!plant.IsSelected())
        {
            SelectedUILogic.instance.Select(plant);
            plant.Select();
            SelectedPlants.Add(plant);
            SelectionSpriteLogic.instance.AddSelectedObject(plant);
            return true;
        }
        return false;
    }

    private void DeselectSelectedPlants()
    {
        foreach (PlantLogic plant in SelectedPlants)
        {
            plant.Deselect();
            SelectionSpriteLogic.instance.RemoveSelectedObject(plant);
        }
        SelectedUILogic.instance.Deselect();
        SelectedPlants.Clear();
        //SelectionSpriteLogic.instance.SetSelectedObjects(new List<SelectableLogic>(SelectedPlants));
    }

    public void ExitSelectionMode()
    {
        DeselectAll();
    }
}
