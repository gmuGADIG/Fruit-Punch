using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(GroundCheck))]
public class GrabbableItem : MonoBehaviour
{
    const float groundDecel = 5f;
    Rigidbody rb;
    GroundCheck groundCheck;
    
    void Start()
    {
        this.GetComponentOrError(out rb);
        this.GetComponentOrError(out groundCheck);
    }

    void Update()
    {
        if (groundCheck.IsGrounded() && !rb.isKinematic)
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, groundDecel * Time.deltaTime);
        }
        else if (!groundCheck.IsGrounded() && !rb.isKinematic)
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity / rb.mass, Vector3.zero / rb.mass, groundDecel * Time.deltaTime);
        }
    }
}
