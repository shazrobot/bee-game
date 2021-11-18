using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private SelectableLogic target = null;
    private float distanceThreshold = 0.5f;
    private float projectileSpeed = 80f;

    private CreatureLogic attackSource = null;

    public void SetParameters(SelectableLogic selectable, CreatureLogic source)
    {
        transform.position = source.GetUIPosition().position;
        target = selectable;
        attackSource = source;
    }

    private void MoveTowards()
    {
        Vector3 bearing = Vector3.Normalize(target.GetUIPosition().position - transform.position);
        if (WithinMoveRange())
        {
            transform.position = target.GetUIPosition().position;
        }
        else
        {
            transform.position += bearing * projectileSpeed * Time.deltaTime;
        }
        transform.LookAt(target.GetUIPosition().position, Vector3.up);
    }

    private bool WithinMoveRange()
    {
        return (Vector3.Distance(target.GetUIPosition().position, transform.position) < projectileSpeed * Time.deltaTime);
    }

    private bool ReachedTarget()
    {
        return (Vector3.Distance(target.GetUIPosition().position, transform.position) < distanceThreshold);
    }

    private void HitTarget()
    {
        attackSource.ProjectileLandedOnObjective(target);
        target = null;
        attackSource = null;
        gameObject.SetActive(false);
    }

    public bool IsActive()
    {
        return target != null;
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            MoveTowards();
            if (ReachedTarget())
            {
                HitTarget();
            }
        }
    }
}
