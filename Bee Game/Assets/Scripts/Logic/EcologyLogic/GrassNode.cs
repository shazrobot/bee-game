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


    private float localDistance = 20f;
    private Vector3 localMeanPos = new Vector3();

    public Vector3 Reproduce(List<GrassNode> grasses)
    {
        if (!choked)
        {
            if (!IsChokedTest(grasses))
            {
                //int xdirection = (Random.Range(-1, 2) > 0) ? -1 : 1;
                //float xpos = xdirection*Random.Range(chokeDistance, reproductionDistance);
                //int zdirection = (Random.Range(-1, 2) > 0) ? -1 : 1;
                //float zpos = zdirection * Random.Range(chokeDistance, reproductionDistance);

                int xdirection = (Random.Range(-1, 2) > 0) ? -1 : 1;
                float xpos = xdirection*Random.Range(chokeDistance, reproductionDistance);
                int zdirection = (Random.Range(-1, 2) > 0) ? -1 : 1;
                float zpos = zdirection * Random.Range(chokeDistance, reproductionDistance);

                //find local position, then create a vector using from local mean towards your position, normalise, then 10x away
                //add some randomness;

                localMeanPos = FindLocalMeanPos(grasses);

                Vector3 bearing = reproductionDistance * Vector3.Normalize(transform.position-localMeanPos);

                bearing = new Vector3(bearing.x+ (xpos/4f), bearing.y, bearing.z+ (zpos/4f));

                //return new Vector3(transform.position.x+xpos, transform.position.y, transform.position.z+zpos);
                return new Vector3(transform.position.x + bearing.x, transform.position.y, transform.position.z + bearing.z);
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

    private Vector3 FindLocalMeanPos(List<GrassNode> grasses)
    {
        float localNum = 0;
        Vector3 totals = new Vector3();
        foreach (GrassNode grass in grasses)
        {
            if (Vector3.Distance(grass.transform.position, transform.position) < localDistance)
            {
                localNum++;
                totals += grass.transform.position;
            }
        }

        return new Vector3(totals.x/ localNum, totals.y/ localNum, totals.z/ localNum);
    }

    private bool IsChokedTest(List<GrassNode> grasses)
    {

        if (EcosystemLogic.instance.IsOutOfBounds(transform.position))
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
