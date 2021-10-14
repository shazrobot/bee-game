using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystemLogic : MonoBehaviour
{

    public List<GameObject> saveableObjects;

    private List<GameObject> savedObjects;

    private void WriteDataWithJSON(GameObject obj)
    {
        string jsonData = JsonUtility.ToJson(obj);
        PlayerPrefs.SetString(obj.name, jsonData);
        savedObjects.Add(obj);
    }

    private GameObject ReadDataWithJSON(string objName)
    {
        string obj = PlayerPrefs.GetString(objName);
        GameObject jsonData = JsonUtility.FromJson<GameObject>(obj);
        return jsonData;
    }



    public void SaveGameState()
    {
        foreach (GameObject obj in saveableObjects)
        {
            WriteDataWithJSON(obj);
        }
    }

    public void LoadSaveState()
    {

    }

}
