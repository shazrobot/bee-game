using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourData : MonoBehaviour
{
    public static ColourData instance;

    public Color activated;
    public Color inactivated;

    public List<Color> seasons;

    public Color summerTopGrass;
    public Color summerBottomGrass;
    public Color autumnTopGrass;
    public Color autumnBottomGrass;
    public Color winterTopGrass;
    public Color winterBottomGrass;
    public Color springTopGrass;
    public Color springBottomGrass;

    public Color honey;

    public Color friendly;
    public Color hostile;
    public Color neutral;

    public Material GrowthMaskMaterial;

    public List<Sprite> beeGrowthStages = new List<Sprite>();

    private void Awake()
    {
        instance = this;
    }
}
