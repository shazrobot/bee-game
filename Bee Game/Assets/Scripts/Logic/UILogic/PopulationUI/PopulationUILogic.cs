using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopulationUILogic : FactionObserver
{
    public TextMeshProUGUI populationAmount;

    private void Start()
    {
        PlayerLogic.instance.SubscribeToPlayersFactionObserver(this);
    }

    public override void FactionUpdated(FactionLogic faction)
    {
        populationAmount.text = string.Format("{0}/{1}", faction.GetBeeAmount(), faction.GetBeeCap());
    }
}
