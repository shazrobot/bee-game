using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedUILogic : MonoBehaviour
{
    [NonSerialized]
    public static SelectedUILogic instance;

    [SerializeField]
    private GameObject basePanel;

    [SerializeField]
    private Image healthPanel;

    [SerializeField]
    private TextMeshProUGUI selectedName;

    [SerializeField]
    private GameObject rowOne;
    [SerializeField]
    private TextMeshProUGUI rowOneInfo;

    [SerializeField]
    private GameObject rowTwo;
    [SerializeField]
    private TextMeshProUGUI rowTwoInfo;


    [SerializeField]
    private GameObject hivePanel;
    [SerializeField]
    private List<GrowthHexUILogic> growthHexes;

    private HiveLogic selectedHive = null;

    [SerializeField]
    private Button button1;
    [SerializeField]
    private Button button2;

    private void Start()
    {
        instance = this;
        Deselect();
    }

    private void PlantDisplay(PlantLogic plant)
    {
        switch (plant.GetFlowerType())
        {
            case FlowerType.Fertility:
                selectedName.text = "Fertility Flower";
                break;
            case FlowerType.Health:
                selectedName.text = "Health Flower";
                break;
            case FlowerType.Lethality:
                selectedName.text = "Lethality Flower";
                break;
        }
        rowOne.SetActive(true);
        rowOneInfo.text = string.Format("{0} Gr        {1} Gatherers", plant.pollenAmount, plant.GetGathererAllocationSize());
        rowTwo.SetActive(true);
        rowTwoInfo.text = string.Format("{0}", (plant.IsPollinated()?"Pollinated":"Not Pollinated"));
        hivePanel.SetActive(false);
        HideButtons();
    }

    private void BeeDisplay(CreatureLogic bee)
    {
        selectedName.text = "Bee";
        rowOne.SetActive(true);
        rowOneInfo.text = string.Format("{0} Gr", bee.GetPollenAmount());
        rowTwo.SetActive(false);
        //rowTwoInfo.text = string.Format("{0}", (plant.IsPollinated() ? "Pollinated" : "Not Pollinated"));
        hivePanel.SetActive(false);
        if (PlayerLogic.instance.factionLogic.IsMyBee(bee))
            ShowButtons();
        else
            HideButtons();
    }

    private void HiveDisplay(HiveLogic hive)
    {
        selectedName.text = "Hive";
        rowOne.SetActive(false);
        //rowOneInfo.text = string.Format("{0} Gr", bee.GetPollenAmount());
        rowTwo.SetActive(false);
        //rowTwoInfo.text = string.Format("{0}", (plant.IsPollinated() ? "Pollinated" : "Not Pollinated"));
        if (PlayerLogic.instance.factionLogic.IsMyHive(hive))
            hivePanel.SetActive(true);
        else
            hivePanel.SetActive(false);
        selectedHive = hive;
        HideGrowthHexes();
        
        ShowGrowthHexes();
        HideButtons();
    }

    private void HideButtons()
    {
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
    }

    private void ShowButtons()
    {
        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(true);
    }

    private void HideGrowthHexes()
    {
        foreach (GrowthHexUILogic growthHex in growthHexes)
        {
            growthHex.gameObject.SetActive(false);
        }
    }

    private void ShowGrowthHexes()
    {
        for (int i = 0; i < selectedHive.beeQueue; i++)
        {
            growthHexes[i].gameObject.SetActive(true);
            growthHexes[i].UpdateGrowthHex(selectedHive.beeConstructionTime, 0);
        }
    }

    public void BeeBuildButton()
    {
        selectedHive.QueueBeeBuild();
    }

    private void UpdateGrowthHexes()
    {
        HideGrowthHexes();
        ShowGrowthHexes();
        if (selectedHive.beeQueue > 0)
        {
            growthHexes[0].UpdateGrowthHex(selectedHive.beeConstructionTime, Mathf.Min(selectedHive.constructionTimer, selectedHive.beeConstructionTime - .01f));
        }
    }

    public void Select(SelectableLogic selectable)
    {
        if (selectable.GetComponent<PlantLogic>() != null)
        {
            PlantLogic plant = selectable.GetComponent<PlantLogic>();
            PlantDisplay(plant);
        }
        if (selectable.GetComponent<CreatureLogic>() != null)
        {
            CreatureLogic bee = selectable.GetComponent<CreatureLogic>();
            BeeDisplay(bee);
        }
        if (selectable.GetComponent<HiveLogic>() != null)
        {
            HiveLogic hive = selectable.GetComponent<HiveLogic>();
            HiveDisplay(hive);
        }
        healthPanel.fillAmount = selectable.GetHealthRatio();
        basePanel.SetActive(true);

        switch (PlayerLogic.instance.factionLogic.GetFriendlinessOfSelectable(selectable))
        {
            case FriendlinessType.Friendly:
                healthPanel.color = ColourData.instance.friendly;
                break;
            case FriendlinessType.Hostile:
                healthPanel.color = ColourData.instance.hostile;
                break;
            case FriendlinessType.Neutral:
                healthPanel.color = ColourData.instance.neutral;
                break;
        }
    }

    private void FixedUpdate()
    {
        if (selectedHive != null)
        {
            UpdateGrowthHexes();
        }
    }

    public void Deselect()
    {
        rowOne.SetActive(false);
        rowTwo.SetActive(false);
        hivePanel.SetActive(false);
        basePanel.SetActive(false);
        selectedHive = null;
    }
}
