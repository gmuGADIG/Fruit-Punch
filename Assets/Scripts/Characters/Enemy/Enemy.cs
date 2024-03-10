using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyState
{
    Wandering, Aggressive, Attacking, Hurt, Grabbed, InAir
}

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    /// <summary>
    /// To prevent too many enemies from attacking at once, this stores the current amount. <br/>
    /// Incremented when aggressive, decremented when an attack ends.
    /// </summary>
    private static int currentAttackingEnemies = 0;
    private const int MaxSimultaneousAttackers = 2;
    
    [Tooltip("How fast the enemy approaches the player, in \"meters\" per second")]
    [SerializeField] private float walkingSpeed;

    [Tooltip("When the enemy is this close to the player, it will start attacking.")]
    [SerializeField] private float attackingDistance;
    
    [Tooltip("Enemy will leave wandering state after at least this many seconds.")]
    [SerializeField] private float wanderingTimeMin = 5000;
    [Tooltip("Enemy will leave wandering state after no more than this many seconds.")]
    [SerializeField] private float wanderingTimeMax = 10000;

    [SerializeField] float gravity = 10f;

    [Tooltip("This much damage multiplied by its mass is dealt on throw, both to itself and anything it hits.")]
    [SerializeField] float throwBaseDamage = 20f;
    
    const float attackingDuration = 2; // how long the enemy stays in attacking state. might be worth reading its animation for this value eventually

    const float wanderXDistance = 1f; // each time the enemy chooses to wander, it can go this far in the x-direction
    const float wanderZMin = -0.5f, wanderZMax = 0.5f; // each time the enemy chooses to wander, it's new z position must be within these bounds
    const float wanderTimeBetweenSteps = 3f; // when wandering, the enemy takes a few steps with this period

    const float grabTimeToEscape = 5f; // enemy will break out of a grab after this long

    private float wanderingTimeTillAttack = 0;
    private Vector3 wanderingToPosition;
    private float wanderingTimeTillWander;
    
    /// <summary>
    /// If the enemy is in the approaching state, this value will be the object it's going towards.
    /// Otherwise, it's value is meaningless.
    /// </summary>
    private Transform aggressiveCurrentTarget = null;

    private float hurtTimeLeft;

    /// <summary>
    /// When the enemy is thrown, it queues its damaged, taking it when it lands.
    /// </summary>
    private bool thrownDamageQueue = false;
    
    private GroundCheck groundCheck;
    private Grabbable grabbable;
    private Rigidbody rb;
    private Health health;
    private StateMachine<EnemyState> stateMachine = new();

    private void Start()
    {
        this.GetComponentOrError(out rb);
        this.GetComponentOrError(out grabbable);
        this.GetComponentOrError(out health);
        this.GetComponentInChildrenOrError(out groundCheck);
  
        stateMachine.AddState(EnemyState.Wandering, WanderingEnter, WanderingUpdate, null);
        stateMachine.AddState(EnemyState.Aggressive, AggressiveEnter, AggressiveUpdate, null);
        stateMachine.AddState(EnemyState.Attacking, AttackingEnter, AttackingUpdate, AttackingExit);
        stateMachine.AddState(EnemyState.Hurt, HurtEnter, HurtUpdate, null);
        stateMachine.AddState(EnemyState.Grabbed, null, GrabbedUpdate, null);
        stateMachine.AddState(EnemyState.InAir, null, InAirUpdate, InAirExit);
        stateMachine.FinalizeAndSetState(EnemyState.Wandering);

        grabbable.onGrab += OnGrabCallback;
        grabbable.onThrow += OnThrowCallback;
    }

    private void Update()
    {
        stateMachine.Update();
        if (rb.isKinematic == false) rb.velocity += Vector3.down * gravity * Time.deltaTime;
    }

    void WanderingEnter()
    {
        wanderingTimeTillAttack = Random.Range(wanderingTimeMin, wanderingTimeMax);
        wanderingToPosition = transform.position;
    }
    
    EnemyState WanderingUpdate()
    {
        if (groundCheck.IsGrounded() == false) return EnemyState.InAir;
        
        // print($"enemy status ({gameObject.name}): wandering, to {wanderingToPosition}, re-wandering in {wanderingTimeTillWander}, attacking in {wanderingTimeTillAttack}");
        // changing to aggressive state after waiting some time
        wanderingTimeTillAttack -= Time.deltaTime;
        if (wanderingTimeTillAttack <= 0)
        {
            if (currentAttackingEnemies < MaxSimultaneousAttackers) // TODO: multiply by player count
            {
                return EnemyState.Aggressive;
            }
        }
        
        // approach randomly set wander point (unless already right next to it)
        var vecToTarget = wanderingToPosition - transform.position;
        if (vecToTarget.magnitude > 0.1f) rb.velocity = vecToTarget.normalized * walkingSpeed;
        else rb.velocity = Vector3.zero;
        if (wanderingTimeTillWander < 0)
        {
            wanderingToPosition = new Vector3(
                transform.position.x + Random.Range(-wanderXDistance, +wanderXDistance),
                Random.Range(wanderZMin, wanderZMax),
                transform.position.z
            );
            wanderingTimeTillWander = wanderTimeBetweenSteps;
        }

        wanderingTimeTillWander -= Time.deltaTime;
        
        return stateMachine.currentState;
    }

    void AggressiveEnter()
    {
        currentAttackingEnemies += 1;
        
        // start approaching the nearest player
        aggressiveCurrentTarget =
            FindObjectsOfType<Player>()
            .OrderBy(p => Vector3.Distance(this.transform.position, p.transform.position))
            .First().transform;
    }
    
    EnemyState AggressiveUpdate()
    {
        if (groundCheck.IsGrounded() == false) return EnemyState.InAir;
        
        // print($"enemy status ({gameObject.name}): aggressive, targeting {aggressiveCurrentTarget.name}");
        var vecToTarget = (aggressiveCurrentTarget.position - this.transform.position);
        vecToTarget.y = 0;
        rb.velocity = vecToTarget.normalized * walkingSpeed;

        if (vecToTarget.magnitude < attackingDistance)
        {
            return EnemyState.Attacking;
        }

        return stateMachine.currentState;
    }

    void AttackingEnter() { }
    
    EnemyState AttackingUpdate()
    {
        if (groundCheck.IsGrounded() == false) return EnemyState.InAir;
        
        if (stateMachine.timeInState >= attackingDuration) return EnemyState.Wandering;
        else return stateMachine.currentState;
    }

    void AttackingExit()
    {
        currentAttackingEnemies -= 1;
    }

    void HurtEnter()
    {
        print("Enemy: Ouch!!");
        currentAttackingEnemies -= 1;
    }
    
    EnemyState HurtUpdate()
    {
        hurtTimeLeft -= Time.deltaTime;
        if (hurtTimeLeft <= 0) return EnemyState.Aggressive;
        else return stateMachine.currentState;
    }

    EnemyState GrabbedUpdate()
    {
        if (stateMachine.timeInState >= grabTimeToEscape)
        {
            GetComponent<Grabbable>().ForceRelease();
            return EnemyState.Aggressive;
        }

        return stateMachine.currentState;
    }

    EnemyState InAirUpdate()
    {
        if (groundCheck.IsGrounded()) return EnemyState.Wandering;
        return stateMachine.currentState;
    }

    void InAirExit()
    {
        if (thrownDamageQueue)
        {
            var dmg = throwBaseDamage * rb.mass;
            health.Damage(new DamageInfo(dmg, Vector2.zero, AuraType.Throw));
            thrownDamageQueue = false;
        }
    }

    void OnGrabCallback() => stateMachine.SetState(EnemyState.Grabbed);

    void OnThrowCallback()
    {
        thrownDamageQueue = true;
        stateMachine.SetState(EnemyState.InAir);
    }
}
