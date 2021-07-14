using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightAdjustmentUI : MonoBehaviour
{
    public static HeightAdjustmentUI instance;

    [SerializeField]
    private LineRenderer line;
    [SerializeField]
    private SpriteRenderer big;
    [SerializeField]
    private SpriteRenderer little;

    public void AdjustHeightUI(Vector3 bigPos, Vector3 littlePos)
    {
        ShowHeightUI();
        line.SetPosition(0, littlePos);
        line.SetPosition(1, bigPos);
        little.transform.position = littlePos;
        big.transform.position = bigPos;
    }

    private void ShowHeightUI()
    {
        line.gameObject.SetActive(true);
        big.gameObject.SetActive(true);
        little.gameObject.SetActive(true);
    }

    public void HideHeightUI()
    {
        line.gameObject.SetActive(false);
        big.gameObject.SetActive(false);
        little.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        HideHeightUI();
    }
}
