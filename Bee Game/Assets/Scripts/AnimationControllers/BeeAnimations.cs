using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeAnimations : MonoBehaviour
{
    [SerializeField]
    private GameObject redStatusIndicator;

    [SerializeField]
    private ParticleSystem healthAnimation;

    [SerializeField]
    private GameObject pollen;

    [SerializeField]
    private Animator animatorController;

    [SerializeField]
    private AnimationClip attackclip;
    [SerializeField]
    private AnimationClip gatherclip;

    private float attackLength;


    private int attackingHash = 0;
    private int gatheringHash = 0;

    private float attackCounter = 0f;

    public delegate void SelectableDelegate(SelectableLogic selectable);
    private SelectableDelegate selectableDelegateFunction;
    private SelectableLogic actionTarget;

    private void Start()
    {
        HidePollen();
        StopHealthAnimation();
        attackingHash = Animator.StringToHash("Attacking");
        gatheringHash = Animator.StringToHash("Gathering");
        attackLength = attackclip.length;
    }

    private void FixedUpdate()
    {
        if (animatorController.GetBool(attackingHash))
        {
            attackCounter += Time.deltaTime;
            if(attackCounter > (attackLength / 2f))
            {
                selectableDelegateFunction(actionTarget);
                StopAttackAnimation();
            }
        }
    }

    public void StopHealthAnimation()
    {
        if (healthAnimation != null)
        {
            healthAnimation.Stop();
        }
    }

    public void PlayHealthAnimation()
    {
        if (healthAnimation != null)
        {
            healthAnimation.Play();
        }
    }

    public void PlayAttackAnimation(SelectableDelegate del, SelectableLogic selectable)
    {
        selectableDelegateFunction = del;
        actionTarget = selectable;
        attackCounter = 0f;
        animatorController.SetBool(attackingHash, true);
    }
    public void StopAttackAnimation()
    {
        animatorController.SetBool(attackingHash, false);
    }

    public void PlayGatherAnimation()
    {
        animatorController.SetBool(gatheringHash, true);
    }

    public void StopGatherAnimation()
    {
        animatorController.SetBool(gatheringHash, false);
    }

    public void SetRedStatus(bool status)
    {
        redStatusIndicator.SetActive(status);
    }

    public void HideRedStatus()
    {
        redStatusIndicator.SetActive(false);
    }

    public void ShowRedStatus()
    {
        redStatusIndicator.SetActive(true);
    }

    public void ShowPollen()
    {
        pollen.gameObject.SetActive(true);
    }

    public void HidePollen()
    {
        pollen.gameObject.SetActive(false);
    }
}
