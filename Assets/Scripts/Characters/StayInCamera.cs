using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Prevents an object from moving outside the camera's bounds, factoring in its collider if it exists. <br/>
/// Note that if an object starts outside of bounds, this script won't immediately force it in; it will only prevent it from going further out of bounds. <br/>
/// This allows items and enemies to start outside.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class StayInCamera : MonoBehaviour
{
    [Tooltip("Upon hitting a wall, this much of the velocity is kept in the opposite direction. 0 = no bounce; 1 = very high bounce.")]
    [SerializeField, Range(0, 1)]
    float bounceFactor = 0;

    // how much the collider extends past the center-point
    float leftExtent = 0, rightExtent = 0;
    Rigidbody rb;
    
    void Start()
    {
        this.GetComponentOrError(out rb);
        if (TryGetComponent<Collider>(out var col))
        {
            // get the bounds of the collider, relative to the center
            leftExtent = col.bounds.min.x - transform.position.x;
            rightExtent = col.bounds.max.x - transform.position.x;
        }
    }

    void LateUpdate()
    {
        Vector3 position = transform.position;

        float distance = transform.position.x - Camera.main.transform.position.x;

        float leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance)).x - leftExtent;
        float rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance)).x - rightExtent;

        var isOutsideBounds =
            position.x <= leftBorder  && rb.velocity.x < 0 || 
            position.x >= rightBorder && rb.velocity.x > 0;

        if (isOutsideBounds)
        {
            var vel = rb.velocity;
            vel.x *= -1 * bounceFactor; // flip and dampen
            rb.velocity = vel;
        }

            // position.x = Mathf.Clamp(position.x, leftBorder, rightBorder);

        transform.position = position;
    }
}
