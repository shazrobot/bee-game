using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantAnimations : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> flowers;

    public void HideFlowers()
    {
        foreach(GameObject flower in flowers)
        {
            flower.SetActive(false);
        }
    }

    public void ShowFlowers()
    {
        foreach (GameObject flower in flowers)
        {
            flower.SetActive(true);
        }
    }
}
