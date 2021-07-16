using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


//contains a green sprite, will reproduce if not near other grassNodes
public class GrassNode : MonoBehaviour
{
    //how far it'll make grass nodes from its location
    [SerializeField]
    private float reproductionDistance;
    //how far it deems a node to be too close
    [SerializeField]
    private float chokeDistance;
    //the amount of nodes which counts as being choked
    [SerializeField]
    private int chokeCount;
    [SerializeField]
    private bool choked = false;


    //Good to replace this with a map static class which has map data
    [SerializeField]
    private float xMax = 200;
    [SerializeField]
    private float xMin = -200;
    [SerializeField]
    private float zMax = 200;
    [SerializeField]
    private float zMin = -200;



    public Vector3 Reproduce(List<GrassNode> grasses)
    {
        if (!choked)
        {
            if (!IsChokedTest(grasses))
            {
                int xdirection = (Random.Range(-1, 2) > 0) ? -1 : 1;
                float xpos = xdirection*Random.Range(chokeDistance, reproductionDistance);
                int zdirection = (Random.Range(-1, 2) > 0) ? -1 : 1;
                float zpos = zdirection * Random.Range(chokeDistance, reproductionDistance);
                return new Vector3(transform.position.x+xpos, transform.position.y, transform.position.z+zpos);
            }
            else
            {
                choked = true;
                //GetComponent<SpriteRenderer>().color = Color.black;
                return (Vector3.zero);
            }
        }
        return (Vector3.zero);
    }

    public bool IsChoked()
    {
        return choked;
    }

    private bool OutOfBounds()
    {
        return ((transform.position.x < xMax) &&
            (transform.position.x > xMin) &&
            (transform.position.z < zMax) &&
            (transform.position.z > zMin));
    }

    private bool IsChokedTest(List<GrassNode> grasses)
    {
        if (!OutOfBounds())
            return true;

        int chokeCounter = 0;
        foreach (GrassNode grass in grasses)
        {
            if (Vector3.Distance(grass.transform.position, transform.position) < chokeDistance)
            {
                chokeCounter += 1;
                if (chokeCounter >= chokeCount)
                    return true;
            }            
        }
        return false;
    }
}
