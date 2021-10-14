using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverSpriteLogic : MonoBehaviour
{
    public static MouseOverSpriteLogic instance;
    [SerializeField]
    private SpriteRenderer mouseOverDisplay;
    private Transform target;

    [SerializeField]
    private Texture2D defaultMouse;

    [SerializeField]
    private Texture2D friendlyMouse;

    [SerializeField]
    private Texture2D neutralMouse;

    [SerializeField]
    private Texture2D unfriendlyMouse;

    [SerializeField]
    private Texture2D defaultAttackMouse;

    [SerializeField]
    private Texture2D friendlyAttackMouse;

    [SerializeField]
    private Texture2D neutralAttackMouse;

    [SerializeField]
    private Texture2D unfriendlyAttackMouse;

    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 defaultSpot = Vector2.zero;
    private Vector2 midSpot;
    private bool attackMode = false;

    private void Start()
    {
        instance = this;
        mouseOverDisplay.gameObject.SetActive(false);
        attackMode = false;
        midSpot = new Vector2(defaultAttackMouse.width / 2f, defaultAttackMouse.width / 2f);
    }

    public void AttackModeOn()
    {
        attackMode = true;
        Cursor.SetCursor(defaultAttackMouse, midSpot, cursorMode);
    }
    public void AttackModeOff()
    {
        attackMode = false;
        Cursor.SetCursor(defaultMouse, defaultSpot, cursorMode);
    }

    public void SetTarget(FriendlinessType friendliness, Transform newTarget = null, int scale = 5)
    {
        if(newTarget == null)
        {
            mouseOverDisplay.gameObject.SetActive(false);
            if(attackMode)
                Cursor.SetCursor(defaultAttackMouse, midSpot, cursorMode);
            else
                Cursor.SetCursor(defaultMouse, defaultSpot, cursorMode);
            mouseOverDisplay.color = Color.white;
        }
        else
        {
            mouseOverDisplay.gameObject.SetActive(true);
            mouseOverDisplay.transform.position = newTarget.transform.position;
            mouseOverDisplay.transform.localScale = scale*Vector3.one;
            if (friendliness == FriendlinessType.None)
            {
                if (attackMode)
                    Cursor.SetCursor(defaultAttackMouse, midSpot, cursorMode);
                else
                    Cursor.SetCursor(defaultMouse, defaultSpot, cursorMode);
                mouseOverDisplay.color = Color.white;
            }
            if (friendliness == FriendlinessType.Friendly)
            {
                if (attackMode)
                    Cursor.SetCursor(friendlyAttackMouse, midSpot, cursorMode);
                else
                    Cursor.SetCursor(friendlyMouse, defaultSpot, cursorMode);
                mouseOverDisplay.color = ColourData.instance.friendly;
            }
            if (friendliness == FriendlinessType.Neutral)
            {
                if (attackMode)
                    Cursor.SetCursor(neutralAttackMouse, midSpot, cursorMode);
                else
                    Cursor.SetCursor(neutralMouse, defaultSpot, cursorMode);
                mouseOverDisplay.color = ColourData.instance.neutral;
            }
            if (friendliness == FriendlinessType.Hostile)
            {
                if (attackMode)
                    Cursor.SetCursor(unfriendlyAttackMouse, midSpot, cursorMode);
                else
                    Cursor.SetCursor(unfriendlyMouse, defaultSpot, cursorMode);
                mouseOverDisplay.color = ColourData.instance.hostile;
            }
        }
    }
}
