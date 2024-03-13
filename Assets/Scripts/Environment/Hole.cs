using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// For the Hole prefab, which should be placed below a gap in the floor. <br/>
/// Upon collision, damages the player and respawns them at the surface. <br/>
/// Note that falling down the hole is not done by this script; it's done by adjusting the floor colliders.
/// </summary>
public class Hole : MonoBehaviour
{
    [Tooltip("When a player falls in the hole, they're brought back at this position.")]
    [SerializeField] Transform respawnPoint;

    [Tooltip("How much to hurt the player for falling.")]
    [SerializeField] float damage = 50;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() == null) return;
        
        StartCoroutine(FallCoroutine(other));
    }

    IEnumerator FallCoroutine(Collider col)
    {
        yield return new WaitForSeconds(1);
        
        col.transform.position = respawnPoint.position;
            
        var health = col.GetComponent<Health>();
        health.Damage(new DamageInfo(damage, Vector2.zero, AuraType.EnemyAtk));

        var rb = col.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
    }
}
