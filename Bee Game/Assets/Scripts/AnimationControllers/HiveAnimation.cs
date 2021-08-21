using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveAnimation : MonoBehaviour
{
    private HiveLogic hiveLogic;
    [SerializeField]
    private List<GameObject> combs;

    private void Awake()
    {
        hiveLogic = gameObject.GetComponent<HiveLogic>();
    }

    public void ColourCombs(Material material)
    {
        foreach(GameObject comb in combs)
        {
            comb.GetComponent<MeshRenderer>().material = material;
        }
    }

    public void HideCombs()
    {
        foreach (GameObject comb in combs)
        {
            comb.SetActive(false);
        }
    }

    public void ShowCombs()
    {
        foreach (GameObject comb in combs)
        {
            comb.SetActive(true);
        }
    }

    public void GrowHive(float progressDecimal)
    {
        float scale = Mathf.Min(progressDecimal, 1);
        int displayAmount = 1 + Mathf.FloorToInt(((combs.Count-1) * scale));

        for(int i=0; i< displayAmount; i++)
        {
            combs[i].SetActive(true);
        }
    }
}
