using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundCheck : MonoBehaviour
{
    /// <summary>
    /// List of colliders currently colliding. If count is zero, the object must be in the air.
    /// </summary>
    List<Collider> collidingWith = new List<Collider>();

    public UnityEvent GroundHit;

    private void Update()
    {
        // look for and remove colliders that were destroyed.
        for (int i = 0; i < collidingWith.Count; i++)
        {
            if (collidingWith[i] == null)
            {
                collidingWith.Remove(collidingWith[i]);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Assert(other.gameObject.layer == LayerMask.NameToLayer("World"));
        GroundHit.Invoke();
        collidingWith.Add(other);
    }
    
    void OnTriggerExit(Collider other)
    {
        collidingWith.Remove(other);
    }

    public bool IsGrounded()
    {
        return collidingWith.Count > 0;
    }
}
