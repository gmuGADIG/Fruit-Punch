using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Base class for all bosses. <br/>
/// Provides some helpful functions for subclasses, but does NOT handle calling of them. <br/>
/// The subclass should implement a state machine and determine when to call these functions if needed.
/// </summary>
public class Boss : MonoBehaviour
{
    [SerializeField] protected float walkSpeed = 20f;
    protected Rigidbody rb;
    protected NavMeshAgent navMesh;

    void Start()
    {
        this.GetComponentOrError(out rb);
        this.GetComponentOrError(out navMesh);
        navMesh.speed = walkSpeed;
    }

    protected IEnumerator WalkToPlayer()
    {
        var player =
            FindObjectsOfType<Player>()
            .OrderBy(p => Vector3.Distance(this.transform.position, p.transform.position))
            .First();
        
        // TODO: use nav-mesh
        yield return null;
    }
}
