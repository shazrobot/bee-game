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

    private List<GrassNode> customGrasses = new List<GrassNode>();

    //Manually defined for current scale of 2 with current sprite
    private int grassNodeRadius = 9;

    private Vector3 fertilityGrassScale = new Vector3(4, 4, 4);

    // Start is called before the first frame update
    void Start()
    {
        grassTemplate.gameObject.SetActive(false);
        StartupGrassPopulate();
        nonChokedGrasses.AddRange(allGrasses);
    }

    private void StartupGrassPopulate()
    {
        allGrasses.Clear();

        allGrasses.AddRange(FindObjectsOfType<GrassNode>());
        allGrasses.Remove(grassTemplate);
    }

    private List<GrassNode> GrassListRandomiser(List<GrassNode> list)
    {
        List<GrassNode> alpha = new List<GrassNode>(list);
        for (int i = 0; i < list.Count; i++)
        {
            GrassNode temp = alpha[i];
            int randomIndex = Random.Range(i, list.Count);
            alpha[i] = alpha[randomIndex];
            alpha[randomIndex] = temp;
        }

        return alpha;
    }

    public bool WithinRangeOfGrass(Vector3 testPosition)
    {
        foreach(GrassNode grass in allGrasses)
        {
            Vector3 grassPos = new Vector3(grass.transform.position.x, testPosition.y, grass.transform.position.z);
            if (Vector3.Distance(grassPos, testPosition) <= grassNodeRadius)
                return true;
        }
        return false;
    }

    public void CreateCustomGrassNode(Vector3 position)
    {
        GrassNode grass = Instantiate(grassTemplate) as GrassNode;

        grass.gameObject.SetActive(true);
        grass.transform.parent = grassTemplate.transform.parent;
        grass.transform.position = position;
        grass.transform.localScale = fertilityGrassScale;

        allGrasses.Add(grass);
        nonChokedGrasses.Add(grass);
    }

    public void CreateNewGrass(Vector3 position)
    {
        GrassNode grass = Instantiate(grassTemplate) as GrassNode;

        grass.gameObject.SetActive(true);
        grass.transform.parent = grassTemplate.transform.parent;
        grass.transform.position = position;
        
        allGrasses.Add(grass);
        nonChokedGrasses.Add(grass);
        //StartCoroutine(UIAnimations.BounceScaleAnimation(grass.transform.localScale.x * 2, grass.transform.localScale.x, 2f, grass.gameObject));
    }

    public IEnumerator GrassReproductionCoroutine()
    {
        List<Vector3> newPositions = new List<Vector3>();
        List<GrassNode> chokedGrasses = new List<GrassNode>();
        Vector3 testPos;
        int tempCounter = 0;

        List<GrassNode> reproductionGroup = new List<GrassNode>(nonChokedGrasses);
        float timeBetweenSpawns = TimeManager.instance.GetSeasonDuration() / reproductionGroup.Count;
        //find all the new grass positions
        foreach (GrassNode grass in GrassListRandomiser(reproductionGroup))
        {
            testPos = grass.Reproduce(allGrasses);
            tempCounter += 1;
            if (!(testPos == Vector3.zero) || (testPos == null))
            {

                newPositions.Add(testPos);
                CreateNewGrass(testPos);
            }
            else
            {
                chokedGrasses.Add(grass);
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        //place new grass
        nonChokedGrasses = new List<GrassNode>(nonChokedGrasses.Except(chokedGrasses));
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
            CreateNewGrass(position);
        }
    }
}
