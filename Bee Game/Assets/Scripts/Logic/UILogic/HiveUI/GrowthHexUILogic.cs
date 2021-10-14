using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrowthHexUILogic : MonoBehaviour
{
    public Image honeyProgressBar;
    public Image growthSymbol;
    private float growthStepTime = 0;
    private float progressPercentage = 0;

    public void UpdateGrowthHex(float growthTimeTotal, float currentTime)
    {
        SetHoneyProgress(growthTimeTotal, currentTime);
        SetGrowthSprite(growthTimeTotal, currentTime);
    }

    private void SetHoneyProgress(float growthTimeTotal, float currentTime)
    {
        progressPercentage = currentTime / growthTimeTotal;
        honeyProgressBar.fillAmount = progressPercentage;
        honeyProgressBar.color = ColourData.instance.honey;
    }

    private void SetGrowthSprite(float growthTimeTotal, float currentTime)
    {
        growthStepTime = growthTimeTotal / (float)ColourData.instance.beeGrowthStages.Count;
        growthSymbol.sprite = ColourData.instance.beeGrowthStages[(int)(currentTime/ growthStepTime)];
    }
}
