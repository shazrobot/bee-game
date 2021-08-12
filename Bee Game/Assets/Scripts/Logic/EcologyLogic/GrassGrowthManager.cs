using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrassGrowthManager : MonoBehaviour
{
    public GrassNode grassTemplate;


    [SerializeField]
    private List<GrassNode> allGrasses = new List<GrassNode>();

    private List<GrassNode> nonChokedGrasses = new List<GrassNode>();

    //Manually defined for current scale of 2 with current sprite
    private int GrassNodeRadius = 9;

    // Start is called before the first frame update
    void Start()
    {
        grassTemplate.gameObject.SetActive(false);
        nonChokedGrasses.AddRange(allGrasses);
    }

    public bool WithinRangeOfGrass(Vector3 testPosition)
    {
        foreach(GrassNode grass in allGrasses)
        {
            Vector3 grassPos = new Vector3(grass.transform.position.x, testPosition.y, grass.transform.position.z);
            if (Vector3.Distance(grassPos, testPosition) <= GrassNodeRadius)
                return true;
        }
        return false;
    }

    public void createNewGrass(Vector3 position)
    {
        GrassNode grass = Instantiate(grassTemplate) as GrassNode;

        grass.gameObject.SetActive(true);
        grass.transform.parent = grassTemplate.transform.parent;
        grass.transform.position = position;
        
        allGrasses.Add(grass);
        nonChokedGrasses.Add(grass);
    }

    public void GrassReproduction()
    {
        List<Vector3> newPositions = new List<Vector3>();
        List<GrassNode> chokedGrasses = new List<GrassNode>();
        Vector3 testPos;
        int tempCounter=0;
        foreach (GrassNode grass in nonChokedGrasses)
        {
            testPos = grass.Reproduce(allGrasses);
            tempCounter += 1;
            if (!(testPos == Vector3.zero) || (testPos == null))
            {

                newPositions.Add(testPos);
            }
            else
            {
                chokedGrasses.Add(grass);
            }
        }
        nonChokedGrasses = new List<GrassNode>(nonChokedGrasses.Except(chokedGrasses));
        foreach (Vector3 position in newPositions)
        {
            createNewGrass(position);
        }
        //Debug.Log("Attempts: " + tempCounter + " many grass nodes tried to reproduce");
        //Debug.Log("created: "+newPositions.Count+" many grass nodes successfully reproduced");
    }
}
