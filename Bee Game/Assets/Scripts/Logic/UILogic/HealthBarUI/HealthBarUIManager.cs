using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HealthBarUIManager : MonoBehaviour
{
    [SerializeField]
    private HealthBarUI healthBarTemplate;

    private List<HealthBarUI> healthBars = new List<HealthBarUI>();
    private List<SelectableLogic> eligbleUnits = new List<SelectableLogic>();

    private void Start()
    {
        healthBarTemplate.gameObject.SetActive(false);
    }

    private void UpdateEligbleUnits()
    {
        List<SelectableLogic> allUnits = new List<SelectableLogic>(FindObjectsOfType<SelectableLogic>());

        eligbleUnits.Clear();

        foreach(SelectableLogic unit in allUnits)
        {
            if(unit.isActiveAndEnabled && !unit.IsDead() && unit.IsDamaged())
            {
                eligbleUnits.Add(unit);
            }
        }
    }

    private void UpdateHealthBarDisplays()
    {
        HideHealthbars();
        int index = 0;
        foreach (SelectableLogic unit in eligbleUnits)
        {
            if(healthBars.Count == index)
            {
                CreateHealthbar();
            }
            

            healthBars[index].gameObject.SetActive(true);
            healthBars[index].SetObject(unit);
            index++;
        }
    }

    private void UpdateHealthBarPositions()
    {
        foreach (HealthBarUI healthBar in healthBars)
        {
            if (healthBar.isActiveAndEnabled)
                healthBar.UpdatePosition();
        }
    }

    private void CreateHealthbar()
    {
        HealthBarUI healthBar = Instantiate(healthBarTemplate) as HealthBarUI;

        healthBar.transform.SetParent(healthBarTemplate.transform.parent, false);

        healthBar.gameObject.SetActive(false);
        healthBars.Add(healthBar);
    }

    private void HideHealthbars()
    {
        foreach (HealthBarUI healthBar in healthBars)
        {
            healthBar.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateHealthBarPositions();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateEligbleUnits();
        UpdateHealthBarDisplays();
    }
}
