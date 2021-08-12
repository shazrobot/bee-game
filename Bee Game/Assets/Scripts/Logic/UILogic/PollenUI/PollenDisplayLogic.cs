using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PollenDisplayLogic : FactionObserver
{
    public TextMeshProUGUI pollenAmount;

    private void Start()
    {
        PlayerLogic.instance.SubscribeToPlayersFactionObserver(this);
    }

    public override void FactionUpdated(FactionLogic faction)
    {
        pollenAmount.text = string.Format("{0} Gr", faction.pollenAmount);
    }
}
