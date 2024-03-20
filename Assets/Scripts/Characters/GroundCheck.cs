using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    /// <summary>
    /// Amount of triggers currently colliding. If zero, the object must be in the air.
    /// </summary>
    private int collisionCount = 0;

    void OnTriggerEnter(Collider other)
    {
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
