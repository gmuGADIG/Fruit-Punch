using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawns : MonoBehaviour
{
    /// <summary>
    /// Spawner type that affects how the enemy is spawned in the screen.
    /// </summary>
    //public enum SpawnPointType
    //{
    //    Left,
    //    Right,
    //    Ground,
    //    JumpIn
    //}

    

    //[Tooltip("How the enemy should spawned into the screen")]
    //public SpawnPointType spawnType;

    [Tooltip("The velocity the enemy should spawn with. Enemy counts as spawned when they land.")]
    public Vector3 spawnVelocity;

    /// <summary>
    /// Creates a copy of the provided enemy at this spawner's location and returns it.
    /// </summary>
    /// <param name="enemy">Source enemy to copy</param>
    /// <param name="aura"></param>
    /// <returns></returns>
    public Enemy SpawnEnemy(Enemy enemy, AuraType aura)
    {
        Enemy copy = Instantiate(enemy, transform.position, Quaternion.identity);
        copy.GetComponent<Health>().vulnerableTypes = aura;
        copy.GetComponent<Rigidbody>().velocity = spawnVelocity;
        return copy;
    }

    private void OnDrawGizmosSelected()
    {
        const float gizmoSize = 0.1f;

        // Get a line of points from the spawner, in the direction of the velocity over time minus gravity to show the path the enemy will take.
        Vector3[] points = new Vector3[50];
        points[0] = transform.position;
        Vector3 velocity = spawnVelocity;
        for (int i = 1; i < points.Length; i++)
        {
            velocity += Physics.gravity * Time.fixedDeltaTime*2;
            points[i] = points[i - 1] + velocity * Time.fixedDeltaTime*2;
            if (Physics.Raycast(points[i-1], velocity.normalized, out RaycastHit hit, velocity.magnitude * Time.fixedDeltaTime*2))
            {
                points[i] = hit.point;
                break;
            }
        }
        //if the array is full, the enemy never hit anything, so draw a line to the last point and log a warning
        if (points[points.Length - 1] != Vector3.zero)
        {
            Debug.LogWarning("Enemy path was not interrupted by a collider. Enemy will continue off screen and never spawn.");
            Gizmos.color = Color.red;
        }
        else
        {
            //rezise the array to the number of points that were actually used
            System.Array.Resize(ref points, System.Array.IndexOf(points, Vector3.zero));
            Gizmos.color = Color.yellow;
        }
        Gizmos.DrawSphere(transform.position, gizmoSize);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(points[points.Length - 1], gizmoSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawLineStrip(points, false);

    }
    

}
