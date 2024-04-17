using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// Base class for all bosses. <br/>
/// Provides some helpful functions for subclasses, but does NOT handle calling of them. <br/>
/// The subclass should implement a state machine and determine when to call these functions if needed.
/// </summary>
public class Boss : MonoBehaviour
{
    protected Rigidbody rb;
    protected NavMeshAgent navMesh;

    protected void Start()
    {
        this.GetComponentOrError(out rb);
        this.GetComponentOrError(out navMesh);
    }

    /// <summary>
    /// This coroutine walks towards the nearest player, exiting when it reaches it. <br/>
    /// Uses the navmesh agent to calculate motion, and the rigidbody to actually apply motion.
    /// </summary>
    protected IEnumerator WalkToPlayer(float speed)
    {
        const float DIST_THRESHOLD = 0.5f; // stops walking when this close
        const float SAFETY_TIMEOUT = 10f; // aborts coroutine after this many seconds, to prevent soft-lock in case of error

        var player = GetNearestPlayer();
        
        Debug.Assert(player != null); // TODO: handle this gracefully
        navMesh.ResetPath();

        var safetyTimer = 0f; // if traversal fails for too long, just exit
        while (true)
        {
            navMesh.SetDestination(player.transform.position);
            rb.velocity = (navMesh.steeringTarget - transform.position).normalized * speed;
            
            safetyTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();

            // exit gracefully
            if (Vector3.Distance(transform.position, player.transform.position) < DIST_THRESHOLD)
            {
                break;
            }

            // exit aaa scary
            if (safetyTimer > SAFETY_TIMEOUT)
            {
                Debug.LogWarning("Boss couldn't reach player in 10 seconds! Aborting coroutine.");
                break;
            }
        }
        
        print("finished moving to player");
        rb.velocity = Vector3.zero;
    }

    protected Player GetNearestPlayer()
    {
        return
            FindObjectsOfType<Player>()
            .OrderBy(p => Vector3.Distance(this.transform.position, p.transform.position))
            .First();
    }
    
    /// <summary>
    /// Handles flipping the boss based on its moving direction <br/>
    /// </summary>
    protected void FlipWithVelocity(Vector3 velocity)
    {
        if (velocity.x < 0) transform.localEulerAngles = new Vector3(0, 180, 0);
        else if (velocity.x > 0) transform.localEulerAngles = Vector3.zero;
    }
}
