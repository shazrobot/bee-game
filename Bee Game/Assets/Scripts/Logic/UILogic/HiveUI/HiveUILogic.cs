using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HiveUILogic : MonoBehaviour
{
    public static HiveUILogic instance;

    public Image hiveUI;

    private HiveLogic hive;
    private Vector3 screenPos;

    public List<GrowthHexUILogic> growthHexes;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
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
        if(hive != null)
        {
            hiveUI.gameObject.SetActive(true);
            screenPos = Camera.main.WorldToScreenPoint(hive.transform.position);
            hiveUI.rectTransform.position = screenPos;
            UpdateGrowthHexes();
        }
        else
        {
            hiveUI.gameObject.SetActive(false);
        }
    }

    private void UpdateGrowthHexes()
    {
        HideGrowthHexes();
        ShowGrowthHexes();
        if(hive.beeQueue > 0)
        {
            growthHexes[0].UpdateGrowthHex(hive.beeConstructionTime, hive.constructionTimer);
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
