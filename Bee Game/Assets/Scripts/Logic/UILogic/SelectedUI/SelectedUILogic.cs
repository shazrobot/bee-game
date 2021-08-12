using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectedUILogic : MonoBehaviour
{
    [NonSerialized]
    public static SelectedUILogic instance;

    [SerializeField]
    private GameObject basePanel;

    [SerializeField]
    private GameObject rowOne;
    [SerializeField]
    private TextMeshProUGUI rowOneTitle;
    [SerializeField]
    private TextMeshProUGUI rowOneInfo;

    [SerializeField]
    private GameObject rowTwo;
    [SerializeField]
    private TextMeshProUGUI rowTwoTitle;
    [SerializeField]
    private TextMeshProUGUI rowTwoInfo;

    private void Start()
    {
        instance = this;
        Deselect();
    }

    private void PlantDisplay(PlantLogic plant)
    {
        rowOne.SetActive(true);
        rowOneTitle.text = string.Format("Pollen:");
        rowOneInfo.text = string.Format("{0} Gr", plant.pollenAmount);
        rowTwo.SetActive(true);
        rowTwoTitle.text = string.Format("Reproduction:");
        rowTwoInfo.text = string.Format("{0}", (plant.IsPollinated()?"Pollinated":"Not Pollinated"));
    }

    public void Select(SelectableLogic selectable)
    {
        if (selectable.GetComponent<PlantLogic>() != null)
        {
            PlantLogic plant = selectable.GetComponent<PlantLogic>();
            PlantDisplay(plant);
        }
        basePanel.SetActive(true);
    }

    public void Deselect()
    {
        rowOne.SetActive(false);
        rowTwo.SetActive(false);
        basePanel.SetActive(false);
    }
}
