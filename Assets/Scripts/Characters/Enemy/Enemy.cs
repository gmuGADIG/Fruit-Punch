using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyState
{
    Wandering, Aggressive, Attacking, Hurt
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

    [SerializeField] private float wanderingTimeMin = 5000;
    [SerializeField] private float wanderingTimeMax = 10000;
    
    protected BeltCharacter beltChar;
    protected StateMachine<EnemyState> stateMachine = new();

    #region Wandering-State Values
    [SerializeField, ReadOnlyInInspector] private float wanderingTimeTillAttack = 0;
    [SerializeField, ReadOnlyInInspector] private Vector3 wanderingToPosition;
    [SerializeField, ReadOnlyInInspector] private float wanderingTimeTillWander;
    #endregion
    
    #region Aggressive-State Values
    /// <summary>
    /// If the enemy is in the approaching state, this value will be the object it's going towards.
    /// Otherwise, it's value is meaningless.
    /// </summary>
    protected BeltCharacter aggressiveCurrentTarget = null;
    #endregion

    #region Attacking-State Values
    private float attackingTimeLeft;
    #endregion

    #region Hurt-State Values
    private float hurtTimeLeft;
    #endregion

    private void Start()
    {
        this.GetComponentOrError(out beltChar);
        stateMachine.AddState(EnemyState.Wandering, WanderingEnter, WanderingUpdate, null);
        stateMachine.AddState(EnemyState.Aggressive, AggressiveEnterExt, AggressiveUpdate, AggressiveExit);
        stateMachine.AddState(EnemyState.Attacking, AttackingEnter, AttackingUpdate, AttackingExitExt);
        stateMachine.AddState(EnemyState.Hurt, HurtEnter, HurtUpdate, null);
        stateMachine.FinalizeAndSetState(EnemyState.Wandering);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    void WanderingEnter()
    {
        wanderingTimeTillAttack = Random.Range(wanderingTimeMin, wanderingTimeMax);

        wanderingToPosition = beltChar.internalPosition;
    }
    
    EnemyState WanderingUpdate()
    {
        // changing to aggressive state after waiting some time
        wanderingTimeTillAttack -= Time.deltaTime;
        if (wanderingTimeTillAttack <= 0)
        {
            if (currentAttackingEnemies >= MaxSimultaneousAttackers) // TODO: multiply by player count
            {
                return EnemyState.Aggressive;
            }
        }
        
        // random movement
        beltChar.internalPosition = Vector3.MoveTowards(beltChar.internalPosition, wanderingToPosition, walkingSpeed * Time.deltaTime);
        if (wanderingTimeTillWander < 0)
        {
            wanderingToPosition = beltChar.internalPosition + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
            wanderingTimeTillWander = 3;
        }

        wanderingTimeTillWander -= Time.deltaTime;
        
        return stateMachine.currentState;
    }

    protected virtual void AggressiveEnter()
    {
        // find all player belt characters
        var playerBeltChars =
            FindObjectsOfType<Player>()
            .Select(x => x.GetComponent<BeltCharacter>())
            .Where(x => x != null);

        // start approaching the nearest one
        // (note: sometimes it feels like differences in z-position are exaggerated for this? might want to use transform position instead, idk)
        aggressiveCurrentTarget =
            playerBeltChars
            .OrderBy(bc => Vector3.Distance(this.beltChar.internalPosition, bc.internalPosition))
            .First();
    }

    void AggressiveEnterExt() {
        currentAttackingEnemies += 1;
        AggressiveEnter();
    }
    
    protected virtual EnemyState AggressiveUpdate()
    {
        var walkTo = aggressiveCurrentTarget.internalPosition;
        walkTo.y = this.beltChar.internalPosition.y;
        beltChar.internalPosition = Vector3.MoveTowards(
            beltChar.internalPosition,
            walkTo,
            walkingSpeed * Time.deltaTime
        );

        if (Vector3.Distance(beltChar.internalPosition, walkTo) < attackingDistance)
        {
            Debug.Log("I'm attached to: " + aggressiveCurrentTarget.name);
            Component[] c = aggressiveCurrentTarget.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour script in c)
            {
                Debug.Log(script);
            }

            Health health = aggressiveCurrentTarget.GetComponent<Health>();

            // HARDCODED DAMAGE - TODO: make this a variable
            //health.Damage(new DamageInfo(90, Vector2.zero, AuraType.Strike));
            return EnemyState.Attacking;
        }

        return stateMachine.currentState;
    }

    protected virtual void AggressiveExit() {}

    protected virtual void AttackingEnter()
    {
        attackingTimeLeft = 2;
        // print("Enemy: WHAM!!");
    }
    
    protected virtual EnemyState AttackingUpdate()
    {
        attackingTimeLeft -= Time.deltaTime;
        if (attackingTimeLeft <= 0) return EnemyState.Wandering;
        else return stateMachine.currentState;
    }

    protected virtual void AttackingExit() {}

    void AttackingExitExt()
    {
        currentAttackingEnemies -= 1;
        AttackingExit();
    }

    public void Hurt(DamageInfo info)
    {
        // print($"health down to {100 - damage}");
        // stateMachine.SetState(EnemyState.Hurt);
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
}
