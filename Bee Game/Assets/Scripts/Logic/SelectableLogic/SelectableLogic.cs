using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableLogic : MonoBehaviour
{
    private  bool selected;
    [SerializeField]
    private int uIScale = 5;
    [SerializeField]
    private float radius = 5;
    [SerializeField]
    private Transform manualUITransform;

    [SerializeField]
    private protected float maxHealth = 100;
    private protected float currentHealth = 100;

    private protected bool dead = false;

    private float healthRegenCounter = 0;
    private int healthRegenAmount = 1;
    private float healthRegenTime = 5f;

    protected virtual void Awake()
    {
        selected = false;
        currentHealth = maxHealth;
        if (manualUITransform != null)
        {
            manualUITransform.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (!dead)
        {
            IncrementHealthTimer();
        }
    }

    //Getters

    public float GetRadius()
    {
        return radius;
    }

    public bool IsSelected()
    {
        return selected;
    }

    public int GetUIScale()
    {
        if (uIScale <= 0)
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
        return (currentHealth / (float)maxHealth);
    }

    //Setters

    public void Select()
    {
        selected = true;
    }

    public void Deselect()
    {
        selected = false;
    }

    public virtual void ChangeHealth(float healthChange)
    {
        ResetHealthTimer();
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

    protected void IncrementHealthTimer()
    {
        healthRegenCounter += Time.deltaTime;
        if (healthRegenCounter >= healthRegenTime)
        {
            ChangeHealth(healthRegenAmount);
        }
    }

    protected void ResetHealthTimer()
    {
        healthRegenCounter = 0f;
    }

    
}
