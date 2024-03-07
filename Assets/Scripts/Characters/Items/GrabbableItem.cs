using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class GrabbableItem : MonoBehaviour
{
    const float groundDecel = 5f;
    Rigidbody rb;
    
    void Start()
    {
        this.GetComponentOrError(out rb);
    }

    void Update()
    {
        if (IsGrounded()) rb.velocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, groundDecel * Time.deltaTime);
    }

    bool IsGrounded()
    {
        // overlap a box below the player to see if it's grounded
        // uses the hitbox's x and z size, with an arbitrary height
        var hitBox = GetComponent<BoxCollider>();
        const float groundBoxHeight = 0.05f;
        
        var overlaps = Physics.OverlapBox(
            transform.position, 
            new Vector3(hitBox.size.x, groundBoxHeight, hitBox.size.z),
            Quaternion.identity,
            LayerMask.GetMask("Ground")
        );

        return overlaps.Length > 0;
    }
}
