using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawns : MonoBehaviour
{
    /// <summary>
    /// Spawner type that affects how the enemy is spawned in the screen.
    /// </summary>
    public enum SpawnPointType
    {
        Left,
        Right,
        Ground,
        JumpIn
    }

    

    [Tooltip("How the enemy should spawned into the screen")]
    public SpawnPointType spawnType;

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
        return copy;
    }

    private void OnDrawGizmosSelected()
    {
        // Get a line of points from the spawner, in the direction of the velocity over time minus gravity to show the path the enemy will take.
        Vector3[] points = new Vector3[50];
        points[0] = transform.position;
        Vector3 velocity = spawnVelocity;
        for (int i = 1; i < points.Length; i++)
        {
            velocity += Physics.gravity * Time.fixedDeltaTime;
            points[i] = points[i - 1] + velocity * Time.fixedDeltaTime;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(points[points.Length - 1], 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLineList(points);

    }
    

}
