using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUIManager : MonoBehaviour
{
    public static TimeUIManager instance;

    public List<Image> speedIndicators;

    public Button pausePlayButton;
    public Sprite playSprite;
    public Sprite pauseSprite;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        UpdateSpeedIndicators();
        SetPausePlayButtonSprite();
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
}
