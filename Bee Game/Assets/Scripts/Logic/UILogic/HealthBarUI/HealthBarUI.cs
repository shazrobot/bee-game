using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField]
    private Image healthBar;

    private SelectableLogic selectable = null;

    private float yAdjustment = 5f;

    public void SetObject(SelectableLogic obj)
    {
        selectable = obj;
        ColourHealthBar();
        SetHealthBar();
        ScaleHealthBar(obj.GetUIScale()/6f);
    }

    private void ColourHealthBar()
    {
        if (selectable.GetComponent<CreatureLogic>() != null)
        {
            healthBar.color = selectable.GetComponent<CreatureLogic>().GetFaction().GetFactionColour();
        }
        else if (selectable.GetComponent<HiveLogic>() != null)
        {
            healthBar.color = selectable.GetComponent<HiveLogic>().GetFaction().GetFactionColour();
        }
        else
        {
            healthBar.color = ColourData.instance.neutral;
        }
    }

    private void ScaleHealthBar(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void SetHealthBar()
    {
        if (selectable != null)
        {
            healthBar.fillAmount = selectable.GetHealthRatio();
        }
    }

    public void UpdatePosition()
    {
        if (selectable != null){
            // feel free to offset this by a bit vertically


            Vector3 unitPos = new Vector3(selectable.GetUIPosition().position.x,
                selectable.GetUIPosition().position.y + yAdjustment,
                selectable.GetUIPosition().position.z);

            Vector2 screenPoint = Camera.main.WorldToScreenPoint(unitPos);

            //GetComponent<RectTransform>().anchoredPosition = screenPoint;
            //GetComponent<RectTransform>().localPosition = screenPoint;
            gameObject.transform.position = screenPoint;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void UpdateHealthBar()
    {
        SetHealthBar();
    }
}
