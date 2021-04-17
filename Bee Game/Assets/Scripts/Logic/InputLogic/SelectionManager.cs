using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;

    public RectTransform selectionRectangle;
    public RectTransform selectionRectangleOutline;
    private Vector3 selectionStartPosition;
    private Vector3 selectionCurrentPosition;
    private float distanceAllowance = 20f;

    public List<CreatureLogic> SelectedUnits = new List<CreatureLogic>();

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
    }
}
