using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploderEnemy : Enemy
{
    [SerializeField] GameObject explosionPrefab;

    // Countdown state

    [Tooltip("How many before the explosion countdown starts.")]
    [SerializeField] float preExplosionTimerDuration = 10f;

    /// <summary>
    /// The internal counter. <see cref="preExplosionTimerDuration"/> is the default
    /// time.
    /// </summary>
    float preExplosionCountdown;

    bool InPreCountdown => preExplosionCountdown > 0;
    public bool IsExploding => stateMachine.currentState == EnemyState.Attacking;

    [Tooltip("How many seconds until the enemy explodes.")]
    [SerializeField] float explosionTimerDuration = 10f;

    SpriteRenderer sprite;

    protected override void Start()
    {
        base.Start();

        preExplosionCountdown = preExplosionTimerDuration;
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void AggressiveEnter()
    {
        base.AggressiveEnter();
        if (InPreCountdown) {
            preExplosionCountdown = preExplosionTimerDuration;
        }
    }

    // When the enemy is in the countdown state, it shouldn't be able to leave that state?
    // Hurt becomes flinched
    // Wander can never be returned to
    // In-Air is impossible if the NavMesh works
    // Attacking is when it explodes
    // Grabbed is impossible because the grabbable is disabled

    protected override EnemyState AggressiveUpdate()
    {
        if (InPreCountdown) {
            preExplosionCountdown -= Time.deltaTime;
        } else {
            // enemy in exploding countdown cannot be grabbed.
            grabbable.enabled = false;
            explosionTimerDuration -= Time.deltaTime;

            sprite.color = explosionTimerDuration % 1 >= .5 ? Color.white : Color.red; 
        }

        if (explosionTimerDuration <= 0) {
            return EnemyState.Attacking;
        }
        base.AggressiveUpdate(); // ignore the output of base.AggressiveUpdate()
        return stateMachine.currentState;
    }

    // exploding state
    protected override void AttackingEnter()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        GetComponent<Health>().Die();
    }
}
