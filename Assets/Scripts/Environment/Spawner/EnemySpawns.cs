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

    private void OnDrawGizmos()
    {

    }
    

}
