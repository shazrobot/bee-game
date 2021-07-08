using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeWings : MonoBehaviour
{

    public GameObject theBee;

    private void Awake()
    {
        theBee = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        theBee.GetComponent<Animator>().Play("Armature|ArmatureAction");
    }
}
