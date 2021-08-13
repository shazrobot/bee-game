using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableLogic : MonoBehaviour
{
    private  bool selected;
    [SerializeField]
    private int uIScale = 5;
    [SerializeField]
    private Transform manualUITransform;

    [SerializeField]
    private protected int maxHealth = 100;
    private protected int currentHealth = 100;

    private protected bool dead = false;

    protected virtual void Awake()
    {
        selected = false;
        currentHealth = maxHealth;
        if (manualUITransform != null)
        {
            manualUITransform.gameObject.SetActive(false);
        }
    }

    public bool IsSelected()
    {
        return selected;
    }

    public void Select()
    {
        selected = true;
    }

    public void Deselect()
    {
        selected = false;
    }

    public int GetUIScale()
    {
        if(uIScale <= 0)
        {
            return 100;
        }
        return uIScale;
    }

    public Transform GetUIPosition()
    {
        if (manualUITransform != null)
            return manualUITransform;
        else
            return transform;
    }

    public bool IsDead()
    {
        return dead;
    }

    public bool IsDamaged()
    {
        return currentHealth != maxHealth;
    }

    public float GetHealthRatio()
    {
        return (currentHealth/(float)maxHealth);
    }

    public virtual void ChangeHealth(int healthChange)
    {
        currentHealth += healthChange;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        else if (currentHealth <= 0)
        {
            currentHealth = 0;
            dead = true;
            gameObject.SetActive(false);
        }
            
    }
}
