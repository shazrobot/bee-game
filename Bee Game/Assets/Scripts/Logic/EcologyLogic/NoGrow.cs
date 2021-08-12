using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoGrow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EcosystemLogic.instance.noGrowObjects.Add(this);
    }

    //needs a mesh renderer and also needs a collider
    private void DuplicateSelf()
    {
        //makes a red material
        GameObject mask = (Instantiate(this) as NoGrow).gameObject;
        //mask.layer = LayerMask.NameToLayer("GrassRipples");
        mask.GetComponent<MeshRenderer>().material = ColourData.instance.GrowthMaskMaterial;


        EcosystemLogic.instance.noGrowObjects.Add(this);
    }
}
