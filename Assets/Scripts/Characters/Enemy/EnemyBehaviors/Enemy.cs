using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

// NOTE: exploder enemy works around the states.
// if you add a new state, make sure exploder enemy
// can handle it properly with it's countdown state.
public enum EnemyState
{
    Spawning,
    Wandering, 
    Aggressive, 
    Attacking, 
    Hurt, 
    Grabbed, 
    InAir
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
    [SerializeField] protected float walkingSpeed;

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
    const float wanderZMin = -0.15f, wanderZMax = 0.15f; // each time the enemy chooses to wander, it's new z position must be within these bounds
    const float wanderTimeBetweenSteps = 3f; // when wandering, the enemy takes a few steps with this period

    const float grabTimeToEscape = 5f; // enemy will break out of a grab after this long

    private float wanderingTimeTillAttack = 0;
    private Vector3 wanderingToPosition;
    private float wanderingTimeTillWander;

    /// <summary>
    /// If the enemy is in the approaching state, this value will be the object it's going towards.
    /// Otherwise, it's value is meaningless.
    /// </summary>
    protected Transform aggressiveCurrentTarget = null;

    // TODO: Serialize field? Use animation triggers instead?
    const float initialHurtTime = .75f;
    private float hurtTimeLeft = initialHurtTime;
    
    /// <summary>
    /// When the enemy is thrown, it queues its damaged, taking it when it lands.
    /// </summary>
    private bool thrownDamageQueue = false;
    
    protected GroundCheck groundCheck;
    protected Grabbable grabbable;
    protected Rigidbody rb;
    private Health health;
    protected StateMachine<EnemyState> stateMachine = new();

    [SerializeField] protected GameObject debugMarkerPrefab;

    private DebugMarker wanderingMarker;

#if UNITY_EDITOR
    [ReadOnlyInInspector]
#endif
    [SerializeField] private EnemyState state;

    protected virtual void Start()
    {
        this.GetComponentOrError(out rb);
        this.GetComponentOrError(out grabbable);
        this.GetComponentOrError(out health);
        this.GetComponentInChildrenOrError(out groundCheck);

        stateMachine.AddState(EnemyState.Spawning, SpawningEnter, SpawningUpdate, SpawningExit);
        stateMachine.AddState(EnemyState.Wandering, WanderingEnter, WanderingUpdate, WanderingExit);
        stateMachine.AddState(EnemyState.Aggressive, AggressiveEnterExt, AggressiveUpdate, AggressiveExitExt);
        stateMachine.AddState(EnemyState.Attacking, AttackingEnter, AttackingUpdate, AttackingExitExt);
        stateMachine.AddState(EnemyState.Hurt, HurtEnter, HurtUpdate, HurtExit);
        stateMachine.AddState(EnemyState.Grabbed, GrabbedEnter, GrabbedUpdate, GrabbedExit);
        stateMachine.AddState(EnemyState.InAir, InAirEnter, InAirUpdate, InAirExit);
        stateMachine.FinalizeAndSetState(EnemyState.Spawning);

        grabbable.onGrab.AddListener(OnGrabCallback);
        grabbable.onForceRelease.AddListener(OnForceReleaseCallback);
        grabbable.onThrow.AddListener(OnThrowCallback);
        
        health.onHurt += OnHurt;
    }

    protected virtual void OnHurt(DamageInfo damageInfo) {
        SoundManager.Instance.PlaySoundAtPosition("EnemyHurt", transform.position);
        stateMachine.SetState(EnemyState.Hurt);
    }

    private void Update()
    {
        stateMachine.Update();
        if (rb.isKinematic == false) rb.velocity += Vector3.down * gravity * Time.deltaTime;

        if (rb.velocity.x < 0) transform.localEulerAngles = new Vector3(0, 180, 0);
        else if (rb.velocity.x > 0) transform.localEulerAngles = Vector3.zero;

        state = stateMachine.currentState;

        if (transform.position.y < -1000)
        {
            Debug.LogError($"Enemy ({name}) fell to the death plane! This should never happen.");
            health.Damage(new DamageInfo(this.gameObject, 1000000, Vector2.zero, AuraType.Everything));
        }
    }

    /// <summary>
    /// Sets the rigidbody's velocity so that it walks, with path-finding, to the target. <br/>
    /// Returns true if it has reached the target. <br/>
    /// Note that the velocity will be maintained when this function is no longer called. Make sure to set it to zero when you want to stop moving. <br/>
    /// </summary>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    protected bool WalkTowards(Vector3 target, float speed)
    {
        var path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
        
        Vector3 firstPoint;
        if (path.corners.Length >= 2) firstPoint = path.corners[1];
        else if (path.corners.Length == 1) firstPoint = path.corners[0];
        else return true;
        
        rb.velocity = (firstPoint - transform.position).normalized * speed;
        if (Vector3.Distance(firstPoint, transform.position) <= 0.1)
        {
            rb.velocity = Vector3.zero;
            return true;
        }
        else return false;
    }
    
