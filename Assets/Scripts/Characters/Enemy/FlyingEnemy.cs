using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [Tooltip("Parent transform of all objects that need to hover.")]
    [SerializeField] Transform hoveringObjects;

    [Tooltip("Height at which the enemy will hover above the ground")]
    [SerializeField] float hoverHeight = 2f;

    [Tooltip("How far up and down the enemy will bob when in the air.")]
    [SerializeField] float hoverVariance = .1f;

    [Tooltip("How quickly the enemy will bob up and down.")]
    [SerializeField] float bobSpeed = 2f;

    protected override EnemyState WanderingUpdate()
    {
        hoveringObjects.localPosition = (hoverHeight + hoverVariance * Mathf.Sin(bobSpeed * Time.time)) * Vector3.up;
        return base.WanderingUpdate();
    }

    protected override EnemyState AggressiveUpdate()
    {
        hoveringObjects.localPosition = (hoverHeight + hoverVariance * Mathf.Sin(bobSpeed * Time.time)) * Vector3.up;
        return base.AggressiveUpdate();
    }

    protected override EnemyState AttackingUpdate()
    {
        hoveringObjects.localPosition = (hoverHeight + hoverVariance * Mathf.Sin(bobSpeed * Time.time)) * Vector3.up;
        return base.AttackingUpdate();
    }

    protected override void GrabbedEnter()
    {
        base.GrabbedEnter();
        hoveringObjects.localPosition = Vector3.zero;
    }
}
