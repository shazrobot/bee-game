using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PollenDisplayLogic : PlayerObserver
{
    public TextMeshProUGUI pollenAmount;

    public override void PlayerUpdated()
    {
        pollenAmount.text = string.Format("{0} Gr", PlayerLogic.instance.pollenAmount);
    }
}
