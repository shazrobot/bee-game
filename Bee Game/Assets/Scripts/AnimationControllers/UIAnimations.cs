using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimations
{
    //input should be between 0 to 1
    private static float BounceFunction(float zeroToOneInput)
    {
        float n1 = 7.5625f;
        float d1 = 2.75f;

        if (zeroToOneInput < (1f / d1))
        {
            return n1 * zeroToOneInput * zeroToOneInput;
        }
        else if (zeroToOneInput < (2f / d1))
        {
            return n1 * (zeroToOneInput - (1.5f / d1)) * (zeroToOneInput - (1.5f / d1)) + 0.75f;
        }
        else if (zeroToOneInput < (2.5f / d1))
        {
            return n1 * (zeroToOneInput - (2.25f / d1)) * (zeroToOneInput - (2.25f / d1)) + 0.9375f;
        }
        else
        {
            return n1 * (zeroToOneInput - (2.625f / d1)) * (zeroToOneInput - (2.625f / d1)) + 0.984375f;
        }
    }

    //input should be between 0 to 1
    private static float QuadraticFunction(float zeroToOneInput)
    {
        return zeroToOneInput * zeroToOneInput * zeroToOneInput;
    }

    /*
    float Bounce::easeOut(float t, float b, float c, float d)
    {
        if ((t /= d) < (1 / 2.75f))
        {
            return c * (7.5625f * t * t) + b;
        }
        else if (t < (2 / 2.75f))
        {
            float postFix = t -= (1.5f / 2.75f);
            return c * (7.5625f * (postFix) * t + .75f) + b;
        }
        else if (t < (2.5 / 2.75))
        {
            float postFix = t -= (2.25f / 2.75f);
            return c * (7.5625f * (postFix) * t + .9375f) + b;
        }
        else
        {
            float postFix = t -= (2.625f / 2.75f);
            return c * (7.5625f * (postFix) * t + .984375f) + b;
        }
    }
    */

    public static IEnumerator QuadraticScaleAnimation(float initSize, float normSize, float animationDuration, GameObject animationTarget)
    {
        float animationFrameLength = 1f / 60f;
        Vector3 scale;

        for (float elapsed = 0; elapsed <= animationDuration; elapsed += animationFrameLength)
        {
            scale = Vector3.Lerp(Vector3.one * initSize,
                Vector3.one * normSize,
                QuadraticFunction(elapsed / animationDuration));
            //Debug.Log(string.Format("Input: {0}", elapsed / animationDuration));
            //Debug.Log(string.Format("Output: {0}", QuadraticFunction(elapsed / animationDuration)));
            //Debug.Log(string.Format("scale: {0}", scale.x));
            animationTarget.transform.localScale = scale;
            yield return new WaitForSeconds(animationFrameLength);
        }
        animationTarget.transform.localScale = Vector3.one * normSize;
    }

    public static IEnumerator BounceScaleAnimation(float initSize, float normSize, float animationDuration, GameObject animationTarget)
    {
        float animationFrameLength = 1f / 60f;
        Vector3 scale;

        for (float elapsed = 0; elapsed <= animationDuration; elapsed += animationFrameLength)
        {
            scale = Vector3.Lerp(Vector3.one * initSize,
                Vector3.one * normSize,
                BounceFunction(elapsed/animationDuration));
            animationTarget.transform.localScale = scale;
            yield return new WaitForSeconds(animationFrameLength);
        }
        animationTarget.transform.localScale = Vector3.one*normSize;
    }

    public static IEnumerator CustomScaleAnimation(float initSize, float normSize, float animationDuration, GameObject animationTarget, AnimationCurve curve)
    {
        float animationFrameLength = 1f / 60f;
        Vector3 scale;

        for (float elapsed = 0; elapsed <= animationDuration; elapsed += animationFrameLength)
        {
            scale = Vector3.Lerp(Vector3.one * initSize,
                Vector3.one * normSize, 
                curve.Evaluate(elapsed / animationDuration));
            //Debug.Log(string.Format("Input: {0}", elapsed / animationDuration));
            //Debug.Log(string.Format("Output: {0}", BounceFunction(elapsed / animationDuration)));
            //Debug.Log(string.Format("scale: {0}", scale.x));
            animationTarget.transform.localScale = scale;
            yield return new WaitForSeconds(animationFrameLength);
        }
        animationTarget.transform.localScale = Vector3.one * normSize;
    }

    public static IEnumerator BounceMoveAnimation(float initSize, float normSize, float animationDuration, GameObject animationTarget)
    {
        float animationFrameLength = 1f/60f;
        Vector3 scale;

        for (float elapsed = 0; elapsed <= animationDuration; elapsed += animationFrameLength)
        {
            scale = Vector3.Lerp(Vector3.one * initSize,
                Vector3.one * normSize,
                BounceFunction(elapsed / animationDuration));
            //Debug.Log(string.Format("scale: {0}", scale.x));
            animationTarget.transform.localScale = scale;
            yield return new WaitForSeconds(animationFrameLength);
        }
        animationTarget.transform.localScale = Vector3.one * normSize;
    }
}
