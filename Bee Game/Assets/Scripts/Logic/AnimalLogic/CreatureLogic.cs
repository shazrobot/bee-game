using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreatureLogic : MonoBehaviour
{
    public MeshRenderer modelMesh;

    public bool selected;

    public Material selectedMaterial;
    public Material ordinaryMaterial;

    public MovementLogic movementLogic;


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
}
