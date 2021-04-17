using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUIManager : MonoBehaviour
{
    public static TimeUIManager instance;

    public Image seasonBlock;
    public Image seasonBlockBlur;

    public Image radialHand;
    public List<Image> speedIndicators;

    public Button pausePlayButton;
    public Sprite playSprite;
    public Sprite pauseSprite;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        MonthProgress();
        DayProgress();
        UpdateSpeedIndicators();
        SetPausePlayButtonSprite();
    }

    private void MonthProgress()
    {
        int season = TimeManager.instance.GetSeason();
        seasonBlock.color = ColourData.instance.seasons[season];
        seasonBlockBlur.color = ColourData.instance.seasons[season];
        seasonBlock.transform.rotation = Quaternion.Euler(0, 0, season * -90);
        seasonBlockBlur.transform.rotation = Quaternion.Euler(0,0, season*-90);
    }

    private void DayProgress()
    {
        radialHand.transform.rotation = Quaternion.Euler(0, 0, TimeManager.instance.GetYearProgress()*-360);
    }

    private void UpdateSpeedIndicators()
    {
        foreach(Image speedIndicator in speedIndicators)
        {
            speedIndicator.color = ColourData.instance.inactivated;
        }

        int num = TimeManager.instance.GetTimeScale();

        for(int i = 0; i < num; i++)
        {
            speedIndicators[i].color = ColourData.instance.activated;
        }
    }

    private void SetPausePlayButtonSprite()
    {
        if (TimeManager.instance.timeScaleInt == 0)
        {
            pausePlayButton.image.sprite = playSprite;
        }
        else
        {
            pausePlayButton.image.sprite = pauseSprite;
        }
    }

    public void IncrementButton()
    {
        TimeManager.instance.IncrementTimeScale();
        UpdateSpeedIndicators();
        SetPausePlayButtonSprite();
    }

    public void DecrementButton()
    {
        TimeManager.instance.DecrementTimeScale();
        UpdateSpeedIndicators();
        SetPausePlayButtonSprite();
    }

    public void PausePlayToggle()
    {
        if (TimeManager.instance.timeScaleInt == 0)
        {
            TimeManager.instance.SetNormalTimeScale();
        }
        else
        {
            TimeManager.instance.ZeroTimeScale();
        }
        UpdateSpeedIndicators();
        SetPausePlayButtonSprite();
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeManager.instance.dayProgressed)
        {
            DayProgress();
        }
        if (TimeManager.instance.monthProgressed)
        {
            MonthProgress();
        }
    }
}
