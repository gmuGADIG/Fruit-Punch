using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [Tooltip("When a player falls in the hole, they're brought back at this position.")]
    [SerializeField] Transform respawnPoint;

    [Tooltip("How much to hurt the player for falling.")]
    [SerializeField] float damage = 50;

    new Collider collider;
    List<Collider> previousCollisions = new();

    void Start()
    {
        this.GetComponentOrError(out collider);
    }

    void Update()
    {
        LayerMask collisionLayers = LayerMask.GetMask("Player");
        var collisions = collider.NewCollisions(collisionLayers, ref previousCollisions);
        foreach (var col in collisions)
        {
            StartCoroutine(FallCoroutine(col));
        }
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