    protected virtual void SpawningEnter() {
        // Enemy should be disabled until it lands
        health.enabled = false;
    }
    protected virtual EnemyState SpawningUpdate() { 
        if(groundCheck.IsGrounded())
        {
            return EnemyState.Wandering;
        }

        return stateMachine.currentState; 
    }
    protected virtual void SpawningExit(EnemyState newState) {
        health.enabled = true;
    }


    protected virtual void WanderingEnter()
    {
        wanderingTimeTillAttack = Random.Range(wanderingTimeMin, wanderingTimeMax);
        wanderingToPosition = transform.position;
    }
    
    protected virtual EnemyState WanderingUpdate()
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
        
        // approach randomly set wander point
        WalkTowards(wanderingToPosition, walkingSpeed);
        
        // calculate new wander point after some time has passed
        if (wanderingTimeTillWander < 0)
        {
            wanderingToPosition = new Vector3(
                transform.position.x + Random.Range(-wanderXDistance, +wanderXDistance),
                transform.position.y,
                Random.Range(wanderZMin, wanderZMax)
            );
            wanderingTimeTillWander = wanderTimeBetweenSteps;

            // draw editor-only debug point
            if (Application.isEditor) {
                if (wanderingMarker != null) {
                    Destroy(wanderingMarker.gameObject);
                }
                wanderingMarker = DebugMarker.Instantiate(debugMarkerPrefab, wanderingToPosition, Color.blue);
            }
        }

        wanderingTimeTillWander -= Time.deltaTime;
        
        return stateMachine.currentState;
    }

    protected virtual void WanderingExit(EnemyState _newState) {
        if (wanderingMarker != null) {
            Destroy(wanderingMarker.gameObject);
        }
    }

    protected virtual void AggressiveEnter()
    {
        // start approaching the nearest player
        aggressiveCurrentTarget =
            FindObjectsOfType<Player>()
            .OrderBy(p => Vector3.Distance(this.transform.position, p.transform.position))
            .First().transform;
    }

    // The "ext" pattern here ensures a programmer doesn't accidentally override this code
    // and break the currentAttackingEnemies invariant.
    // non-ext code should contain behavior-specific code.
    void AggressiveEnterExt() {
        currentAttackingEnemies += 1;
        AggressiveEnter();
    }
    
    protected virtual EnemyState AggressiveUpdate()
    {
        if (groundCheck.IsGrounded() == false) return EnemyState.InAir;

        WalkTowards(aggressiveCurrentTarget.position, walkingSpeed);

        // print($"enemy status ({gameObject.name}): aggressive, targeting {aggressiveCurrentTarget.name}"); 
         var vecToTarget = (aggressiveCurrentTarget.position - this.transform.position);
         vecToTarget.y = 0;


        if (vecToTarget.magnitude < attackingDistance)
        {
            return EnemyState.Attacking;
        }

        return stateMachine.currentState;
    }

    void AggressiveExitExt(EnemyState newState) {
        if (newState != EnemyState.Attacking) {
            currentAttackingEnemies -= 1;
        } 
        AggressiveExit(newState);
    }

    protected virtual void AggressiveExit(EnemyState newState) { }

    protected virtual void AttackingEnter() { }

    protected virtual EnemyState AttackingUpdate()
    {
        rb.velocity = Vector3.zero;
        return stateMachine.currentState;
    }

    protected virtual void AttackingExit(EnemyState newState) {}

    void AttackingExitExt(EnemyState newState)
    {
        currentAttackingEnemies -= 1;
        AttackingExit(newState);
    }

    protected virtual void HurtEnter()
    {
        hurtTimeLeft = initialHurtTime;
    }
    
    protected virtual EnemyState HurtUpdate()
    {
        hurtTimeLeft -= Time.deltaTime;
        if (hurtTimeLeft <= 0) return EnemyState.Aggressive;
        else return stateMachine.currentState;
    }

    protected virtual void HurtExit(EnemyState newState) {}

    protected virtual void GrabbedEnter() {}

    protected virtual EnemyState GrabbedUpdate()
    {
        if (stateMachine.timeInState >= grabTimeToEscape)
        {
            grabbable.ForceRelease();
            return EnemyState.Aggressive;
        }

        return stateMachine.currentState;
    }

    protected virtual void GrabbedExit(EnemyState newState) {}

    protected virtual void InAirEnter() {}

    protected virtual EnemyState InAirUpdate()
    {
        if (groundCheck.IsGrounded()) return EnemyState.Wandering;
        return stateMachine.currentState;
    }

    protected virtual void InAirExit(EnemyState newState)
    {
        // make sure we're not being regrabbed, because then there shouldn't be
        // any damage being taken
        if (thrownDamageQueue && newState != EnemyState.Grabbed)
        {
            thrownDamageQueue = false;
            var dmg = throwBaseDamage * rb.mass;
            health.Damage(new DamageInfo(gameObject, dmg, Vector2.zero, AuraType.Throw));
        }
    }

    void OnGrabCallback() => stateMachine.SetState(EnemyState.Grabbed);

    void OnThrowCallback()
    {
        thrownDamageQueue = true;
        stateMachine.SetState(EnemyState.InAir);
    }

    void OnForceReleaseCallback()
    {
        stateMachine.SetState(EnemyState.InAir);
    }

    private void OnDestroy() {
        health.onHurt -= OnHurt;
    }
}
