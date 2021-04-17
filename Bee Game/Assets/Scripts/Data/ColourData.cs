using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourData : MonoBehaviour
{
    public static ColourData instance;

    public Color activated;
    public Color inactivated;

    public List<Color> seasons;

    private void Awake()
    {
        instance = this;
    }
}
