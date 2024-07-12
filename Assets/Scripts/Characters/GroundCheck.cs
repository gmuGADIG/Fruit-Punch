using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundCheck : MonoBehaviour
{
    /// <summary>
    /// Amount of triggers currently colliding. If zero, the object must be in the air.
    /// </summary>
    private int collisionCount = 0;

    public UnityEvent GroundHit;

    void OnTriggerEnter(Collider other)
    {
        Debug.Assert(other.gameObject.layer == LayerMask.NameToLayer("World"));
        GroundHit.Invoke();
        collisionCount += 1;
    }
    
    void OnTriggerExit(Collider other)
    {
        collisionCount -= 1;
    }

    public bool IsGrounded()
    {
        return collisionCount > 0;
    }
}
