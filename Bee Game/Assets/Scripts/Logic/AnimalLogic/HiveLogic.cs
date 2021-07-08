using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveLogic : MonoBehaviour
{
    public MeshRenderer modelMesh;

    public bool selected;

    public Material selectedMaterial;
    public Material ordinaryMaterial;

    public CreatureLogic beeTemplate;

    [NonSerialized]
    public int beeQueue = 0;
    private int beeQueueMax = 6;
    [NonSerialized]
    public float constructionTimer = 0f;
    [NonSerialized]
    public float beeConstructionTime = 30f;


    public void Awake()
    {
        selected = false;
        beeTemplate.gameObject.SetActive(false);
    }

    public void UpdateMaterial()
    {
        if (selected)
        {
            modelMesh.material = selectedMaterial;
        }
        else
        {
            modelMesh.material = ordinaryMaterial;
        }
    }

    public void Select()
    {
        selected = true;
        UpdateMaterial();
    }

    public void Deselect()
    {
        selected = false;
        UpdateMaterial();
    }

    public void BuildBee()//Build bee
    {
        CreatureLogic bee = Instantiate(beeTemplate) as CreatureLogic;

        bee.gameObject.SetActive(true);
        bee.transform.SetParent(beeTemplate.transform.parent);
        bee.transform.position = transform.position;
        beeQueue -= 1;
        ResetTimer();
    }

    private void ResetTimer()
    {
        constructionTimer = 0f;
    }

    public bool QueueBeeBuild()
    {
        if(beeQueue == 6)
        {
            return false;
        }
        else
        {
            beeQueue += 1;
            return true;
        }
    }

    private void IncrementTime()
    {
        if (beeQueue > 0)
        {
            constructionTimer += Time.deltaTime;
            if (constructionTimer >= beeConstructionTime)
            {
                BuildBee();
            }
        }
    }

    public void Update()
    {
        IncrementTime();
    }
}
