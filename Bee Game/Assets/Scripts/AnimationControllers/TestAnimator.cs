using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimator : MonoBehaviour
{
    public float animationDuration = 2;
    public AnimationCurve curve;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(UIAnimations.CustomScaleAnimation(transform.localScale.x * 4, transform.localScale.x, animationDuration, gameObject, curve));
        StartCoroutine(UIAnimations.BounceScaleAnimation(transform.localScale.x * 4, transform.localScale.x, animationDuration, gameObject));
    }
}
