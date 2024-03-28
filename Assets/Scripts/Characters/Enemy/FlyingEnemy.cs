using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [Tooltip("Parent transform of all objects that need to hover.")]
    [SerializeField] Transform hoveringObjects;

    [Tooltip("Height at which the enemy will hover above the ground")]
    [SerializeField] float hoverHeight = 2f;

    protected override void WanderingEnter()
    {
        hoveringObjects.localPosition = new Vector3(0, hoverHeight, 0);
    }

    protected override void GrabbedEnter()
    {
        base.GrabbedEnter();
        hoveringObjects.localPosition = Vector3.zero;
    }
}
