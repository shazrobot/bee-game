using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HiveUILogic : MonoBehaviour
{
    public static HiveUILogic instance;

    private HiveLogic hive;
    private Vector3 screenPos;

    public List<GrowthHexUILogic> growthHexes;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        HideGrowthHexes();
    }

    public void SelectHive(HiveLogic h = null)
    {
        hive = h;
        if(h != null)
        {
            PlaceUI();
        }        
    }

    private void PlaceUI()
    {
        if(hive != null && !hive.IsBuilding())
        {
            gameObject.SetActive(true);
            screenPos = Camera.main.WorldToScreenPoint(hive.GetUIPosition().position);
            transform.position = screenPos;
            UpdateGrowthHexes();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void UpdateGrowthHexes()
    {
        HideGrowthHexes();
        ShowGrowthHexes();
        if(hive.beeQueue > 0)
        {
            growthHexes[0].UpdateGrowthHex(hive.beeConstructionTime, Mathf.Min(hive.constructionTimer, hive.beeConstructionTime-.01f));
        }
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
        for(int i=0; i < hive.beeQueue; i++)
        {
            growthHexes[i].gameObject.SetActive(true);
            growthHexes[i].UpdateGrowthHex(hive.beeConstructionTime, 0);
        }
    }

    public void BeeBuildButton()
    {
        hive.QueueBeeBuild();
    }

    // Update is called once per frame
    void Update()
    {
        PlaceUI();
    }
}
