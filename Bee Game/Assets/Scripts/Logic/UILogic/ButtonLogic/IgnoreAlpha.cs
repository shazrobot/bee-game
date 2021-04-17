using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IgnoreAlpha : MonoBehaviour
{
    private Image img;

    void Start()
    {
        img = this.GetComponent<Image>();
        img.alphaHitTestMinimumThreshold = 0.5f;
    }
}
