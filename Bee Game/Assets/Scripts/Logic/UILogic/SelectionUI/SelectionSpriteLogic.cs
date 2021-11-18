using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSpriteLogic : MonoBehaviour
{
    public static SelectionSpriteLogic instance;
    [SerializeField]
    private SpriteRenderer template;
    private List<SpriteRenderer> selectionSprites = new List<SpriteRenderer>();
    private List<FriendlinessType> friendlinessTypes = new List<FriendlinessType>();
    private List<SelectableLogic> selectedObjects = new List<SelectableLogic>();

    private void Start()
    {
        template.gameObject.SetActive(false);
        instance = this;
    }

    private void HideSprites()
    {
        foreach (SpriteRenderer spr in selectionSprites)
        {
            spr.gameObject.SetActive(false);
        }
    }

    private void UpdateFriendlinessList()
    {
        friendlinessTypes.Clear();
        foreach (SelectableLogic selected in selectedObjects)
        {
            friendlinessTypes.Add(PlayerLogic.instance.factionLogic.GetFriendlinessOfSelectable(selected));
        }
    }

    private void GenerateSprite()
    {
        SpriteRenderer newSprite = Instantiate(template) as SpriteRenderer;
        selectionSprites.Add(newSprite);
        newSprite.transform.SetParent(template.transform.parent);
        newSprite.gameObject.SetActive(true);
        //make new sprite and add it to selectionSprites
    }

    private void UpdateSpriteLocations()
    {
        int i = 0;
        foreach(SelectableLogic selected in selectedObjects)
        {
            if (i >= selectionSprites.Count)
                GenerateSprite();
            selectionSprites[i].gameObject.SetActive(true);
            selectionSprites[i].transform.position = selected.GetUIPosition().position;
            selectionSprites[i].transform.localScale = selected.GetUIScale()*Vector3.one;
            if (friendlinessTypes[i] == FriendlinessType.Friendly)
                selectionSprites[i].color = ColourData.instance.friendly;
            else if (friendlinessTypes[i] == FriendlinessType.Hostile)
                selectionSprites[i].color = ColourData.instance.hostile;
            else
                selectionSprites[i].color = ColourData.instance.neutral;
            i++;
        }
    }

    public void RemoveSelectedObject(SelectableLogic obj)
    {
        selectedObjects.Remove(obj);
        HideSprites();
        UpdateSpriteLocations();
    }

    public void AddSelectedObject(SelectableLogic obj)
    {
        if (!selectedObjects.Contains(obj))
        {
            selectedObjects.Add(obj);
            UpdateFriendlinessList();
            UpdateSpriteLocations();
        }        
    }

    public void SetSelectedObjects(List<SelectableLogic> obj)
    {
        selectedObjects = obj;
        UpdateFriendlinessList();
        HideSprites();
        UpdateSpriteLocations();
    }

    private void Update()
    {
        UpdateSpriteLocations();
    }
}
